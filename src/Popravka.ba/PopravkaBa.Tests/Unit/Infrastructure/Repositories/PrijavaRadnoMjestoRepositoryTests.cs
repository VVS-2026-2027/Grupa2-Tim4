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
    public async Task CRUDIPrethodnePrijave_SveOperacije_Rade()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.AddRange(
            new Majstor { Id = "m1", Adresa = "Sarajevo" },
            new Majstor { Id = "m2", Adresa = "Mostar" },
            new Majstor { Id = "m3", Adresa = "Tuzla" });
        var repository = new PrijavaRadnoMjestoRepository(context);
        var prijava = new PrijavaRadnoMjesto { ID=1, OglasID=10, MajstorID="m1", VrijemePrijave=new DateTime(2026,11,20), StatusPrijave=Status.NaCekanju };

        await repository.DodajAsync(prijava);
        Assert.IsNotNull(await context.PrijaveRadnoMjesto.FindAsync(1));

        prijava.StatusPrijave=Status.Prihvaceno;
        await repository.UrediAsync(prijava);
        Assert.AreEqual(Status.Prihvaceno, (await context.PrijaveRadnoMjesto.FindAsync(1))!.StatusPrijave);

        context.PrijaveRadnoMjesto.AddRange(
            new PrijavaRadnoMjesto { ID=2, OglasID=10, MajstorID="m2", VrijemePrijave=new DateTime(2026,11,21) },
            new PrijavaRadnoMjesto { ID=3, OglasID=11, MajstorID="m3", VrijemePrijave=new DateTime(2026,11,22) });
        await context.SaveChangesAsync();

        _ = await repository.DajPoIdAsync(1);
        var prijaveOglasa = (await repository.DajSvePrijaveOglasaAsync(10)).ToList();
        Assert.AreEqual(2, prijaveOglasa.Count);
        Assert.AreEqual(1, prijaveOglasa[0].ID);
        Assert.AreEqual(2, prijaveOglasa[1].ID);
        Assert.AreEqual(2, await context.PrijaveRadnoMjesto.CountAsync(x => x.OglasID == 10));

        await repository.ObrisiAsync(1);
        await repository.ObrisiAsync(999);
        Assert.IsNull(await context.PrijaveRadnoMjesto.FindAsync(1));
    }
}
