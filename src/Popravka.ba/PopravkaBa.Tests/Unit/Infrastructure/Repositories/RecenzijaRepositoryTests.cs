using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class RecenzijaRepositoryCoverageTests
{
    [TestMethod]
    public async Task DodajPoIDISacuvajAsync_NovaRecenzija_SpremaIAžuriraOcjenu()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.AddRange(new Klijent { Id = "k1", UserName = "k1" }, new Majstor { Id = "i1", Adresa = "Sarajevo" });
        var repository = new RecenzijaRepository(context);
        var recenzija = new Recenzija { RecenzijaID = 1, KlijentID = "k1", IzvrsilacID = "i1", Ocjena = 5, Komentar = "Odlicno" };

        await repository.DodajAsync(recenzija);
        recenzija.Ocjena = 4;
        await repository.SacuvajAsync(recenzija);

        Assert.IsTrue(await repository.PostojiAsync("k1", "i1"));
        Assert.AreEqual(4, (await repository.DajPoIdAsync(1))!.Ocjena);
    }

    [TestMethod]
    public async Task DajSveIPrethodneListe_PostojećeRecenzije_VracajuPretrazeneListe()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.AddRange(new Klijent { Id = "k1", UserName = "k1" }, new Majstor { Id = "i1", Adresa = "Sarajevo" });
        context.Recenzije.Add(new Recenzija { RecenzijaID = 1, KlijentID = "k1", IzvrsilacID = "i1", Ocjena = 5, Komentar = "Odlicno", Prijavljena = true });
        await context.SaveChangesAsync();
        var repository = new RecenzijaRepository(context);

        var sve = await repository.DajSveAsync();
        var prijavljene = await repository.DajPrijavljeneAsync();
        var izvodjac = await repository.DajRecenzijeIzvrsiocaAsync("i1");
        var klijent = await repository.DajRecenzijeKlijentaAsync("k1");

        Assert.AreEqual(1, sve.Count());
        Assert.AreEqual(1, prijavljene.Count());
        Assert.AreEqual(1, izvodjac.Count());
        Assert.AreEqual(1, klijent.Count());
    }

    [TestMethod]
    public async Task ObrisiAsync_PostojećaINepostojećaRecenzija_UklanjaSamoPostojeću()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Recenzije.Add(new Recenzija { RecenzijaID = 1, KlijentID = "k1", IzvrsilacID = "i1", Ocjena = 5, Komentar = "Odlicno" });
        await context.SaveChangesAsync();
        var repository = new RecenzijaRepository(context);

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);

        Assert.IsFalse(await repository.PostojiAsync("k1", "i1"));
    }
}
