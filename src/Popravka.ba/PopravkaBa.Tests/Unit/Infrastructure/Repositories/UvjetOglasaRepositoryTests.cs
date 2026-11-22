using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class UvjetOglasaRepositoryCoverageTests
{
    [TestMethod]
    public async Task DodajDajAzurirajUkloni_SviPutevi_Rade()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new UvjetOglasaRepository(context);

        await repository.DodajUvjeteOglasu(1,[]);
        Assert.AreEqual(0, await context.UvjetiOglasa.CountAsync());

        await repository.DodajUvjeteOglasu(1,["Prvi","Drugi"]);
        Assert.AreEqual(2, (await repository.DajSveUvjeteOglasa(1)).Count());

        var uvjet = (await repository.DajSveUvjeteOglasa(1)).First();
        Assert.IsNotNull(await repository.DajUvjetPoIdAsync(uvjet!.ID));
        Assert.IsNull(await repository.DajUvjetPoIdAsync(999));

        await repository.AzurirajUvjeteOglasa(1,["Novi"]);
        Assert.AreEqual("Novi", (await repository.DajSveUvjeteOglasa(1)).Single()!.TekstUvjeta);

        await repository.UkloniSveUvjeteOglasa(1);
        Assert.AreEqual(0, (await repository.DajSveUvjeteOglasa(1)).Count());
    }
}
