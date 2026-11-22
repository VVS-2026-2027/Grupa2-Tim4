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
    public async Task SveImplementiraneMetode_RubniIPostojećiPodaci_Rade()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository=new MjestoRepository(context);

        context.Mjesta.AddRange(new Mjesto{MjestoID=1,Naziv="Sarajevo",Kanton=Kanton.Sarajevski},new Mjesto{MjestoID=2,Naziv="Mostar",Kanton=Kanton.HercegovackoNeretvanski});
        await context.SaveChangesAsync();

        Assert.AreEqual(2,(await repository.DajSvaMjestaAsync()).Count());
        Assert.IsNotNull(await repository.DajPoIdAsync(1));
        Assert.IsNull(await repository.DajPoIdAsync(999));
        Assert.AreEqual(2,(await repository.PronadjiMjestaAsync("test")).Count());

        await repository.DodajMjestaKorisniku("u1",[1,2]);
        Assert.AreEqual(2,(await repository.DajSveKorisnikeMjestaAsync("u1")).Count());
        await repository.AzurirajMjestaKorisnika("u1",[2]);
        Assert.AreEqual(1,(await repository.DajSveKorisnikeMjestaAsync("u1")).Count());
        await repository.UkloniSvaMjestaKorisniku("u1");
        Assert.AreEqual(0,(await repository.DajSveKorisnikeMjestaAsync("u1")).Count());

        Assert.IsNotNull(await repository.DajMjestaPoKantonuAsync(9));
        Assert.AreEqual(0,(await repository.DajOglaseMajstoraPoMjestu(1)).Count());
        Assert.AreEqual(0,(await repository.DajOglaseRadnogMjestaPoMjestu(1)).Count());
        Assert.AreEqual(0,(await repository.DajOglaseUslugePoMjestu(1)).Count());
    }
}
