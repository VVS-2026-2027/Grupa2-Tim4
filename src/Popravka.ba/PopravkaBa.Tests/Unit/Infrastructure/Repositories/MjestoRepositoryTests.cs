using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class MjestoRepositoryCoverageTests
{
    [TestMethod]
    public async Task DajSvaMjestaAsync_PostojeMjesta_VracaSva()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Mjesta.AddRange(new Mjesto { MjestoID = 1, Naziv = "Sarajevo", Kanton = Kanton.Sarajevski }, new Mjesto { MjestoID = 2, Naziv = "Mostar", Kanton = Kanton.HercegovackoNeretvanski });
        await context.SaveChangesAsync();
        var repository = new MjestoRepository(context);

        var rezultat = await repository.DajSvaMjestaAsync();

        Assert.AreEqual(2, rezultat.Count());
    }

    [TestMethod]
    public async Task DajPoIdAsync_PostojećiINepostojećiId_VracaOcekivaniRezultat()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo", Kanton = Kanton.Sarajevski });
        await context.SaveChangesAsync();
        var repository = new MjestoRepository(context);

        var postoji = await repository.DajPoIdAsync(1);
        var nepostoji = await repository.DajPoIdAsync(999);

        Assert.IsNotNull(postoji);
        Assert.IsNull(nepostoji);
    }

    [TestMethod]
    public async Task DodajAzurirajIUkloniMjestaKorisnika_Promjene_VracaTrenutnaMjesta()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Mjesta.AddRange(new Mjesto { MjestoID = 1, Naziv = "Sarajevo", Kanton = Kanton.Sarajevski }, new Mjesto { MjestoID = 2, Naziv = "Mostar", Kanton = Kanton.HercegovackoNeretvanski });
        await context.SaveChangesAsync();
        var repository = new MjestoRepository(context);

        await repository.DodajMjestaKorisniku("u1", [1, 2]);
        Assert.AreEqual(2, (await repository.DajSveKorisnikeMjestaAsync("u1")).Count());

        await repository.AzurirajMjestaKorisnika("u1", [2]);
        Assert.AreEqual(1, (await repository.DajSveKorisnikeMjestaAsync("u1")).Count());

        await repository.UkloniSvaMjestaKorisniku("u1");
        Assert.AreEqual(0, (await repository.DajSveKorisnikeMjestaAsync("u1")).Count());
    }

    [TestMethod]
    public async Task PretragaIOglasiPoMjestu_PrazniPodaci_VracajuPrazneRezultate()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo", Kanton = Kanton.Sarajevski });
        await context.SaveChangesAsync();
        var repository = new MjestoRepository(context);

        var pronadjena = await repository.PronadjiMjestaAsync("Sarajevo");
        var kanton = await repository.DajMjestaPoKantonuAsync(9);
        var majstori = await repository.DajOglaseMajstoraPoMjestu(1);

        Assert.AreEqual(1, pronadjena.Count());
        Assert.IsNotNull(kanton);
        Assert.AreEqual(0, majstori.Count());
        Assert.AreEqual(0, (await repository.DajOglaseRadnogMjestaPoMjestu(1)).Count());
        Assert.AreEqual(0, (await repository.DajOglaseUslugePoMjestu(1)).Count());
    }
}
