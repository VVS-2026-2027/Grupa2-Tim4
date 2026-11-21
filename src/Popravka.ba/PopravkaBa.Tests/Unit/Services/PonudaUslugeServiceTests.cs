using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class PonudaUslugeServiceTests
    {
        private Mock<IPonudaUslugeRepository> _ponudaRepo = null!;
        private Mock<IOglasUslugeRepository> _oglasRepo = null!;
        private Mock<IIzvrsilacUslugeRepository> _izvrsilacRepo = null!;
        private PonudaUslugeService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _ponudaRepo = new Mock<IPonudaUslugeRepository>();
            _oglasRepo = new Mock<IOglasUslugeRepository>();
            _izvrsilacRepo = new Mock<IIzvrsilacUslugeRepository>();
            _servis = new PonudaUslugeService(_ponudaRepo.Object, _oglasRepo.Object, _izvrsilacRepo.Object);
        }

        private OglasUsluge PostaviOglas(int id, Status status, string vlasnikId = "klijent-1")
        {
            var oglas = new OglasUsluge { StatusOglasa = status, VlasnikOglasaID = vlasnikId };
            _oglasRepo.Setup(r => r.DajPoIdAsync(id)).ReturnsAsync(oglas);
            return oglas;
        }

        private PonudaUsluge PostaviPonudu(int id, int oglasId, Status status, string izvrsilacId = "izvrsilac-1")
        {
            var ponuda = new PonudaUsluge { ID = id, OglasUslugeID = oglasId, StatusPonude = status, IzvrsilacID = izvrsilacId };
            _ponudaRepo.Setup(r => r.DajPoIdAsync(id)).ReturnsAsync(ponuda);
            return ponuda;
        }

        [TestMethod]
        public async Task DajPonuduPoId_PostojeciId_VracaPonudu()
        {
            var ponuda = PostaviPonudu(1, 5, Status.NaCekanju);

            var rezultat = await _servis.DajPonuduPoId(1);

            Assert.AreSame(ponuda, rezultat);
        }

        [TestMethod]
        public async Task DajPonuduPoId_NepostojeciId_VracaNull()
        {
            _ponudaRepo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((PonudaUsluge?)null);

            var rezultat = await _servis.DajPonuduPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task DajSvePonudeOglasa_OglasId_VracaPonudeIzRepozitorija()
        {
            var ponude = new List<PonudaUsluge>();
            _ponudaRepo.Setup(r => r.DajSvePonudeOglasaAsync(5)).ReturnsAsync(ponude);

            var rezultat = await _servis.DajSvePonudeOglasa(5);

            Assert.AreSame(ponude, rezultat);
        }

        [TestMethod]
        public async Task ObrisiPonudu_PonudaId_PozivaRepozitorijJednom()
        {
            await _servis.ObrisiPonudu(3);

            _ponudaRepo.Verify(r => r.ObrisiAsync(3), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiPonudu_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _oglasRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.PosaljiPonudu(new KreirajPonudaUslugeDto { OglasUslugeID = 99 }, "izvrsilac-1"));
        }

        [TestMethod]
        [DataRow(Status.Neaktivan)]
        [DataRow(Status.Isporuceno)]
        public async Task PosaljiPonudu_OglasNijeAktivan_BacaInvalidOperationException(Status status)
        {
            PostaviOglas(5, status);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.PosaljiPonudu(new KreirajPonudaUslugeDto { OglasUslugeID = 5 }, "izvrsilac-1"));
        }

        [TestMethod]
        public async Task PosaljiPonudu_OglasNijeAktivan_NeSpremaPonudu()
        {
            PostaviOglas(5, Status.Neaktivan);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.PosaljiPonudu(new KreirajPonudaUslugeDto { OglasUslugeID = 5 }, "izvrsilac-1"));

            _ponudaRepo.Verify(r => r.DodajAsync(It.IsAny<PonudaUsluge>()), Times.Never);
        }

        [TestMethod]
        public async Task PosaljiPonudu_AktivanOglas_SpremaPonuduNaCekanju()
        {
            PostaviOglas(5, Status.Aktivan);

            await _servis.PosaljiPonudu(new KreirajPonudaUslugeDto { OglasUslugeID = 5 }, "izvrsilac-1");

            _ponudaRepo.Verify(r => r.DodajAsync(It.Is<PonudaUsluge>(p =>
                p.OglasUslugeID == 5 &&
                p.IzvrsilacID == "izvrsilac-1" &&
                p.StatusPonude == Status.NaCekanju)), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiPonudu_DatumIzForme_SpremaSeKaoUtc()
        {
            PostaviOglas(5, Status.Aktivan);
            PonudaUsluge? spremljena = null;
            _ponudaRepo.Setup(r => r.DodajAsync(It.IsAny<PonudaUsluge>()))
                       .Callback<PonudaUsluge>(p => spremljena = p)
                       .Returns(Task.CompletedTask);
            var dto = new KreirajPonudaUslugeDto
            {
                OglasUslugeID = 5,
                DatumPocetkaUsluge = new DateTime(2026, 12, 1, 9, 0, 0, DateTimeKind.Unspecified)
            };

            await _servis.PosaljiPonudu(dto, "izvrsilac-1");

            Assert.AreEqual(DateTimeKind.Utc, spremljena!.DatumPocetkaUsluge.Kind);
        }

        [TestMethod]
        public async Task PosaljiPonudu_BezDatumaZavrsetka_DatumZavrsetkaJeNull()
        {
            PostaviOglas(5, Status.Aktivan);
            PonudaUsluge? spremljena = null;
            _ponudaRepo.Setup(r => r.DodajAsync(It.IsAny<PonudaUsluge>()))
                       .Callback<PonudaUsluge>(p => spremljena = p)
                       .Returns(Task.CompletedTask);

            await _servis.PosaljiPonudu(new KreirajPonudaUslugeDto { OglasUslugeID = 5, DatumOcekivanogZavrsetka = null }, "izvrsilac-1");

            Assert.IsNull(spremljena!.DatumOcekivanogZavrsetka);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_NepostojecaPonuda_BacaKeyNotFoundException()
        {
            _ponudaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((PonudaUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.PrihvatiPonudu(99));
        }

        [TestMethod]
        public async Task PrihvatiPonudu_PostojecaPonuda_PostavljaStatusPrihvaceno()
        {
            var ponuda = PostaviPonudu(1, 5, Status.NaCekanju);

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Prihvaceno, ponuda.StatusPonude);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OstalePonudeNaCekanju_PostajuOdbijene()
        {
            var prihvacena = PostaviPonudu(1, 5, Status.NaCekanju);
            var druga = new PonudaUsluge { ID = 2, OglasUslugeID = 5, StatusPonude = Status.NaCekanju };
            var vecOdbijena = new PonudaUsluge { ID = 3, OglasUslugeID = 5, StatusPonude = Status.Odbijeno };
            _ponudaRepo.Setup(r => r.DajSvePonudeOglasaAsync(5))
                       .ReturnsAsync(new List<PonudaUsluge> { prihvacena, druga, vecOdbijena });

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Odbijeno, druga.StatusPonude);
            _ponudaRepo.Verify(r => r.UrediAsync(vecOdbijena), Times.Never);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OglasPostoji_OglasPostajeNeaktivan()
        {
            PostaviPonudu(1, 5, Status.NaCekanju);
            var oglas = PostaviOglas(5, Status.Aktivan);

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Neaktivan, oglas.StatusOglasa);
            _oglasRepo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OglasNePostoji_NeUredjujeOglas()
        {
            PostaviPonudu(1, 5, Status.NaCekanju);
            _oglasRepo.Setup(r => r.DajPoIdAsync(5)).ReturnsAsync((OglasUsluge?)null);

            await _servis.PrihvatiPonudu(1);

            _oglasRepo.Verify(r => r.UrediAsync(It.IsAny<OglasUsluge>()), Times.Never);
        }


        [TestMethod]
        public async Task OdbijPonudu_PostojecaPonuda_PostavljaStatusOdbijenoISprema()
        {
            var ponuda = PostaviPonudu(1, 5, Status.NaCekanju);

            await _servis.OdbijPonudu(1);

            Assert.AreEqual(Status.Odbijeno, ponuda.StatusPonude);
            _ponudaRepo.Verify(r => r.UrediAsync(ponuda), Times.Once);
        }

        [TestMethod]
        public async Task OdbijPonudu_NepostojecaPonuda_BacaKeyNotFoundException()
        {
            _ponudaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((PonudaUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.OdbijPonudu(99));
        }


        [TestMethod]
        public async Task ZavrsiPosao_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _oglasRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.ZavrsiPosao(99, "klijent-1"));
        }

        [TestMethod]
        public async Task ZavrsiPosao_KorisnikNijeVlasnik_BacaUnauthorizedAccessException()
        {
            PostaviOglas(5, Status.Neaktivan, vlasnikId: "klijent-1");

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _servis.ZavrsiPosao(5, "neko-drugi"));
        }

        [TestMethod]
        public async Task ZavrsiPosao_NemaPrihvacenePonude_BacaInvalidOperationException()
        {
            PostaviOglas(5, Status.Neaktivan);
            _ponudaRepo.Setup(r => r.DajSvePonudeOglasaAsync(5))
                       .ReturnsAsync(new List<PonudaUsluge> { new PonudaUsluge { StatusPonude = Status.Odbijeno } });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _servis.ZavrsiPosao(5, "klijent-1"));
        }

        [TestMethod]
        public async Task ZavrsiPosao_PrihvacenaPonuda_PonudaIOglasPostajuIsporuceni()
        {
            var oglas = PostaviOglas(5, Status.Neaktivan);
            var prihvacena = new PonudaUsluge { StatusPonude = Status.Prihvaceno, IzvrsilacID = "izvrsilac-7" };
            _ponudaRepo.Setup(r => r.DajSvePonudeOglasaAsync(5)).ReturnsAsync(new List<PonudaUsluge> { prihvacena });

            await _servis.ZavrsiPosao(5, "klijent-1");

            Assert.AreEqual(Status.Isporuceno, prihvacena.StatusPonude);
            Assert.AreEqual(Status.Isporuceno, oglas.StatusOglasa);
        }

        [TestMethod]
        public async Task ZavrsiPosao_PrihvacenaPonuda_PovecavaBrojZavrsenihPoslovaIzvrsioca()
        {
            PostaviOglas(5, Status.Neaktivan);
            var prihvacena = new PonudaUsluge { StatusPonude = Status.Prihvaceno, IzvrsilacID = "izvrsilac-7" };
            _ponudaRepo.Setup(r => r.DajSvePonudeOglasaAsync(5)).ReturnsAsync(new List<PonudaUsluge> { prihvacena });

            await _servis.ZavrsiPosao(5, "klijent-1");

            _izvrsilacRepo.Verify(r => r.PovecajBrojZavrsenihPoslovaAsync("izvrsilac-7"), Times.Once);
        }
    }
}