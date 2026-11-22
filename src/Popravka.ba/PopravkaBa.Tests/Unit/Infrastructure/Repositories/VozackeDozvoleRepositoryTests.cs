using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class VozackeDozvoleRepositoryCoverageTests
{
    [TestMethod]
    public async Task DodajVozackeDozvoleOglasu_NullIPraznaLista_VracaPrazanSkup()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new VozackeDozvoleRepository(context);

        await repository.DodajVozackeDozvoleOglasu(1, null);
        await repository.DodajVozackeDozvoleOglasu(1, []);

        Assert.AreEqual(0, (await repository.DajSveVozackeDozvoleOglasa(1)).Count());
    }

    [TestMethod]
    public async Task DodajAzurirajIUkloniVozackeDozvole_DupliranaLista_VracaUnikatnaAzuriranaIDeleted()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new VozackeDozvoleRepository(context);

        await repository.DodajVozackeDozvoleOglasu(1, [VozackaDozvola.B, VozackaDozvola.B, VozackaDozvola.C]);
        CollectionAssert.AreEquivalent(new[] { VozackaDozvola.B, VozackaDozvola.C }, (await repository.DajSveVozackeDozvoleOglasa(1)).ToArray());

        await repository.AzurirajVozackeDozvoleOglasa(1, [VozackaDozvola.CE]);
        CollectionAssert.AreEqual(new[] { VozackaDozvola.CE }, (await repository.DajSveVozackeDozvoleOglasa(1)).ToArray());

        await repository.UkloniSveVozackeDozvoleOglasa(1);
        Assert.AreEqual(0, (await repository.DajSveVozackeDozvoleOglasa(1)).Count());
    }
}
