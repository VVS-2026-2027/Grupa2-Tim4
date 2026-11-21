using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class PrijavaOglasServiceTests
    {
        private Mock<IPrijavaRadnoMjestoRepository> _prijavaRepo = null!;
        private Mock<IOglasRadnoMjestoRepository> _oglasRepo = null!;
        private PrijavaOglasService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _prijavaRepo = new Mock<IPrijavaRadnoMjestoRepository>();
            _oglasRepo = new Mock<IOglasRadnoMjestoRepository>();
            _servis = new PrijavaOglasService(_prijavaRepo.Object, _oglasRepo.Object);
        }

        private OglasRadnoMjesto PostaviOglas(int id, Status status, int brojIzvrsilaca = 1, List<PrijavaRadnoMjesto>? prijave = null)
        {
            var oglas = new OglasRadnoMjesto { StatusOglasa = status, BrojIzvrsilaca = brojIzvrsilaca, Prijave = prijave };
            _oglasRepo.Setup(r => r.DajPoIdAsync(id)).ReturnsAsync(oglas);
            return oglas;
        }

        private PrijavaRadnoMjesto PostaviPrijavu(int id, int oglasId, Status status)
        {
            var prijava = new PrijavaRadnoMjesto { ID = id, OglasID = oglasId, StatusPrijave = status };
            _prijavaRepo.Setup(r => r.DajPoIdAsync(id)).ReturnsAsync(prijava);
            return prijava;
        }

        [TestMethod]
        public async Task DajPrijavuPoId_PostojeciId_VracaPrijavu()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);

            var rezultat = await _servis.DajPrijavuPoId(1);

            Assert.AreSame(prijava, rezultat);
        }

        [TestMethod]
        public async Task DajPrijavuPoId_NepostojeciId_VracaNull()
        {
            _prijavaRepo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((PrijavaRadnoMjesto?)null);

            var rezultat = await _servis.DajPrijavuPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task KreirajPrijavu_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _oglasRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasRadnoMjesto?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 99, MajstorID = "majstor-1" }));
        }

        [TestMethod]
        [DataRow(Status.Neaktivan)]
        [DataRow(Status.Isporuceno)]
        public async Task KreirajPrijavu_OglasNijeAktivan_BacaInvalidOperationException(Status status)
        {
            PostaviOglas(5, status);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 5, MajstorID = "majstor-1" }));
        }

        [TestMethod]
        public async Task KreirajPrijavu_MajstorVecPrijavljen_PorukaKazeDaJeVecPrijavljen()
        {
            var postojeca = new PrijavaRadnoMjesto { MajstorID = "majstor-1" };
            PostaviOglas(5, Status.Aktivan, prijave: new List<PrijavaRadnoMjesto> { postojeca });

            var izuzetak = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 5, MajstorID = "majstor-1" }));

            StringAssert.Contains(izuzetak.Message, "Već ste se prijavili");
        }

        [TestMethod]
        public async Task KreirajPrijavu_MajstorVecPrijavljen_NeSpremaNovuPrijavu()
        {
            var postojeca = new PrijavaRadnoMjesto { MajstorID = "majstor-1" };
            PostaviOglas(5, Status.Aktivan, prijave: new List<PrijavaRadnoMjesto> { postojeca });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 5, MajstorID = "majstor-1" }));

            _prijavaRepo.Verify(r => r.DodajAsync(It.IsAny<PrijavaRadnoMjesto>()), Times.Never);
        }

        [TestMethod]
        public async Task KreirajPrijavu_DrugiMajstorPrijavljen_SpremaPrijavuNaCekanju()
        {
            var tudja = new PrijavaRadnoMjesto { MajstorID = "majstor-2" };
            PostaviOglas(5, Status.Aktivan, prijave: new List<PrijavaRadnoMjesto> { tudja });

            await _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 5, MajstorID = "majstor-1" });

            _prijavaRepo.Verify(r => r.DodajAsync(It.Is<PrijavaRadnoMjesto>(p =>
                p.OglasID == 5 &&
                p.MajstorID == "majstor-1" &&
                p.StatusPrijave == Status.NaCekanju)), Times.Once);
        }

        [TestMethod]
        public async Task KreirajPrijavu_OglasBezPrijava_SpremaPrijavu()
        {
            PostaviOglas(5, Status.Aktivan, prijave: null);

            await _servis.KreirajPrijavu(new KreirajPrijavaRadnoMjestoDto { OglasID = 5, MajstorID = "majstor-1" });

            _prijavaRepo.Verify(r => r.DodajAsync(It.IsAny<PrijavaRadnoMjesto>()), Times.Once);
        }


        [TestMethod]
        public async Task PrihvatiPonudu_NepostojecaPrijava_BacaKeyNotFoundException()
        {
            _prijavaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((PrijavaRadnoMjesto?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.PrihvatiPonudu(99));
        }

        [TestMethod]
        public async Task PrihvatiPonudu_PostojecaPrijava_PostavljaStatusPrihvaceno()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);
            PostaviOglas(5, Status.Aktivan, brojIzvrsilaca: 2);

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Prihvaceno, prijava.StatusPrijave);
            _prijavaRepo.Verify(r => r.UrediAsync(prijava), Times.Once);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        [DataRow(5, false)]
        public async Task PrihvatiPonudu_BrojIzvrsilaca_VracaDaLiJeOglasPopunjen(int brojIzvrsilaca, bool ocekivano)
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);
            PostaviOglas(5, Status.Aktivan, brojIzvrsilaca);
            _prijavaRepo.Setup(r => r.DajSvePrijaveOglasaAsync(5)).ReturnsAsync(new List<PrijavaRadnoMjesto> { prijava });

            var rezultat = await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OglasJosImaMjesta_OglasOstajeAktivan()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);
            var oglas = PostaviOglas(5, Status.Aktivan, brojIzvrsilaca: 3);
            _prijavaRepo.Setup(r => r.DajSvePrijaveOglasaAsync(5)).ReturnsAsync(new List<PrijavaRadnoMjesto> { prijava });

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Aktivan, oglas.StatusOglasa);
            _oglasRepo.Verify(r => r.UrediAsync(It.IsAny<OglasRadnoMjesto>()), Times.Never);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OglasPopunjen_OdbijaOstalePrijaveNaCekanju()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);
            var druga = new PrijavaRadnoMjesto { ID = 2, OglasID = 5, StatusPrijave = Status.NaCekanju };
            var vecOdbijena = new PrijavaRadnoMjesto { ID = 3, OglasID = 5, StatusPrijave = Status.Odbijeno };
            PostaviOglas(5, Status.Aktivan, brojIzvrsilaca: 1);
            _prijavaRepo.Setup(r => r.DajSvePrijaveOglasaAsync(5))
                        .ReturnsAsync(new List<PrijavaRadnoMjesto> { prijava, druga, vecOdbijena });

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Odbijeno, druga.StatusPrijave);
            Assert.AreEqual(Status.Prihvaceno, prijava.StatusPrijave);
            _prijavaRepo.Verify(r => r.UrediAsync(vecOdbijena), Times.Never);
        }

        [TestMethod]
        public async Task PrihvatiPonudu_OglasPopunjen_OglasPostajeNeaktivan()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);
            var oglas = PostaviOglas(5, Status.Aktivan, brojIzvrsilaca: 1);
            _prijavaRepo.Setup(r => r.DajSvePrijaveOglasaAsync(5)).ReturnsAsync(new List<PrijavaRadnoMjesto> { prijava });

            await _servis.PrihvatiPonudu(1);

            Assert.AreEqual(Status.Neaktivan, oglas.StatusOglasa);
            _oglasRepo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task OdbijPrijavu_PostojecaPrijava_PostavljaStatusOdbijenoISprema()
        {
            var prijava = PostaviPrijavu(1, 5, Status.NaCekanju);

            await _servis.OdbijPrijavu(1);

            Assert.AreEqual(Status.Odbijeno, prijava.StatusPrijave);
            _prijavaRepo.Verify(r => r.UrediAsync(prijava), Times.Once);
        }

        [TestMethod]
        public async Task OdbijPrijavu_NepostojecaPrijava_BacaKeyNotFoundException()
        {
            _prijavaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((PrijavaRadnoMjesto?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.OdbijPrijavu(99));
        }
    }
}