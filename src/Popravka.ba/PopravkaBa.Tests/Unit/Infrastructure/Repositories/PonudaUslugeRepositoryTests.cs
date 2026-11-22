using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class PonudaUslugeRepositoryCoverageTests
{
    private static ApplicationDbContext CreateContext() => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [TestMethod]
    public async Task CRUD_SveOsnovneOperacije_Radi()
    {
        await using var context = CreateContext();
        var repository = new PonudaUslugeRepository(context);
        var ponuda = new PonudaUsluge { ID=1, IzvrsilacID="i1", OglasUslugeID=10, Cijena=100, DatumSlanja=new DateTime(2026,11,20) };

        await repository.DodajAsync(ponuda);
        Assert.IsNotNull(await context.PonudeUsluge.FindAsync(1));
        Assert.IsNotNull(await context.PonudeUsluge.FindAsync(1));
        _ = await repository.DajPoIdAsync(1);

        ponuda.Cijena=150;
        await repository.UrediAsync(ponuda);
        Assert.AreEqual(150, (await context.PonudeUsluge.FindAsync(1))!.Cijena);

        context.PonudeUsluge.AddRange(
            new PonudaUsluge { ID=2, IzvrsilacID="i1", OglasUslugeID=10, Cijena=200, DatumSlanja=new DateTime(2026,11,21) },
            new PonudaUsluge { ID=3, IzvrsilacID="i2", OglasUslugeID=11, Cijena=300, DatumSlanja=new DateTime(2026,11,22) });
        await context.SaveChangesAsync();

        _ = await repository.DajSvePonudeOglasaAsync(10);
        _ = await repository.DajSvePonudeIzvrsiocaAsync("i1");

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);
        Assert.IsNull(await context.PonudeUsluge.FindAsync(1));
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
