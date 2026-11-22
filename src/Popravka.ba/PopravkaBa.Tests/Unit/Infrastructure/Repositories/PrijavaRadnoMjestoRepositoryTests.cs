using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class PrijavaRadnoMjestoRepositoryCoverageTests
{
    [TestMethod]
    public async Task DodajIUrediAsync_PostojećaPrijava_AžuriraStatus()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.Add(new Majstor { Id = "m1", Adresa = "Sarajevo" });
        var repository = new PrijavaRadnoMjestoRepository(context);
        var prijava = new PrijavaRadnoMjesto { ID = 1, OglasID = 10, MajstorID = "m1", StatusPrijave = Status.NaCekanju };

        await repository.DodajAsync(prijava);
        prijava.StatusPrijave = Status.Prihvaceno;
        await repository.UrediAsync(prijava);

        Assert.AreEqual(Status.Prihvaceno, (await context.PrijaveRadnoMjesto.FindAsync(1))!.StatusPrijave);
    }

    [TestMethod]
    public async Task DajPoIdAsync_PostojećiINepostojećiId_VracaOcekivaniRezultat()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.Add(new Majstor { Id = "m1", Adresa = "Sarajevo" });
        context.OglasiRadnogMjesta.Add(new OglasRadnoMjesto { OglasID = 10, Naslov = "Posao", Opis = "Opis", VlasnikOglasaID = "owner", MjestoID = 1 });
        context.PrijaveRadnoMjesto.Add(new PrijavaRadnoMjesto { ID = 1, OglasID = 10, MajstorID = "m1" });
        await context.SaveChangesAsync();
        var repository = new PrijavaRadnoMjestoRepository(context);

        var postoji = await repository.DajPoIdAsync(1);
        var nepostoji = await repository.DajPoIdAsync(999);

        Assert.IsNotNull(postoji);
        Assert.IsNull(nepostoji);
    }

    [TestMethod]
    public async Task DajSvePrijaveOglasaAsync_DvijePrijave_VracaSortiranePrijave()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.AddRange(new Majstor { Id = "m1", Adresa = "Sarajevo" }, new Majstor { Id = "m2", Adresa = "Mostar" });
        context.OglasiRadnogMjesta.Add(new OglasRadnoMjesto { OglasID = 10, Naslov = "Posao", Opis = "Opis", VlasnikOglasaID = "owner", MjestoID = 1 });
        context.PrijaveRadnoMjesto.AddRange(
            new PrijavaRadnoMjesto { ID = 1, OglasID = 10, MajstorID = "m1", VrijemePrijave = new DateTime(2026, 11, 20) },
            new PrijavaRadnoMjesto { ID = 2, OglasID = 10, MajstorID = "m2", VrijemePrijave = new DateTime(2026, 11, 21) });
        await context.SaveChangesAsync();
        var repository = new PrijavaRadnoMjestoRepository(context);

        var rezultat = (await repository.DajSvePrijaveOglasaAsync(10)).ToList();

        Assert.AreEqual(2, rezultat.Count);
        Assert.AreEqual(1, rezultat[0].ID);
        Assert.AreEqual(2, rezultat[1].ID);
    }

    [TestMethod]
    public async Task ObrisiAsync_PostojećaINepostojećaPrijava_UklanjaSamoPostojeću()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.PrijaveRadnoMjesto.Add(new PrijavaRadnoMjesto { ID = 1, OglasID = 10, MajstorID = "m1" });
        await context.SaveChangesAsync();
        var repository = new PrijavaRadnoMjestoRepository(context);

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);

        Assert.IsNull(await context.PrijaveRadnoMjesto.FindAsync(1));
    }
}
