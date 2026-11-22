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
    public async Task SveMetode_CRUDIzvrsilacKlijentPrijavljena_Rade()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new RecenzijaRepository(context);
        var recenzija = new Recenzija { RecenzijaID=1, KlijentID="k1", IzvrsilacID="i1", Ocjena=5, Komentar="Odlicno", DatumRecenzije=new DateTime(2026,11,20), Prijavljena=true, DatumPrijave=DateTime.UtcNow, StatusPrijave=Status.NaCekanju };

        await repository.DodajAsync(recenzija);
        Assert.IsTrue(await repository.PostojiAsync("k1","i1"));
        Assert.IsNotNull(await context.Recenzije.FindAsync(1));
        _ = await repository.DajPoIdAsync(1);
        _ = await repository.DajSveAsync();
        _ = await repository.DajPrijavljeneAsync();
        _ = await repository.DajRecenzijeIzvrsiocaAsync("i1");
        _ = await repository.DajRecenzijeKlijentaAsync("k1");

        recenzija.Ocjena=4;
        await repository.SacuvajAsync(recenzija);
        Assert.AreEqual(4, (await context.Recenzije.FindAsync(1))!.Ocjena);

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);
        Assert.IsFalse(await repository.PostojiAsync("k1","i1"));
    }
}
