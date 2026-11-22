using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Tests.Helpers;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class PonudaUslugeRepositoryCoverageTests
{
    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [TestMethod]
    public async Task DodajAsync_NovaPonuda_SpremaPonudu()
    {
        await using var context = CreateContext();
        context.OglasiUsluga.Add(new OglasUsluge { OglasID = 10, Naslov = "Oglas", Opis = "Opis", VlasnikOglasaID = "k1", MjestoID = 1, Ponude = [] });
        context.ApplicationUsers.Add(new Majstor { Id = "i1", Adresa = "Sarajevo" });
        await context.SaveChangesAsync();
        var repository = new PonudaUslugeRepository(context);
        var ponuda = new PonudaUsluge { ID = 1, IzvrsilacID = "i1", OglasUslugeID = 10, Cijena = 100 };

        await repository.DodajAsync(ponuda);

        Assert.IsNotNull(await repository.DajPoIdAsync(1));
    }

    [TestMethod]
    public async Task UrediAsync_PostojećaPonuda_AžuriraCijenu()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.Add(new Majstor { Id = "i1", Adresa = "Sarajevo" });
        context.OglasiUsluga.Add(new OglasUsluge { OglasID = 10, Naslov = "Oglas", Opis = "Opis", VlasnikOglasaID = "k1", MjestoID = 1, Ponude = [] });
        context.PonudeUsluge.Add(new PonudaUsluge { ID = 1, IzvrsilacID = "i1", OglasUslugeID = 10, Cijena = 100 });
        await context.SaveChangesAsync();
        var repository = new PonudaUslugeRepository(context);
        var ponuda = await context.PonudeUsluge.FindAsync(1);

        ponuda!.Cijena = 150;
        await repository.UrediAsync(ponuda);

        Assert.AreEqual(150, (await context.PonudeUsluge.FindAsync(1))!.Cijena);
    }

    [TestMethod]
    public async Task DajSvePonude_PostojećePonude_VracaPonudeZaOglasIIzvrsioca()
    {
        await using var context = CreateContext();
        context.ApplicationUsers.AddRange(new Majstor { Id = "i1", Adresa = "Sarajevo" }, new Majstor { Id = "i2", Adresa = "Mostar" });
        context.OglasiUsluga.AddRange(
            new OglasUsluge { OglasID = 10, Naslov = "Prvi", Opis = "Opis", VlasnikOglasaID = "k1", MjestoID = 1, Ponude = [] },
            new OglasUsluge { OglasID = 11, Naslov = "Drugi", Opis = "Opis", VlasnikOglasaID = "k1", MjestoID = 1, Ponude = [] });
        context.PonudeUsluge.AddRange(
            new PonudaUsluge { ID = 2, IzvrsilacID = "i1", OglasUslugeID = 10, Cijena = 200 },
            new PonudaUsluge { ID = 3, IzvrsilacID = "i2", OglasUslugeID = 11, Cijena = 300 });
        await context.SaveChangesAsync();
        var repository = new PonudaUslugeRepository(context);

        var ponudeOglasa = await repository.DajSvePonudeOglasaAsync(10);
        var ponudeIzvrsioca = await repository.DajSvePonudeIzvrsiocaAsync("i1");

        Assert.AreEqual(1, ponudeOglasa.Count());
        Assert.AreEqual(1, ponudeIzvrsioca.Count());
    }

    [TestMethod]
    public async Task ObrisiAsync_PostojećaINepostojećaPonuda_UklanjaSamoPostojeću()
    {
        await using var context = CreateContext();
        context.PonudeUsluge.Add(new PonudaUsluge { ID = 1, IzvrsilacID = "i1", OglasUslugeID = 10, Cijena = 100 });
        await context.SaveChangesAsync();
        var repository = new PonudaUslugeRepository(context);

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);

        Assert.IsNull(await context.PonudeUsluge.FindAsync(1));
    }

    public static IEnumerable<object[]> PonudeProsjekIzCsv =>
        CsvHelper.Ucitaj("ponude_prosjek.csv", red => new object[]
        {
            int.Parse(red[0]),
            red[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
            red[2].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
            decimal.Parse(red[3])
        });

    [TestMethod]
    [DynamicData(nameof(PonudeProsjekIzCsv))]
    public async Task DajProsjekCijenePoKategorijama_PodaciIzCsv_VracaOcekivaniProsjek(int oglasId, List<int> kategorijeOglasa, List<int> trazeneKategorije, decimal ocekivaniProsjek)
    {
        await using var context = CreateContext();
        var repository = new PonudaUslugeRepository(context);
        var oglas = new OglasUsluge { OglasID = oglasId, Naslov = "Oglas", Opis = "Opis", VlasnikOglasaID = "k1", MjestoID = 1, Ponude = [] };
        context.OglasiUsluga.Add(oglas);

        var ponuda = new PonudaUsluge { ID = oglasId, IzvrsilacID = "i1", OglasUslugeID = oglasId, Cijena = 100, OglasUsluge = oglas };
        context.PonudeUsluge.Add(ponuda);

        context.OglasKategorije.Add(new OglasKategorija { OglasID = oglasId, KategorijaID = kategorijeOglasa[0] });

        await context.SaveChangesAsync();

        var rezultat = await repository.DajProsjekCijenePoKategorijama(trazeneKategorije);

        Assert.AreEqual(ocekivaniProsjek, rezultat);
    }

    [TestMethod]
    public async Task ProsjekIpPostojećiPosao_RubniSlucajevi()
    {
        await using var context = CreateContext();
        var repository = new PonudaUslugeRepository(context);

        Assert.IsNull(await repository.DajProsjekCijenePoKategorijama([]));
        Assert.IsFalse(await repository.PostojiZavrseniPosaoAsync("k1","i1"));

        context.PonudeUsluge.Add(new PonudaUsluge { ID=1, IzvrsilacID="i1", OglasUslugeID=10, Cijena=100, StatusPonude=Status.Isporuceno });
        context.OglasiUsluga.Add(new OglasUsluge { OglasID=10, Naslov="O", Opis="O", VlasnikOglasaID="k1", MjestoID=1, Ponude=[] });
        context.OglasKategorije.Add(new OglasKategorija { OglasID=10, KategorijaID=5 });
        await context.SaveChangesAsync();

        Assert.IsTrue(await repository.PostojiZavrseniPosaoAsync("k1","i1"));
        Assert.IsNull(await repository.DajProsjekCijenePoKategorijama([999]));
    }
}
