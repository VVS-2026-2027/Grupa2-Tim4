using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class OglasRadnoMjestoServiceTests
    {
        private Mock<IOglasRadnoMjestoRepository> _repo = null!;
        private OglasRadnoMjestoService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IOglasRadnoMjestoRepository>();
            _servis = new OglasRadnoMjestoService(_repo.Object);
        }

        private OglasRadnoMjesto? UhvatiSpremljeniOglas(Action<OglasRadnoMjesto> uhvati)
        {
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasRadnoMjesto>()))
                 .Callback(uhvati)
                 .Returns(Task.CompletedTask);
            return null;
        }

        [TestMethod]
        public async Task DajOglasPoId_PostojeciId_VracaOglas()
        {
            var oglas = new OglasRadnoMjesto();
            _repo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(oglas);

            var rezultat = await _servis.DajOglasPoId(1);

            Assert.AreSame(oglas, rezultat);
        }

        [TestMethod]
        public async Task DajOglasPoId_NepostojeciId_VracaNull()
        {
            _repo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((OglasRadnoMjesto?)null);

            var rezultat = await _servis.DajOglasPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task ObjaviOglas_ValidanDto_SpremaOglasSaPodacimaIVlasnikom()
        {
            var dto = new ObjaviOglasRadnoMjestoDto { Naslov = "Vozač kamiona", Opis = "Puno radno vrijeme" };

            await _servis.ObjaviOglas(dto, "poslodavac-1");

            _repo.Verify(r => r.DodajAsync(It.Is<OglasRadnoMjesto>(o =>
                o.Naslov == "Vozač kamiona" &&
                o.Opis == "Puno radno vrijeme" &&
                o.VlasnikOglasaID == "poslodavac-1")), Times.Once);
        }

        [TestMethod]
        public async Task ObjaviOglas_BezPrihoda_PrihodiSuNula()
        {
            OglasRadnoMjesto? spremljen = null;
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasRadnoMjesto>()))
                 .Callback<OglasRadnoMjesto>(o => spremljen = o)
                 .Returns(Task.CompletedTask);

            await _servis.ObjaviOglas(new ObjaviOglasRadnoMjestoDto { MinPrihod = null, MaxPrihod = null }, "poslodavac-1");

            Assert.AreEqual(0m, Convert.ToDecimal(spremljen!.MinPrihod));
            Assert.AreEqual(0m, Convert.ToDecimal(spremljen.MaxPrihod));
        }

        [TestMethod]
        public async Task ObjaviOglas_ZadanPrihod_PrihodSePrepisuje()
        {
            OglasRadnoMjesto? spremljen = null;
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasRadnoMjesto>()))
                 .Callback<OglasRadnoMjesto>(o => spremljen = o)
                 .Returns(Task.CompletedTask);

            await _servis.ObjaviOglas(new ObjaviOglasRadnoMjestoDto { MinPrihod = 1500, MaxPrihod = 2500 }, "poslodavac-1");

            Assert.AreEqual(1500m, Convert.ToDecimal(spremljen!.MinPrihod));
            Assert.AreEqual(2500m, Convert.ToDecimal(spremljen.MaxPrihod));
        }

        [TestMethod]
        public async Task ObjaviOglas_ValidanDto_VracaIdKojiJeDodijelilaBaza()
        {
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasRadnoMjesto>()))
                 .Callback<OglasRadnoMjesto>(o => o.OglasID = 42)
                 .Returns(Task.CompletedTask);

            var rezultat = await _servis.ObjaviOglas(new ObjaviOglasRadnoMjestoDto(), "poslodavac-1");

            Assert.AreEqual(42, rezultat);
        }

        [TestMethod]
        public async Task ObrisiOglas_PostojeciOglas_PostavljaStatusNeaktivanISprema()
        {
            var oglas = new OglasRadnoMjesto { StatusOglasa = Status.Aktivan };
            _repo.Setup(r => r.DajPoIdAsync(4)).ReturnsAsync(oglas);

            await _servis.ObrisiOglas(4);

            Assert.AreEqual(Status.Neaktivan, oglas.StatusOglasa);
            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task PronadjiOglase_BezLokacije_VracaSveRezultatePretrage()
        {
            var oglasi = new List<OglasRadnoMjesto> { new OglasRadnoMjesto(), new OglasRadnoMjesto() };
            _repo.Setup(r => r.IzvrsiPretraguTekstaAsync("vozač")).ReturnsAsync(oglasi);

            var rezultat = await _servis.PronadjiOglase("vozač", null);

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(2, 1)]
        [DataRow(3, 0)]
        public async Task PronadjiOglase_SaLokacijom_VracaSamoOglaseIzTogMjesta(int lokacija, int ocekivanBroj)
        {
            _repo.Setup(r => r.IzvrsiPretraguTekstaAsync("vozač")).ReturnsAsync(new List<OglasRadnoMjesto>
            {
                new OglasRadnoMjesto { MjestoID = 1 },
                new OglasRadnoMjesto { MjestoID = 2 },
                new OglasRadnoMjesto { MjestoID = 1 }
            });

            var rezultat = await _servis.PronadjiOglase("vozač", lokacija);

            Assert.AreEqual(ocekivanBroj, rezultat.Count());
        }

        [TestMethod]
        public async Task UrediOglas_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasRadnoMjesto?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.UrediOglas(new UrediOglasRadnoMjestoDto { OglasID = 99 }));
        }

        [TestMethod]
        public async Task UrediOglas_PostojeciOglas_AzuriraPoljaISpremaIzmjene()
        {
            var oglas = new OglasRadnoMjesto { Naslov = "Stari naslov" };
            _repo.Setup(r => r.DajPoIdAsync(5)).ReturnsAsync(oglas);

            await _servis.UrediOglas(new UrediOglasRadnoMjestoDto { OglasID = 5, Naslov = "Novi naslov", MinPrihod = null });

            Assert.AreEqual("Novi naslov", oglas.Naslov);
            Assert.AreEqual(0m, Convert.ToDecimal(oglas.MinPrihod));
            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }
    }
}