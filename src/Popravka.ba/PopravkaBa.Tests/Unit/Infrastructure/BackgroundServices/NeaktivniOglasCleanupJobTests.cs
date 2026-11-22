using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.BackgroundServices;

namespace PopravkaBa.Tests.Unit.Infrastructure.BackgroundServices;

[TestClass]
public class NeaktivniOglasCleanupJobTests
{
    [TestMethod]
    public async Task StartAsync_BriseZastarjeleIOznaceneOglase_CuvaAktivneOglaseSAktivnostima_ISaljeObavijesti()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
        await using (var setup = new ApplicationDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.ApplicationUsers.AddRange(
                new ApplicationUser { Id = "owner", UserName = "owner", Email = "owner@example.com", Ime = "Test", Prezime = "Vlasnik" },
                new ApplicationUser { Id = "no-email", UserName = "no-email" },
                new Majstor { Id = "worker", UserName = "worker", Adresa = "Sarajevo" });
            setup.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo" });
            setup.OglasiUsluga.AddRange(
                NewServiceAd(1, "owner", DateTime.UtcNow.AddDays(-20), Status.Aktivan),
                NewServiceAd(2, "owner", DateTime.UtcNow.AddDays(-2), Status.Aktivan),
                NewServiceAd(3, "owner", DateTime.UtcNow.AddDays(-2), Status.Neaktivan),
                NewServiceAd(4, "no-email", DateTime.UtcNow.AddDays(-2), Status.Neaktivan),
                NewServiceAd(5, "owner", DateTime.UtcNow.AddDays(-20), Status.Aktivan));
            setup.OglasiRadnogMjesta.AddRange(
                NewJobAd(11, "owner", DateTime.UtcNow.AddDays(-20), Status.Aktivan),
                NewJobAd(12, "owner", DateTime.UtcNow.AddDays(-2), Status.Aktivan),
                NewJobAd(13, "owner", DateTime.UtcNow.AddDays(-2), Status.Neaktivan),
                NewJobAd(14, "owner", DateTime.UtcNow.AddDays(-20), Status.Aktivan));
            setup.PonudeUsluge.AddRange(
                new PonudaUsluge { ID = 1, OglasUslugeID = 2, IzvrsilacID = "worker" },
                new PonudaUsluge { ID = 2, OglasUslugeID = 5, IzvrsilacID = "worker" });
            setup.PrijaveRadnoMjesto.AddRange(
                new PrijavaRadnoMjesto { ID = 1, OglasID = 12, MajstorID = "worker" },
                new PrijavaRadnoMjesto { ID = 2, OglasID = 14, MajstorID = "worker" });
            await setup.SaveChangesAsync();
        }

        var attemptedEmails = new List<int>();
        var allEmailsAttempted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var emailSender = new Mock<IEmailSender>();
        emailSender.Setup(sender => sender.PosaljiEmailAsync(It.IsAny<EmailNotifikacijaOglas>()))
            .Callback<EmailNotifikacijaOglas>(notification =>
            {
                lock (attemptedEmails)
                {
                    attemptedEmails.Add(notification.OglasID);
                    if (attemptedEmails.Count == 4)
                        allEmailsAttempted.TrySetResult();
                }
            })
            .Returns<EmailNotifikacijaOglas>(notification => notification.OglasID == 3
                ? Task.FromException(new InvalidOperationException("Email test failure"))
                : Task.CompletedTask);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlite(connection));
        services.AddScoped(_ => emailSender.Object);
        await using var provider = services.BuildServiceProvider();
        var logger = new SignalLogger<NeaktivniOglasCleanupJob>();
        var job = new NeaktivniOglasCleanupJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);

        await job.StartAsync(CancellationToken.None);
        await allEmailsAttempted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await job.StopAsync(CancellationToken.None);

        CollectionAssert.AreEquivalent(new[] { 1, 3, 11, 13 }, attemptedEmails);
        await using var verify = new ApplicationDbContext(options);
        CollectionAssert.AreEquivalent(new[] { 2, 5 }, await verify.OglasiUsluga.Select(ad => ad.OglasID).ToArrayAsync());
        CollectionAssert.AreEquivalent(new[] { 12, 14 }, await verify.OglasiRadnogMjesta.Select(ad => ad.OglasID).ToArrayAsync());
        emailSender.Verify(sender => sender.PosaljiEmailAsync(It.IsAny<EmailNotifikacijaOglas>()), Times.Exactly(4));
    }

    [TestMethod]
    public async Task StartAsync_KadaDbContextNedostaje_BiljeziGresku()
    {
        await using var provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        var logger = new SignalLogger<NeaktivniOglasCleanupJob>();
        var job = new NeaktivniOglasCleanupJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);

        await job.StartAsync(CancellationToken.None);
        Assert.AreEqual(Microsoft.Extensions.Logging.LogLevel.Error, await logger.Logged.WaitAsync(TimeSpan.FromSeconds(5)));
        await job.StopAsync(CancellationToken.None);
    }

    private static OglasUsluge NewServiceAd(int id, string ownerId, DateTime published, Status status) => new()
    {
        OglasID = id,
        Naslov = $"Usluga {id}",
        Opis = "Opis usluge",
        MjestoID = 1,
        VlasnikOglasaID = ownerId,
        DatumObjave = published,
        StatusOglasa = status,
        Ponude = []
    };

    private static OglasRadnoMjesto NewJobAd(int id, string ownerId, DateTime published, Status status) => new()
    {
        OglasID = id,
        Naslov = $"Posao {id}",
        Opis = "Opis posla",
        MjestoID = 1,
        VlasnikOglasaID = ownerId,
        DatumObjave = published,
        StatusOglasa = status,
        Prijave = [],
        Uvjeti = [],
        VozackeDozvole = []
    };
}
