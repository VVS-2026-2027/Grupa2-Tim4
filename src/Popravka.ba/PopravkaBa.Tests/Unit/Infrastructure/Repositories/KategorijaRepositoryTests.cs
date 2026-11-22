using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class KategorijaRepositoryCoverageTests
{
    [TestMethod]
    public async Task SveMetode_CRUDVezeTopKategorije_Rade()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository=new KategorijaRepository(context);
        context.Kategorije.AddRange(new Kategorija{ID=1,Naziv="Vodovod"},new Kategorija{ID=2,Naziv="Elektrika"});
        await context.SaveChangesAsync();

        Assert.AreEqual(2,(await repository.DajSveAsync()).Count());
        Assert.IsNotNull(await repository.DajPoIdAsync(1));
        Assert.IsNull(await repository.DajPoIdAsync(999));

        await repository.DodajKategorijeOglasu(1,[1,2]);
        await repository.DodajKategorijeOglasu(1,[]);
        await repository.AzurirajKategorijeOglasa(1,[2]);
        Assert.AreEqual(1,await context.OglasKategorije.CountAsync());
        await repository.UkloniSveKategorijeOglasa(1);

        await repository.DodajKategorijeIzvrsiocu("i1",[1,1,2]);
        await repository.AzurirajKategorijeIzvrsioca("i1",[2]);
        Assert.AreEqual(1,await context.IzvrsilacKategorija.CountAsync(x=>x.IzvrsilacID=="i1"));
        await repository.UkloniSveKategorijeIzvrsiocu("i1");

        context.IzvrsilacKategorija.AddRange(new IzvrsilacKategorija{IzvrsilacID="a",KategorijaID=1},new IzvrsilacKategorija{IzvrsilacID="b",KategorijaID=1},new IzvrsilacKategorija{IzvrsilacID="c",KategorijaID=2});
        await context.SaveChangesAsync();
        var top=(await repository.DajTopKategorijePoMajstorimaAsync(5)).ToList();
        Assert.AreEqual(2,top.Count);
    }
}
