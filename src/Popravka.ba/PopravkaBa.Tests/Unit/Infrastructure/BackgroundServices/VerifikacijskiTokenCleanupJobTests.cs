using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Popravka.ba.Data;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.BackgroundServices;

namespace PopravkaBa.Tests.Unit.Infrastructure.BackgroundServices;

[TestClass]
public class VerifikacijskiTokenCleanupJobTests
{
    [TestMethod]
    public async Task StartAsync_BriseSamoIstekleTokene()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
        await using (var setup = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.ApplicationUsers.Add(new ApplicationUser { Id = "user-1", UserName = "user-1" });
            setup.Tokeni.AddRange(
                new VerifikacijskiToken { ID = 1, KorisnikID = "user-1", TokenHash = "expired", VrijemeIsteka = DateTime.UtcNow.AddMinutes(-1) },
                new VerifikacijskiToken { ID = 2, KorisnikID = "user-1", TokenHash = "valid", VrijemeIsteka = DateTime.UtcNow.AddDays(1) });
            await setup.SaveChangesAsync();
        }

        await using var provider = services.BuildServiceProvider();
        var logger = new SignalLogger<VerifikacijskiTokenCleanupJob>();
        var job = new VerifikacijskiTokenCleanupJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);
        await job.StartAsync(CancellationToken.None);
        Assert.AreEqual(Microsoft.Extensions.Logging.LogLevel.Information, await logger.Logged.WaitAsync(TimeSpan.FromSeconds(5)));
        await job.StopAsync(CancellationToken.None);

        await using var verify = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options);
        CollectionAssert.AreEqual(new[] { 2 }, await verify.Tokeni.Select(token => token.ID).ToArrayAsync());
    }

    [TestMethod]
    public async Task StartAsync_KadaDbContextNedostaje_BiljeziGresku()
    {
        await using var provider = new ServiceCollection().AddLogging().BuildServiceProvider();
        var logger = new SignalLogger<VerifikacijskiTokenCleanupJob>();
        var job = new VerifikacijskiTokenCleanupJob(provider.GetRequiredService<IServiceScopeFactory>(), logger);

        await job.StartAsync(CancellationToken.None);
        Assert.AreEqual(Microsoft.Extensions.Logging.LogLevel.Error, await logger.Logged.WaitAsync(TimeSpan.FromSeconds(5)));
        await job.StopAsync(CancellationToken.None);
    }
}
