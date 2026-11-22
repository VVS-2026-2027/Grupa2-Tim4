using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class OglasUslugeServiceTests
    {
        private Mock<IOglasUslugeRepository> _repo = null!;
        private OglasUslugeService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IOglasUslugeRepository>();
            _servis = new OglasUslugeService(_repo.Object);
        }

        [TestMethod]
        public async Task DajOglasPoId_PostojeciId_VracaOglas()
        {
            var oglas = new OglasUsluge();
            _repo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(oglas);

            var rezultat = await _servis.DajOglasPoId(1);

            Assert.AreSame(oglas, rezultat);
        }

        [TestMethod]
        public async Task DajOglasPoId_NepostojeciId_VracaNull()
        {
            _repo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((OglasUsluge?)null);

            var rezultat = await _servis.DajOglasPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task ObjaviOglas_ValidanDto_SpremaAktivanOglasSaVlasnikom()
        {
            var dto = new ObjaviOglasUslugeDto { Naslov = "Popravka veš mašine" };

            await _servis.ObjaviOglas(dto, "klijent-1");

            _repo.Verify(r => r.DodajAsync(It.Is<OglasUsluge>(o =>
                o.Naslov == "Popravka veš mašine" &&
                o.VlasnikOglasaID == "klijent-1" &&
                o.StatusOglasa == Status.Aktivan)), Times.Once);
        }

        [TestMethod]
        public async Task ObjaviOglas_ValidanDto_VracaIdKojiJeDodijelilaBaza()
        {
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasUsluge>()))
                 .Callback<OglasUsluge>(o => o.OglasID = 42)
                 .Returns(Task.CompletedTask);

            var rezultat = await _servis.ObjaviOglas(new ObjaviOglasUslugeDto(), "klijent-1");

            Assert.AreEqual(42, rezultat);
        }

        [TestMethod]
        public async Task ObrisiOglas_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.ObrisiOglas(99));
        }

        [TestMethod]
        public async Task ObrisiOglas_PostojeciOglas_PostavljaStatusNeaktivanISprema()
        {
            var oglas = new OglasUsluge { StatusOglasa = Status.Aktivan };
            _repo.Setup(r => r.DajPoIdAsync(4)).ReturnsAsync(oglas);

            await _servis.ObrisiOglas(4);

            Assert.AreEqual(Status.Neaktivan, oglas.StatusOglasa);
            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task PronadjiOglase_BezLokacije_VracaSveRezultatePretrage()
        {
            var oglasi = new List<OglasUsluge> { new OglasUsluge(), new OglasUsluge() };
            _repo.Setup(r => r.IzvrsiPretraguTekstaAsync("bojler")).ReturnsAsync(oglasi);

            var rezultat = await _servis.PronadjiOglase("bojler", null);

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(2, 1)]
        [DataRow(3, 0)]
        public async Task PronadjiOglase_SaLokacijom_VracaSamoOglaseIzTogMjesta(int lokacija, int ocekivanBroj)
        {
            _repo.Setup(r => r.IzvrsiPretraguTekstaAsync("bojler")).ReturnsAsync(new List<OglasUsluge>
            {
                new OglasUsluge { MjestoID = 1 },
                new OglasUsluge { MjestoID = 2 },
                new OglasUsluge { MjestoID = 1 }
            });

            var rezultat = await _servis.PronadjiOglase("bojler", lokacija);

            Assert.AreEqual(ocekivanBroj, rezultat.Count());
        }

        [TestMethod]
        public async Task UrediOglas_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasUsluge?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.UrediOglas(new UrediOglasUslugeDto { OglasID = 99 }));
        }

        [TestMethod]
        public async Task UrediOglas_PostojeciOglas_AzuriraPoljaISpremaIzmjene()
        {
            var oglas = new OglasUsluge { Naslov = "Stari naslov", Opis = "Stari opis" };
            _repo.Setup(r => r.DajPoIdAsync(5)).ReturnsAsync(oglas);

            await _servis.UrediOglas(new UrediOglasUslugeDto { OglasID = 5, Naslov = "Novi naslov", Opis = "Novi opis" });

            Assert.AreEqual("Novi naslov", oglas.Naslov);
            Assert.AreEqual("Novi opis", oglas.Opis);
            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }
    }
}