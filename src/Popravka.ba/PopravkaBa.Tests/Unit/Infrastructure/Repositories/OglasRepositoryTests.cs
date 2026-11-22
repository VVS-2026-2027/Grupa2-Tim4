using Microsoft.EntityFrameworkCore;
using Moq;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Interfaces.Repositories;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class OglasRepositoryCoverageTests
{
    [TestMethod]
    public async Task OglasMajstoraRepository_SveMetode_Rade()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository=new OglasMajstoraRepository(context);
        var oglas=new OglasMajstora{OglasID=1,Naslov="Popravka",Opis="Opis",MjestoID=1,VlasnikOglasaID="n1",StatusOglasa=Status.Aktivan};

        await repository.DodajAsync(oglas);
        Assert.IsNotNull(await context.OglasiMajstora.FindAsync(1));
        _ = await repository.DajPoIdAsync(1);
        _ = await repository.DajSveAsync();
        _ = await repository.DajNedavneAsync(5);
        _ = await repository.IzvrsiPretraguTekstaAsync("Popravka");
        Assert.AreEqual(1,await repository.DajBrojAktivnihZaKorisnikaAsync("n1"));

        oglas.Naslov="Nova popravka";await repository.UrediAsync(oglas);
        Assert.AreEqual("Nova popravka",(await context.OglasiMajstora.FindAsync(1))!.Naslov);
        await repository.ObrisiAsync(1);await repository.ObrisiAsync(999);
        Assert.IsNull(await context.OglasiMajstora.FindAsync(1));
    }

    [TestMethod]
    public async Task OglasUslugeRepository_SveMetode_Rade()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository=new OglasUslugeRepository(context);
        var oglas=new OglasUsluge{OglasID=2,Naslov="Usluga",Opis="Opis usluge",MjestoID=1,VlasnikOglasaID="n1",StatusOglasa=Status.Aktivan,Ponude=[]};

        await repository.DodajAsync(oglas);
        Assert.IsNotNull(await context.OglasiUsluga.FindAsync(2));
        _ = await repository.DajPoIdAsync(2);
        _ = await repository.DajSveAsync();
        _ = await repository.DajNedavneAsync(5);
        _ = await repository.IzvrsiPretraguTekstaAsync("Usluga");
        Assert.AreEqual(0,await repository.DajBrojZavrsenih());

        oglas.Naslov="Nova usluga";await repository.UrediAsync(oglas);
        Assert.AreEqual("Nova usluga",(await context.OglasiUsluga.FindAsync(2))!.Naslov);
        await repository.ObrisiAsync(2);await repository.ObrisiAsync(999);
        Assert.IsNull(await context.OglasiUsluga.FindAsync(2));
    }

    [TestMethod]
    public async Task OglasRadnoMjestoRepository_SveMetode_Rade()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository=new OglasRadnoMjestoRepository(context);
        var oglas=new OglasRadnoMjesto{OglasID=3,Naslov="Posao",Opis="Opis posla",MjestoID=1,VlasnikOglasaID="n1",StatusOglasa=Status.Aktivan,VozackeDozvole=[],Uvjeti=[],Prijave=[]};

        await repository.DodajAsync(oglas);
        Assert.IsNotNull(await context.OglasiRadnogMjesta.FindAsync(3));
        _ = await repository.DajPoIdAsync(3);
        _ = await repository.DajSveAsync();
        _ = await repository.DajNedavneAsync(5);
        _ = await repository.IzvrsiPretraguTekstaAsync("Posao");

        oglas.Naslov="Novi posao";await repository.UrediAsync(oglas);
        Assert.AreEqual("Novi posao",(await context.OglasiRadnogMjesta.FindAsync(3))!.Naslov);
        await repository.ObrisiAsync(3);await repository.ObrisiAsync(999);
        Assert.IsNull(await context.OglasiRadnogMjesta.FindAsync(3));
    }

    [TestMethod]
    public async Task OglasUslugeRepository_PronadjiAsync_FiltriraSortiraIPaginira()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.Add(new ApplicationUser { Id = "n1" });
        context.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo" });
        context.OglasiUsluga.AddRange(
            new OglasUsluge { OglasID = 10, Naslov = "Prvi", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "n1", DatumObjave = new DateTime(2026, 11, 20) },
            new OglasUsluge { OglasID = 11, Naslov = "Drugi", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "n1", DatumObjave = new DateTime(2026, 11, 21) });
        await context.SaveChangesAsync();
        var repository = new OglasUslugeRepository(context);
        var descending = new Mock<ISpecification<OglasUsluge>>();
        descending.Setup(specification => specification.ToExpression()).Returns(oglas => true);
        descending.SetupGet(specification => specification.OrderByDescending).Returns(oglas => oglas.DatumObjave);
        var ascending = new Mock<ISpecification<OglasUsluge>>();
        ascending.Setup(specification => specification.ToExpression()).Returns(oglas => true);
        ascending.SetupGet(specification => specification.OrderBy).Returns(oglas => oglas.DatumObjave);

        var newestFirst = await repository.PronadjiAsync(descending.Object, 1, 1);
        var oldestFirst = await repository.PronadjiAsync(ascending.Object, 1, 1);
        var unsorted = await repository.PronadjiAsync(new AllSpecification<OglasUsluge>(), 1, 10);

        Assert.AreEqual(2, newestFirst.Ukupno);
        Assert.AreEqual(11, newestFirst.Stavke.Single().OglasID);
        Assert.AreEqual(10, oldestFirst.Stavke.Single().OglasID);
        Assert.AreEqual(2, unsorted.Stavke.Count());
    }

    [TestMethod]
    public async Task OglasRadnoMjestoRepository_PronadjiAsync_FiltriraSortiraIPaginira()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.Add(new ApplicationUser { Id = "n1" });
        context.Mjesta.Add(new Mjesto { MjestoID = 1, Naziv = "Sarajevo" });
        context.OglasiRadnogMjesta.AddRange(
            new OglasRadnoMjesto { OglasID = 20, Naslov = "Prvi posao", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "n1", DatumObjave = new DateTime(2026, 11, 20) },
            new OglasRadnoMjesto { OglasID = 21, Naslov = "Drugi posao", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "n1", DatumObjave = new DateTime(2026, 11, 21) });
        await context.SaveChangesAsync();
        var repository = new OglasRadnoMjestoRepository(context);
        var descending = new Mock<ISpecification<OglasRadnoMjesto>>();
        descending.Setup(specification => specification.ToExpression()).Returns(oglas => true);
        descending.SetupGet(specification => specification.OrderByDescending).Returns(oglas => oglas.DatumObjave);
        var ascending = new Mock<ISpecification<OglasRadnoMjesto>>();
        ascending.Setup(specification => specification.ToExpression()).Returns(oglas => true);
        ascending.SetupGet(specification => specification.OrderBy).Returns(oglas => oglas.DatumObjave);

        var newestFirst = await repository.PronadjiAsync(descending.Object, 1, 1);
        var oldestFirst = await repository.PronadjiAsync(ascending.Object, 1, 1);
        var unsorted = await repository.PronadjiAsync(new AllSpecification<OglasRadnoMjesto>(), 1, 10);

        Assert.AreEqual(2, newestFirst.Ukupno);
        Assert.AreEqual(21, newestFirst.Stavke.Single().OglasID);
        Assert.AreEqual(20, oldestFirst.Stavke.Single().OglasID);
        Assert.AreEqual(2, unsorted.Stavke.Count());
    }

    [TestMethod]
    public async Task OglasMajstoraRepository_DodajIUredi_SinhronizujuNajmanjuAktivnuCijenu()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.ApplicationUsers.Add(new Majstor { Id = "majstor", Adresa = "Sarajevo" });
        await context.SaveChangesAsync();
        var repository = new OglasMajstoraRepository(context);
        var skupljiOglas = new OglasMajstora { OglasID = 30, Naslov = "Skuplji", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "majstor", StatusOglasa = Status.Aktivan, MinCijena = 10 };
        var jeftinijiOglas = new OglasMajstora { OglasID = 31, Naslov = "Jeftiniji", Opis = "Opis", MjestoID = 1, VlasnikOglasaID = "majstor", StatusOglasa = Status.Aktivan, MinCijena = 5 };

        await repository.DodajAsync(skupljiOglas);
        await repository.DodajAsync(jeftinijiOglas);
        Assert.AreEqual(5, (await context.ApplicationUsers.OfType<Majstor>().SingleAsync()).MinCijenaUsluge);

        jeftinijiOglas.MinCijena = 0;
        await repository.UrediAsync(jeftinijiOglas);
        Assert.AreEqual(10, (await context.ApplicationUsers.OfType<Majstor>().SingleAsync()).MinCijenaUsluge);

        skupljiOglas.StatusOglasa = Status.Isporuceno;
        await repository.UrediAsync(skupljiOglas);
        Assert.IsNull((await context.ApplicationUsers.OfType<Majstor>().SingleAsync()).MinCijenaUsluge);
    }

    [TestMethod]
    public async Task AgregiraniOglasRepository_SpojiSortiraILimitira()
    {
        await using var context=new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var usluge=new Mock<IOglasUslugeRepository>();
        usluge.Setup(r=>r.DajNedavneAsync(5)).ReturnsAsync([new OglasUsluge{OglasID=1,DatumObjave=new DateTime(2026,11,20)}]);
        var radno=new Mock<IOglasRadnoMjestoRepository>();
        radno.Setup(r=>r.DajNedavneAsync(5)).ReturnsAsync([new OglasRadnoMjesto{OglasID=2,DatumObjave=new DateTime(2026,11,21)}]);
        var repository=new OglasRepository(context,usluge.Object,radno.Object);

        var rezultat=(await repository.DajNedavneAsync(5))!.ToList();
        Assert.AreEqual(2,rezultat.Count);Assert.AreEqual(2,rezultat[0].OglasID);
    }
}
