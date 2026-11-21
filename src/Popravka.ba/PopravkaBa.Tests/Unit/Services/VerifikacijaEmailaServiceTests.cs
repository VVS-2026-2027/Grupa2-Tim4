using Microsoft.AspNetCore.Identity;
using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class VerifikacijaEmailaServiceTests
    {
        private const TipTokena Tip = TipTokena.ResetLozinke;

        private Mock<IVerifikacijskiTokenRepository> _tokenRepo = null!;
        private Mock<UserManager<ApplicationUser>> _userManager = null!;
        private VerifikacijaEmailaService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _tokenRepo = new Mock<IVerifikacijskiTokenRepository>();
            _userManager = MockHelper.KreirajUserManager();
            _servis = new VerifikacijaEmailaService(_tokenRepo.Object, _userManager.Object);
        }

        private static ApplicationUser NoviKorisnik() => new ApplicationUser { Id = "korisnik-1" };

        private VerifikacijskiToken PostaviToken(DateTime? iskoristen = null, int istieZaMinuta = 10)
        {
            var token = new VerifikacijskiToken
            {
                KorisnikID = "korisnik-1",
                VrijemeKoristenja = iskoristen,
                VrijemeIsteka = DateTime.UtcNow.AddMinutes(istieZaMinuta)
            };
            _tokenRepo.Setup(r => r.DajTokenPoHashuAsync(It.IsAny<string>(), It.IsAny<TipTokena>())).ReturnsAsync(token);
            return token;
        }

        private ApplicationUser PostaviKorisnikaUBazi()
        {
            var korisnik = NoviKorisnik();
            _userManager.Setup(u => u.FindByIdAsync("korisnik-1")).ReturnsAsync(korisnik);
            return korisnik;
        }

        private VerifikacijskiToken? UhvatiKreiraniToken()
        {
            VerifikacijskiToken? kreiran = null;
            _tokenRepo.Setup(r => r.KreirajAsync(It.IsAny<VerifikacijskiToken>()))
                      .Callback<VerifikacijskiToken>(t => kreiran = t)
                      .Returns(Task.CompletedTask);
            return kreiran;
        }

        [TestMethod]
        public async Task GenerisiTokenAsync_PrviZahtjev_VracaToken()
        {
            var rezultat = await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            Assert.IsFalse(string.IsNullOrEmpty(rezultat));
        }

        [TestMethod]
        public async Task GenerisiTokenAsync_PrviZahtjev_UBazuSpremaHashUmjestoSirovogTokena()
        {
            VerifikacijskiToken? kreiran = null;
            _tokenRepo.Setup(r => r.KreirajAsync(It.IsAny<VerifikacijskiToken>()))
                      .Callback<VerifikacijskiToken>(t => kreiran = t)
                      .Returns(Task.CompletedTask);

            var sirovi = await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            Assert.AreNotEqual(sirovi, kreiran!.TokenHash);
            Assert.AreEqual(64, kreiran.TokenHash.Length);
        }

        [TestMethod]
        public async Task GenerisiTokenAsync_PrviZahtjev_TokenVazi15Minuta()
        {
            VerifikacijskiToken? kreiran = null;
            _tokenRepo.Setup(r => r.KreirajAsync(It.IsAny<VerifikacijskiToken>()))
                      .Callback<VerifikacijskiToken>(t => kreiran = t)
                      .Returns(Task.CompletedTask);

            await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            Assert.AreEqual(TimeSpan.FromMinutes(15), kreiran!.VrijemeIsteka - kreiran.VrijemeGenerisanja);
        }

        [TestMethod]
        public async Task GenerisiTokenAsync_PrviZahtjev_PonistavaStareNeiskoristeneTokene()
        {
            await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            _tokenRepo.Verify(r => r.PonistiNeiskoristeneAsync("korisnik-1", Tip), Times.Once);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(3, true)]
        [DataRow(5, false)]
        [DataRow(10, false)]
        public async Task GenerisiTokenAsync_ZadnjiTokenPrijeXMinuta_PostujeCooldownOd4Minute(int prijeMinuta, bool ocekujeNull)
        {
            var zadnji = new VerifikacijskiToken { VrijemeGenerisanja = DateTime.UtcNow.AddMinutes(-prijeMinuta) };
            _tokenRepo.Setup(r => r.DajNajnovijiZaKorisnikaAsync("korisnik-1", Tip)).ReturnsAsync(zadnji);

            var rezultat = await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            Assert.AreEqual(ocekujeNull, rezultat is null);
        }

        [TestMethod]
        public async Task GenerisiTokenAsync_UnutarCooldowna_NeKreiraNoviToken()
        {
            var zadnji = new VerifikacijskiToken { VrijemeGenerisanja = DateTime.UtcNow.AddMinutes(-1) };
            _tokenRepo.Setup(r => r.DajNajnovijiZaKorisnikaAsync("korisnik-1", Tip)).ReturnsAsync(zadnji);

            await _servis.GenerisiTokenAsync(NoviKorisnik(), Tip);

            _tokenRepo.Verify(r => r.KreirajAsync(It.IsAny<VerifikacijskiToken>()), Times.Never);
        }

        [TestMethod]
        public async Task PotvrdiAsync_SiroviToken_TraziTokenPoSha256Hashu()
        {
            await _servis.PotvrdiAsync("abc", Tip);

            _tokenRepo.Verify(r => r.DajTokenPoHashuAsync(
                "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", Tip), Times.Once);
        }

        [TestMethod]
        public async Task PotvrdiAsync_NepostojeciToken_VracaOdbijeno()
        {
            _tokenRepo.Setup(r => r.DajTokenPoHashuAsync(It.IsAny<string>(), Tip)).ReturnsAsync((VerifikacijskiToken?)null);

            var status = await _servis.PotvrdiAsync("nepostojeci", Tip);

            Assert.AreEqual(Status.Odbijeno, status);
        }

        [TestMethod]
        public async Task PotvrdiAsync_VecIskoristenToken_VracaVecIskoristen()
        {
            PostaviToken(iskoristen: DateTime.UtcNow.AddMinutes(-2));

            var status = await _servis.PotvrdiAsync("token", Tip);

            Assert.AreEqual(Status.VecIskoristen, status);
        }

        [TestMethod]
        public async Task PotvrdiAsync_IstekaoToken_VracaNeaktivan()
        {
            PostaviToken(istieZaMinuta: -1);

            var status = await _servis.PotvrdiAsync("token", Tip);

            Assert.AreEqual(Status.Neaktivan, status);
        }

        [TestMethod]
        public async Task PotvrdiAsync_KorisnikNePostoji_VracaOdbijeno()
        {
            PostaviToken();
            _userManager.Setup(u => u.FindByIdAsync("korisnik-1")).ReturnsAsync((ApplicationUser?)null);

            var status = await _servis.PotvrdiAsync("token", Tip);

            Assert.AreEqual(Status.Odbijeno, status);
        }

        [TestMethod]
        public async Task PotvrdiAsync_ValidanToken_VracaAktivanIPotvrdjujeKorisnika()
        {
            var token = PostaviToken();
            var korisnik = PostaviKorisnikaUBazi();

            var status = await _servis.PotvrdiAsync("token", Tip);

            Assert.AreEqual(Status.Aktivan, status);
            _tokenRepo.Verify(r => r.PotvrdiAsync(token, korisnik), Times.Once);
        }

        [TestMethod]
        public async Task PotvrdiAsync_IstekaoToken_NePotvrdjujeKorisnika()
        {
            PostaviToken(istieZaMinuta: -1);

            await _servis.PotvrdiAsync("token", Tip);

            _tokenRepo.Verify(r => r.PotvrdiAsync(It.IsAny<VerifikacijskiToken>(), It.IsAny<ApplicationUser>()), Times.Never);
        }

        [TestMethod]
        public async Task ValidirajResetTokenAsync_BiloKojiToken_TraziTipResetLozinke()
        {
            await _servis.ValidirajResetTokenAsync("token");

            _tokenRepo.Verify(r => r.DajTokenPoHashuAsync(It.IsAny<string>(), TipTokena.ResetLozinke), Times.Once);
        }

        [TestMethod]
        public async Task ValidirajResetTokenAsync_NepostojeciToken_VracaOdbijenoBezPodataka()
        {
            _tokenRepo.Setup(r => r.DajTokenPoHashuAsync(It.IsAny<string>(), Tip)).ReturnsAsync((VerifikacijskiToken?)null);

            var (status, token, korisnik) = await _servis.ValidirajResetTokenAsync("nepostojeci");

            Assert.AreEqual(Status.Odbijeno, status);
            Assert.IsNull(token);
            Assert.IsNull(korisnik);
        }

        [TestMethod]
        public async Task ValidirajResetTokenAsync_VecIskoristenToken_VracaVecIskoristen()
        {
            PostaviToken(iskoristen: DateTime.UtcNow.AddMinutes(-2));

            var (status, _, _) = await _servis.ValidirajResetTokenAsync("token");

            Assert.AreEqual(Status.VecIskoristen, status);
        }

        [TestMethod]
        public async Task ValidirajResetTokenAsync_IstekaoToken_VracaNeaktivan()
        {
            PostaviToken(istieZaMinuta: -1);

            var (status, _, _) = await _servis.ValidirajResetTokenAsync("token");

            Assert.AreEqual(Status.Neaktivan, status);
        }

        [TestMethod]
        public async Task ValidirajResetTokenAsync_KorisnikNePostoji_VracaOdbijeno()
        {
            PostaviToken();
            _userManager.Setup(u => u.FindByIdAsync("korisnik-1")).ReturnsAsync((ApplicationUser?)null);

            var (status, _, _) = await _servis.ValidirajResetTokenAsync("token");

            Assert.AreEqual(Status.Odbijeno, status);
        }
    }
}