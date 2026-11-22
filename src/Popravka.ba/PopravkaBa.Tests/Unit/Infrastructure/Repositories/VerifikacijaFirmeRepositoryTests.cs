using Microsoft.EntityFrameworkCore;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class VerifikacijaFirmeRepositoryCoverageTests
{
    [TestMethod]
    public async Task SveMetode_DodajNajnovijuPoFirmiNaCekanjuPoIdISacuvaj_Rade()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new VerifikacijaFirmeRepository(context);
        var firma = FirmaPopunjena("f1");
        var verifikacija = EmailPopunjena(1,"f1",Status.NaCekanju);

        await repository.DodajAsync(verifikacija);
        Assert.IsNotNull(await context.EmailVerifikacijaFirme.FindAsync(1));
        Assert.IsNotNull(await repository.DajZadnjuZaFirmuAsync("f1"));
        Assert.IsNotNull(await context.EmailVerifikacijaFirme.FindAsync(1));
        _ = await repository.DajPoIdAsync(1);

        context.Firme.Add(firma);
        await context.SaveChangesAsync();
        Assert.AreEqual(1,(await repository.DajNaCekanjuAsync()).Count());

        verifikacija.StatusVerifikacije=Status.Prihvaceno;
        await repository.SacuvajAsync();
        Assert.AreEqual(Status.Prihvaceno,(await repository.DajZadnjuZaFirmuAsync("f1"))!.StatusVerifikacije);
        Assert.AreEqual(0,(await repository.DajNaCekanjuAsync()).Count());
    }

    [TestMethod]
    public async Task RubniPodaci_NemaVerifikacijeINemaNaCekanju_RubniPutevi()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var repository = new VerifikacijaFirmeRepository(context);

        Assert.IsNull(await repository.DajZadnjuZaFirmuAsync("nepostoji"));
        Assert.IsNull(await repository.DajPoIdAsync(999));
        Assert.AreEqual(0,(await repository.DajNaCekanjuAsync()).Count());
    }

    private static Firma FirmaPopunjena(string id) => new(){ Id=id, NazivFirme="Firma", Adresa="Sarajevo", Email="firma@test.ba", Kategorije=[] };
    private static EmailVerifikacijaFirme EmailPopunjena(int id,string firmaId,Status status) => new(){
        VerifikacioniID=id,NazivFirme="Firma",FirmaID=firmaId,StatusVerifikacije=status,DatumPodnosenja=DateTime.UtcNow,
        SjedisteFirme="Sarajevo",KontaktTelefon="033",OdgovornaOsobaIme="Ime",OdgovornaOsobaPrezime="Prezime",
        OdgovornaOsobaEmail="ime@test.ba",OdgovornaOsobaPozicija="Direktor",OdgovornaOsobaTelefon="061",
        Rjesenje="rjesenje",PoreznoUvjerenje="porez",Logotip="logo"
    };
}
