using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class UvjetOglasaRepositoryCoverageTests
{
    [TestMethod]
    public async Task DodajUvjeteOglasu_PraznaIPunaLista_VracaPraznuIZatimPuna()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new UvjetOglasaRepository(context);

        await repository.DodajUvjeteOglasu(1, []);
        Assert.AreEqual(0, await context.UvjetiOglasa.CountAsync());

        await repository.DodajUvjeteOglasu(1, ["Prvi", "Drugi"]);

        Assert.AreEqual(2, (await repository.DajSveUvjeteOglasa(1)).Count());
    }

    [TestMethod]
    public async Task DajUvjetPoIdAsync_PostojećiINepostojećiId_VracaOcekivaniRezultat()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.UvjetiOglasa.Add(new PopravkaBa.Domain.Models.UvjetOglasa { ID = 1, TekstUvjeta = "Prvi", OglasID = 1 });
        await context.SaveChangesAsync();
        var repository = new UvjetOglasaRepository(context);

        var postoji = await repository.DajUvjetPoIdAsync(1);
        var nepostoji = await repository.DajUvjetPoIdAsync(999);

        Assert.IsNotNull(postoji);
        Assert.IsNull(nepostoji);
    }

    [TestMethod]
    public async Task AzurirajIUkloniUvjeteOglasa_PromjenaUVjeta_VracaAžuriraniIPrazanSkup()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new UvjetOglasaRepository(context);

        await repository.DodajUvjeteOglasu(1, ["Prvi", "Drugi"]);
        await repository.AzurirajUvjeteOglasa(1, ["Novi"]);

        Assert.AreEqual("Novi", (await repository.DajSveUvjeteOglasa(1)).Single().TekstUvjeta);
        await repository.UkloniSveUvjeteOglasa(1);
        Assert.AreEqual(0, (await repository.DajSveUvjeteOglasa(1)).Count());
    }
}
