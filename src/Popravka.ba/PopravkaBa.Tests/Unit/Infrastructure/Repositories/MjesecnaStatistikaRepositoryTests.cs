using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Popravka.ba.Data;
using PopravkaBa.Application.Strategies.Interface;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class MjesecnaStatistikaRepositoryCoverageTests
{
    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
        .Options);

    [TestMethod]
    public async Task DohvatiFiltrirano_SviFilteriIVrsteSortiranja_Radi()
    {
        await using var context = CreateContext();
        context.MjesecnaStatistikaKompozicija.AddRange(
            new MjesecnaStatistikaKompozicija { ID=1, Godina=2026, Mjesec=11, KategorijaID=1, MjestoID=9, RangStandardni=1, ProsjecnaOcjena=4.5m, DisplayName="A", Username="a", IzvrsilacID="a" },
            new MjesecnaStatistikaKompozicija { ID=2, Godina=2026, Mjesec=11, KategorijaID=2, MjestoID=9, RangStandardni=2, ProsjecnaOcjena=4m, DisplayName="B", Username="b", IzvrsilacID="b" });
        await context.SaveChangesAsync();

        var strategy = new Mock<IStatistikaStrategy>();
        strategy.Setup(s => s.Primijeni(It.IsAny<IQueryable<MjesecnaStatistikaKompozicija>>(), It.IsAny<Sortiranje>()))
            .Returns((IQueryable<MjesecnaStatistikaKompozicija> q, Sortiranje _) => q.OrderByDescending(x => x.RangStandardni));
        var resolver = new Mock<IStatistikaSortResolver>();
        resolver.Setup(r => r.SortPo(KoloneStatistike.Ocjena)).Returns(strategy.Object);
        var repository = new MjesecnaStatistikaRepository(context, resolver.Object);

        Assert.AreEqual(1, (await repository.DohvatiFiltrirano(2026,11,1,9,KoloneStatistike.Ocjena,Sortiranje.Descending)).Count);
        Assert.AreEqual(2, (await repository.DohvatiFiltrirano(2026,11,null,null,KoloneStatistike.Ocjena,Sortiranje.Ascending)).Count);
    }

    [TestMethod]
    public async Task DohvatiMeta_PrazanINeprazanPodaci_Radi()
    {
        await using var context = CreateContext();
        var resolver = new Mock<IStatistikaSortResolver>();
        var repository = new MjesecnaStatistikaRepository(context, resolver.Object);

        var prazna = await repository.DohvatiMeta(2026,11);
        Assert.AreEqual(0, prazna.Kategorije.Count);

        context.MjesecnaStatistikaKompozicija.Add(new MjesecnaStatistikaKompozicija { ID=1, Godina=2026, Mjesec=11, KategorijaID=1, KategorijaNaziv="Vodovod", MjestoID=9, MjestoNaziv="Sarajevo", RangStandardni=1, VrijemeAzuriranja=DateTime.UtcNow, DisplayName="A", Username="a", IzvrsilacID="a" });
        await context.SaveChangesAsync();
        var puna = await repository.DohvatiMeta(2026,11);
        Assert.AreEqual(1, puna.Kategorije.Count);
        Assert.AreEqual(1, puna.Lokacije.Count);
    }

    [TestMethod]
    public async Task ZamijeniSnapshot_PrazanIPun_SpremaPraviNoviSnapshot()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        var resolver = new Mock<IStatistikaSortResolver>();
        var repository = new MjesecnaStatistikaRepository(context, resolver.Object);

        await repository.ZamijeniSnapshot(2026,11,[new MjesecnaStatistikaKompozicija { ID=1, Godina=2026, Mjesec=11, RangStandardni=1, DisplayName="A", Username="a", IzvrsilacID="a" }]);
        Assert.AreEqual(1, await context.MjesecnaStatistikaKompozicija.CountAsync());

        await repository.ZamijeniSnapshot(2026,11,[]);
        Assert.AreEqual(0, await context.MjesecnaStatistikaKompozicija.CountAsync());
    }
}
