using Microsoft.AspNetCore.Identity;
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
public class MjesecnaStatistikaJobTests
{
    [TestMethod]
    public async Task StartAsync_GeneriseSnapshotZaIzvrsiocaSaZavrsenimPoslovima()
    {
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName).Options;
        var sada = DateTime.UtcNow;
        var pocetakMjeseca = new DateTime(sada.Year, sada.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        await using (var setup = new ApplicationDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.ApplicationUsers.AddRange(
                new Klijent { Id = "client", UserName = "client" },
                NewWorker("worker-1", "Ana", "Aktivna"),
                NewWorker("worker-2", "Zana", "BezOcjene"),
                NewWorker("worker-3", "Nema", "Poslove"));
            setup.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo" });
            setup.Kategorije.Add(new Kategorija { ID = 1, Naziv = "Vodoinstalacije" });
            setup.KorisnikMjesto.AddRange(
                new KorisnikMjesto { ID = 1, KorisnikID = "worker-1", MjestoID = 1 },
                new KorisnikMjesto { ID = 2, KorisnikID = "worker-2", MjestoID = 1 });
            setup.IzvrsilacKategorija.Add(new IzvrsilacKategorija { ID = 1, IzvrsilacID = "worker-1", KategorijaID = 1 });
            setup.Roles.Add(new IdentityRole { Id = "role-firma", Name = nameof(KorisnickeUloge.Firma), NormalizedName = nameof(KorisnickeUloge.Firma).ToUpperInvariant() });
            setup.UserRoles.Add(new IdentityUserRole<string> { UserId = "worker-1", RoleId = "role-firma" });
            setup.OglasiUsluga.AddRange(
                new OglasUsluge { OglasID = 1, Naslov = "Oglas 1", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "client" },
                new OglasUsluge { OglasID = 2, Naslov = "Oglas 2", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "client" });
            setup.PonudeUsluge.AddRange(
                new PonudaUsluge { ID = 1, IzvrsilacID = "worker-1", OglasUslugeID = 1, StatusPonude = Status.Isporuceno, DatumIzvrsavanjaUsluge = pocetakMjeseca.AddDays(2) },
                new PonudaUsluge { ID = 2, IzvrsilacID = "worker-2", OglasUslugeID = 2, StatusPonude = Status.Isporuceno, DatumIzvrsavanjaUsluge = pocetakMjeseca.AddDays(3) },
                new PonudaUsluge { ID = 3, IzvrsilacID = "worker-3", OglasUslugeID = 1, StatusPonude = Status.NaCekanju, DatumIzvrsavanjaUsluge = pocetakMjeseca.AddDays(4) });
            setup.Recenzije.Add(new Recenzija { RecenzijaID = 1, KlijentID = "client", IzvrsilacID = "worker-1", Ocjena = 5, Komentar = "Odlicno", DatumRecenzije = pocetakMjeseca.AddDays(1) });
            await setup.SaveChangesAsync();
        }

        IReadOnlyList<MjesecnaStatistikaKompozicija>? snapshot = null;
        var snapshotSaved = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var repository = new Mock<IMjesecnaStatistikaRepository>();
        repository.Setup(repo => repo.ZamijeniSnapshot(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IReadOnlyList<MjesecnaStatistikaKompozicija>>(), It.IsAny<CancellationToken>()))
            .Callback<int, int, IReadOnlyList<MjesecnaStatistikaKompozicija>, CancellationToken>((_, _, rows, _) =>
            {
                snapshot = rows;
                snapshotSaved.TrySetResult();
            })
            .Returns(Task.CompletedTask);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(builder => builder.UseInMemoryDatabase(databaseName));
        services.AddScoped(_ => repository.Object);
        await using var provider = services.BuildServiceProvider();
        var logger = new SignalLogger<MjesecnaStatistikaJob>();
        var job = new MjesecnaStatistikaJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);

        await job.StartAsync(CancellationToken.None);
        await Task.WhenAny(snapshotSaved.Task, logger.Logged, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.IsTrue(snapshotSaved.Task.IsCompleted, logger.Exception?.ToString() ?? logger.Message ?? "Monthly statistics job did not finish in time.");
        await job.StopAsync(CancellationToken.None);

        Assert.IsNotNull(snapshot);
        Assert.AreEqual(2, snapshot.Count);
        var firstWorker = snapshot.Single(row => row.IzvrsilacID == "worker-1");
        Assert.AreEqual(5m, firstWorker.ProsjecnaOcjena);
        Assert.AreEqual(1, firstWorker.BrojPoslova);
        Assert.AreEqual("Vodoinstalacije", firstWorker.KategorijaNaziv);
        Assert.AreEqual("Sarajevo", firstWorker.MjestoNaziv);
        Assert.AreEqual(KorisnickeUloge.Firma, firstWorker.TipKorisnika);
        var secondWorker = snapshot.Single(row => row.IzvrsilacID == "worker-2");
        Assert.AreEqual(0m, secondWorker.ProsjecnaOcjena);
        Assert.AreEqual(KorisnickeUloge.Majstor, secondWorker.TipKorisnika);
        Assert.AreEqual(pocetakMjeseca.Year, firstWorker.Godina);
        Assert.AreEqual(pocetakMjeseca.Month, firstWorker.Mjesec);
        repository.Verify(repo => repo.ZamijeniSnapshot(pocetakMjeseca.Year, pocetakMjeseca.Month, It.IsAny<IReadOnlyList<MjesecnaStatistikaKompozicija>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_KadaSnapshotSpremanjeNeUspije_BiljeziGresku()
    {
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName).Options;
        await using (var setup = new ApplicationDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
        }

        var repository = new Mock<IMjesecnaStatistikaRepository>();
        repository.Setup(repo => repo.ZamijeniSnapshot(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IReadOnlyList<MjesecnaStatistikaKompozicija>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Snapshot test failure"));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(builder => builder.UseInMemoryDatabase(databaseName));
        services.AddScoped(_ => repository.Object);
        await using var provider = services.BuildServiceProvider();
        var logger = new SignalLogger<MjesecnaStatistikaJob>();
        var job = new MjesecnaStatistikaJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);

        await job.StartAsync(CancellationToken.None);
        Assert.AreEqual(Microsoft.Extensions.Logging.LogLevel.Error, await logger.Logged.WaitAsync(TimeSpan.FromSeconds(5)));
        await job.StopAsync(CancellationToken.None);
    }

    private static Majstor NewWorker(string id, string firstName, string lastName) => new()
    {
        Id = id,
        UserName = id,
        Ime = firstName,
        Prezime = lastName,
        Adresa = "Sarajevo"
    };
}
