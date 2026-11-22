using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Popravka.ba.Data;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Repositories;

namespace PopravkaBa.Tests.Unit.Infrastructure.Repositories;

[TestClass]
public class VerifikacijskiTokenRepositoryCoverageTests
{
    [TestMethod]
    public async Task SveMetode_KreirajNajnovijiPoHashuPonistiKoristenPotvrdi_Rade()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var store=new Mock<IUserStore<ApplicationUser>>();
        var manager=new Mock<UserManager<ApplicationUser>>(store.Object,null!,null!,null!,null!,null!,null!,null!,null!);
        var repository=new VerifikacijskiTokenRepository(context,manager.Object);
        var user=new ApplicationUser{Id="u1",UserName="u1"};
        var token=new VerifikacijskiToken{ID=1,KorisnikID="u1",TokenHash="hash",Tip=TipTokena.PotvrdaEmaila,VrijemeGenerisanja=DateTime.UtcNow,VrijemeIsteka=DateTime.UtcNow.AddHours(1)};

        await repository.KreirajAsync(token);
        Assert.IsNotNull(await repository.DajNajnovijiZaKorisnikaAsync("u1",TipTokena.PotvrdaEmaila));
        Assert.IsNotNull(await repository.DajTokenPoHashuAsync("hash",TipTokena.PotvrdaEmaila));
        Assert.IsNull(await repository.DajTokenPoHashuAsync("nepostoji",TipTokena.PotvrdaEmaila));

        var ponisteni=await repository.PonistiNeiskoristeneAsync("u1",TipTokena.PotvrdaEmaila);
        Assert.AreEqual(1,ponisteni.Count);

        await repository.OznaciKoristenimAsync(token);
        Assert.IsNotNull(token.VrijemeKoristenja);

        var drugi=new VerifikacijskiToken{ID=2,KorisnikID="u1",TokenHash="hash2",Tip=TipTokena.PotvrdaEmaila};
        await repository.PotvrdiAsync(drugi,user);
        Assert.IsTrue(user.EmailConfirmed);
        Assert.IsNotNull(drugi.VrijemeKoristenja);
    }
}
