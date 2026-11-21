using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Application.Services.Interface;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class VerifikacijaFirmeServiceTests
    {
        private Mock<IVerifikacijaFirmeRepository> _repo = null!;
        private Mock<IFileStorage> _storage = null!;
        private VerifikacijaFirmeService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IVerifikacijaFirmeRepository>();
            _storage = new Mock<IFileStorage>();
            _servis = new VerifikacijaFirmeService(_repo.Object, _storage.Object);
        }

        private EmailVerifikacijaFirme PostaviZahtjevNaCekanju(int id, Firma? firma = null, string logotip = "", string licenca = "")
        {
            var zahtjev = new EmailVerifikacijaFirme
            {
                NazivFirme = "Popravka d.o.o.",
                OdgovornaOsobaIme = "Amar",
                OdgovornaOsobaPrezime = "Hodžić",
                KontaktTelefon = "061111222",
                Logotip = logotip,
                LicencaDjelatnosti = licenca,
                Firma = firma
            };
            zahtjev.PodnesiVerifikaciju();
            _repo.Setup(r => r.DajPoIdAsync(id)).ReturnsAsync(zahtjev);
            return zahtjev;
        }

        [TestMethod]
        public async Task DajZadnjiZahtjevAsync_FirmaImaZahtjev_VracaZadnjiZahtjev()
        {
            var zahtjev = new EmailVerifikacijaFirme();
            _repo.Setup(r => r.DajZadnjuZaFirmuAsync("firma-1")).ReturnsAsync(zahtjev);

            var rezultat = await _servis.DajZadnjiZahtjevAsync("firma-1");

            Assert.AreSame(zahtjev, rezultat);
        }

        [TestMethod]
        public async Task DajZadnjiZahtjevAsync_FirmaNemaZahtjev_VracaNull()
        {
            _repo.Setup(r => r.DajZadnjuZaFirmuAsync("firma-2")).ReturnsAsync((EmailVerifikacijaFirme?)null);

            var rezultat = await _servis.DajZadnjiZahtjevAsync("firma-2");

            Assert.IsNull(rezultat);
        }


        [TestMethod]
        public async Task PodnesiZahtjevAsync_ValidanDto_PrenosiPodatkeFirmeUZahtjev()
        {
            var dto = new PodnesiVerifikacijuDto { NazivFirme = "Popravka d.o.o.", JIB = "4200000000001" };

            var rezultat = await _servis.PodnesiZahtjevAsync(dto, "firma-1", "admin@popravka.ba");

            Assert.AreEqual("firma-1", rezultat.FirmaID);
            Assert.AreEqual("admin@popravka.ba", rezultat.AdminEmail);
            Assert.AreEqual("Popravka d.o.o.", rezultat.NazivFirme);
            Assert.AreEqual("4200000000001", rezultat.JIB);
        }

        [TestMethod]
        public async Task PodnesiZahtjevAsync_BezLogotipa_LogotipJePrazanString()
        {
            var dto = new PodnesiVerifikacijuDto { Logotip = null };

            var rezultat = await _servis.PodnesiZahtjevAsync(dto, "firma-1", "admin@popravka.ba");

            Assert.AreEqual(string.Empty, rezultat.Logotip);
        }

        [TestMethod]
        public async Task PodnesiZahtjevAsync_ValidanDto_ZahtjevJeNaCekanju()
        {
            var rezultat = await _servis.PodnesiZahtjevAsync(new PodnesiVerifikacijuDto(), "firma-1", "admin@popravka.ba");

            Assert.AreEqual(Status.NaCekanju, rezultat.StatusVerifikacije);
        }

        [TestMethod]
        public async Task PodnesiZahtjevAsync_ValidanDto_SpremaZahtjevUBazu()
        {
            var rezultat = await _servis.PodnesiZahtjevAsync(new PodnesiVerifikacijuDto(), "firma-1", "admin@popravka.ba");

            _repo.Verify(r => r.DodajAsync(rezultat), Times.Once);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_NepostojeciZahtjev_VracaNull()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((EmailVerifikacijaFirme?)null);

            var rezultat = await _servis.ObradiZahtjevAsync(99, odobri: true);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ObradiZahtjevAsync_ZahtjevVecObradjen_VracaNullINeSprema(bool prethodnaOdluka)
        {
            var zahtjev = PostaviZahtjevNaCekanju(1);
            zahtjev.ObradiVerifikaciju(prethodnaOdluka);

            var rezultat = await _servis.ObradiZahtjevAsync(1, odobri: true);

            Assert.IsNull(rezultat);
            _repo.Verify(r => r.SacuvajAsync(), Times.Never);
        }


        [TestMethod]
        public async Task ObradiZahtjevAsync_Odobren_PrenosiPodatkeNaFirmu()
        {
            var firma = new Firma();
            PostaviZahtjevNaCekanju(1, firma);

            await _servis.ObradiZahtjevAsync(1, odobri: true);

            Assert.AreEqual("Amar", firma.Ime);
            Assert.AreEqual("Hodžić", firma.Prezime);
            Assert.AreEqual("Popravka d.o.o.", firma.NazivFirme);
            Assert.AreEqual("061111222", firma.PhoneNumber);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_Odobren_FirmaJeVerificirana()
        {
            var firma = new Firma();
            PostaviZahtjevNaCekanju(1, firma);

            await _servis.ObradiZahtjevAsync(1, odobri: true);

            Assert.IsTrue(firma.AdminVerificirao);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_Odobren_NeBriseDokumente()
        {
            PostaviZahtjevNaCekanju(1, new Firma(), logotip: "logo.png", licenca: "licenca.pdf");

            await _servis.ObradiZahtjevAsync(1, odobri: true);

            _storage.Verify(s => s.ObrisiPublic(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _storage.Verify(s => s.ObrisiPrivate(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_BezFirme_SpremaIVracaZahtjev()
        {
            var zahtjev = PostaviZahtjevNaCekanju(1, firma: null);

            var rezultat = await _servis.ObradiZahtjevAsync(1, odobri: true);

            Assert.AreSame(zahtjev, rezultat);
            _repo.Verify(r => r.SacuvajAsync(), Times.Once);
        }


        [TestMethod]
        public async Task ObradiZahtjevAsync_Odbijen_FirmaNijeVerificirana()
        {
            var firma = new Firma();
            PostaviZahtjevNaCekanju(1, firma);

            await _servis.ObradiZahtjevAsync(1, odobri: false);

            Assert.IsFalse(firma.AdminVerificirao);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_Odbijen_BriseLogotipILicencu()
        {
            PostaviZahtjevNaCekanju(1, logotip: "logo.png", licenca: "licenca.pdf");

            await _servis.ObradiZahtjevAsync(1, odobri: false);

            _storage.Verify(s => s.ObrisiPublic("logo.png", It.IsAny<CancellationToken>()), Times.Once);
            _storage.Verify(s => s.ObrisiPrivate("licenca.pdf", It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ObradiZahtjevAsync_OdbijenBezDokumenata_NistaNeBrise()
        {
            PostaviZahtjevNaCekanju(1);

            await _servis.ObradiZahtjevAsync(1, odobri: false);

            _storage.Verify(s => s.ObrisiPublic(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _storage.Verify(s => s.ObrisiPrivate(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}