using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class KategorijaRepositoryCoverageTests
{
    [TestMethod]
    public async Task DajSveAsync_PostojeKategorije_VracaSve()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Kategorije.AddRange(new Kategorija { ID = 1, Naziv = "Vodovod" }, new Kategorija { ID = 2, Naziv = "Elektrika" });
        await context.SaveChangesAsync();
        var repository = new KategorijaRepository(context);

        var rezultat = await repository.DajSveAsync();

        Assert.AreEqual(2, rezultat.Count());
    }

    [TestMethod]
    public async Task DajPoIdAsync_PostojećiINepostojećiId_VracaOcekivaniRezultat()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Kategorije.Add(new Kategorija { ID = 1, Naziv = "Vodovod" });
        await context.SaveChangesAsync();
        var repository = new KategorijaRepository(context);

        var postojeća = await repository.DajPoIdAsync(1);
        var nepostojeća = await repository.DajPoIdAsync(999);

        Assert.IsNotNull(postojeća);
        Assert.IsNull(nepostojeća);
    }

    [TestMethod]
    public async Task DodajAzurirajIUkloniKategorijeOglasa_Promjene_VracaJednuKategoriju()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Kategorije.AddRange(new Kategorija { ID = 1, Naziv = "Vodovod" }, new Kategorija { ID = 2, Naziv = "Elektrika" });
        await context.SaveChangesAsync();
        var repository = new KategorijaRepository(context);

        await repository.DodajKategorijeOglasu(1, [1, 2]);
        await repository.DodajKategorijeOglasu(1, []);
        await repository.AzurirajKategorijeOglasa(1, [2]);

        Assert.AreEqual(1, await context.OglasKategorije.CountAsync());
        await repository.UkloniSveKategorijeOglasa(1);
        Assert.AreEqual(0, await context.OglasKategorije.CountAsync());
    }

    [TestMethod]
    public async Task DodajAzurirajIUkloniKategorijeIzvrsioca_Promjene_UklanjajuDuplikate()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Kategorije.AddRange(new Kategorija { ID = 1, Naziv = "Vodovod" }, new Kategorija { ID = 2, Naziv = "Elektrika" });
        await context.SaveChangesAsync();
        var repository = new KategorijaRepository(context);

        await repository.DodajKategorijeIzvrsiocu("i1", [1, 1, 2]);
        await repository.AzurirajKategorijeIzvrsioca("i1", [2]);

        Assert.AreEqual(1, await context.IzvrsilacKategorija.CountAsync(item => item.IzvrsilacID == "i1"));
        await repository.UkloniSveKategorijeIzvrsiocu("i1");
        Assert.AreEqual(0, await context.IzvrsilacKategorija.CountAsync(item => item.IzvrsilacID == "i1"));
    }

    [TestMethod]
    public async Task DajTopKategorijePoMajstorimaAsync_NevezaniIzvrsilaci_VracaTopN()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Kategorije.AddRange(new Kategorija { ID = 1, Naziv = "Vodovod" }, new Kategorija { ID = 2, Naziv = "Elektrika" });
        context.IzvrsilacKategorija.AddRange(new IzvrsilacKategorija { IzvrsilacID = "a", KategorijaID = 1 }, new IzvrsilacKategorija { IzvrsilacID = "b", KategorijaID = 1 }, new IzvrsilacKategorija { IzvrsilacID = "c", KategorijaID = 2 });
        await context.SaveChangesAsync();
        var repository = new KategorijaRepository(context);

        var rezultat = await repository.DajTopKategorijePoMajstorimaAsync(5);

        Assert.AreEqual(2, rezultat.Count());
    }
}
