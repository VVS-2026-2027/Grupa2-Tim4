using Microsoft.EntityFrameworkCore;
using Moq;
using Popravka.ba.Data;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class IzvrsilacUslugeRepositoryCoverageTests
{
    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [TestMethod]
    public async Task SviImplementiraniPutevi_PostojeceVrste_VracujuIPromjenjujuPodatke()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.Add(new Majstor { Id = "i1", Ime = "Ana", Prezime = "Anić", Adresa = "Sarajevo", Opis = "Stari", BrojZavrsenihPoslova = 4 });
        await context.SaveChangesAsync();
        var repository = new IzvrsilacUslugeRepository(context);

        Assert.IsNotNull(await repository.DajProfilPoIdAsync("i1"));
        Assert.IsNull(await repository.DajProfilPoIdAsync("nepostoji"));

        await repository.UrediAsync(new Majstor { Id = "i1", Opis = "Novi", SlikePortfolija = [] });
        await repository.OsvjeziOcjeneAsync("i1", 4.8m, 8);
        await repository.PovecajBrojZavrsenihPoslovaAsync("i1");

        var rezultat = await context.ApplicationUsers.OfType<IzvrsilacUsluge>().SingleAsync(i => i.Id == "i1");
        Assert.AreEqual("Novi", rezultat.Opis);
        Assert.AreEqual(4.8m, rezultat.ProsjecnaOcjena);
        Assert.AreEqual(8, rezultat.BrojRecenzija);
        Assert.AreEqual(5, rezultat.BrojZavrsenihPoslova);
    }

    [TestMethod]
    public async Task PronadjiAsync_SpecifikacijaBezFiltera_VracaStraniceniRezultat()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.Add(new Majstor { Id = "i1", Ime = "Ana", Prezime = "Anić", Adresa = "Sarajevo" });
        await context.SaveChangesAsync();
        var repository = new IzvrsilacUslugeRepository(context);

        var rezultat = await repository.PronadjiAsync(new AllSpecification<IzvrsilacUsluge>(), 1, 10);

        Assert.AreEqual(1, rezultat.Ukupno);
        Assert.AreEqual(1, rezultat.Stavke.Count());
    }

    [TestMethod]
    public async Task PronadjiAsync_SaSortiranjem_VracaRezultatePoTrazenomRedoslijedu()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.AddRange(
            new Majstor { Id = "i1", Ime = "Ana", Prezime = "A", Adresa = "Sarajevo" },
            new Majstor { Id = "i2", Ime = "Zana", Prezime = "Z", Adresa = "Mostar" });
        await context.SaveChangesAsync();
        var repository = new IzvrsilacUslugeRepository(context);
        var descending = new Mock<ISpecification<IzvrsilacUsluge>>();
        descending.Setup(specification => specification.ToExpression()).Returns(izvrsilac => true);
        descending.SetupGet(specification => specification.OrderByDescending).Returns(izvrsilac => izvrsilac.Ime!);
        var ascending = new Mock<ISpecification<IzvrsilacUsluge>>();
        ascending.Setup(specification => specification.ToExpression()).Returns(izvrsilac => true);
        ascending.SetupGet(specification => specification.OrderBy).Returns(izvrsilac => izvrsilac.Ime!);

        var descendingResult = await repository.PronadjiAsync(descending.Object, 1, 10);
        var ascendingResult = await repository.PronadjiAsync(ascending.Object, 1, 10);

        Assert.AreEqual("Zana", descendingResult.Stavke.First().Ime);
        Assert.AreEqual("Ana", ascendingResult.Stavke.First().Ime);
    }

    [TestMethod]
    public async Task UrediAsync_ZadrzavaPostojeceUklanjaObrisaneIDodajeNoveSlike()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.Add(new Majstor { Id = "i1", Adresa = "Sarajevo" });
        context.PortfolioSlika.AddRange(
            new PortfolioSlika { PortfolioSlikaID = 1, IzvrsilacID = "i1", URL = "zadrzana.jpg" },
            new PortfolioSlika { PortfolioSlikaID = 2, IzvrsilacID = "i1", URL = "obrisana.jpg" });
        await context.SaveChangesAsync();
        var repository = new IzvrsilacUslugeRepository(context);

        await repository.UrediAsync(new Majstor
        {
            Id = "i1",
            Opis = "Ažuriran profil",
            SlikePortfolija =
            [
                new PortfolioSlika { PortfolioSlikaID = 1, IzvrsilacID = "i1", URL = "zadrzana.jpg" },
                new PortfolioSlika { URL = "nova.jpg" }
            ]
        });

        var slike = await context.PortfolioSlika.OrderBy(slika => slika.PortfolioSlikaID).ToListAsync();
        Assert.AreEqual(2, slike.Count);
        Assert.AreEqual("zadrzana.jpg", slike[0].URL);
        Assert.AreEqual("nova.jpg", slike[1].URL);
        Assert.AreEqual("i1", slike[1].IzvrsilacID);
        Assert.AreEqual("Ažuriran profil", (await context.ApplicationUsers.OfType<IzvrsilacUsluge>().SingleAsync()).Opis);

        await repository.UrediAsync(new Majstor { Id = "i1", SlikePortfolija = null });
        Assert.AreEqual(0, await context.PortfolioSlika.CountAsync());
    }

    [TestMethod]
    public async Task AzuriranjaNepostojecegIzvrsioca_NeMijenjajuPodatke()
    {
        await using var context = CreateContext();
        var repository = new IzvrsilacUslugeRepository(context);

        await repository.UrediAsync(new Majstor { Id = "nepostojeci", SlikePortfolija = [] });
        await repository.OsvjeziOcjeneAsync("nepostojeci", 5m, 1);
        await repository.PovecajBrojZavrsenihPoslovaAsync("nepostojeci");

        Assert.AreEqual(0, await context.ApplicationUsers.CountAsync());
        Assert.AreEqual(0, await context.PortfolioSlika.CountAsync());
    }

    [TestMethod]
    public async Task StubMetode_Poziv_SveBacuNotImplementedException()
    {
        await using var context = CreateContext();
        var repository = new IzvrsilacUslugeRepository(context);

        Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await repository.DajSveAsync()).GetAwaiter().GetResult();
        Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await repository.DajPoIdAsync(1)).GetAwaiter().GetResult();
        Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await repository.DodajAsync(new Majstor())).GetAwaiter().GetResult();
        Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await repository.ObrisiAsync(1)).GetAwaiter().GetResult();
        Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await repository.IzvrsiPretraguTekstaAsync("test")).GetAwaiter().GetResult();
    }
}
