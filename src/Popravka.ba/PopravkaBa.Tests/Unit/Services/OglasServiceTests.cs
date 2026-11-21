using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class OglasMajstoraServiceTests
    {
        private Mock<IOglasMajstoraRepository> _repo = null!;
        private OglasMajstoraService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IOglasMajstoraRepository>();
            _servis = new OglasMajstoraService(_repo.Object);
        }

        [TestMethod]
        public async Task DajOglasPoId_PostojeciId_VracaOglas()
        {
            var oglas = new OglasMajstora();
            _repo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(oglas);

            var rezultat = await _servis.DajOglasPoId(1);

            Assert.AreSame(oglas, rezultat);
        }

        [TestMethod]
        public async Task DajOglasPoId_NepostojeciId_VracaNull()
        {
            _repo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((OglasMajstora?)null);

            var rezultat = await _servis.DajOglasPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(10)]
        public async Task ObjaviOglas_TriIliViseAktivnihOglasa_BacaInvalidOperationException(int brojAktivnih)
        {
            _repo.Setup(r => r.DajBrojAktivnihZaKorisnikaAsync("majstor-1")).ReturnsAsync(brojAktivnih);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.ObjaviOglas(new ObjaviOglasMajstoraDto(), "majstor-1"));
        }

        [TestMethod]
        public async Task ObjaviOglas_PrekoracenLimit_PorukaSpominjeMaksimalno3Oglasa()
        {
            _repo.Setup(r => r.DajBrojAktivnihZaKorisnikaAsync("majstor-1")).ReturnsAsync(3);

            var izuzetak = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.ObjaviOglas(new ObjaviOglasMajstoraDto(), "majstor-1"));

            StringAssert.Contains(izuzetak.Message, "maksimalno 3");
        }

        [TestMethod]
        public async Task ObjaviOglas_PrekoracenLimit_NeSpremaOglas()
        {
            _repo.Setup(r => r.DajBrojAktivnihZaKorisnikaAsync("majstor-1")).ReturnsAsync(3);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.ObjaviOglas(new ObjaviOglasMajstoraDto(), "majstor-1"));

            _repo.Verify(r => r.DodajAsync(It.IsAny<OglasMajstora>()), Times.Never);
        }

        [TestMethod]
        public async Task ObjaviOglas_DvaAktivnaOglasa_SpremaOglasSaPodacimaIzDto()
        {
            var dto = new ObjaviOglasMajstoraDto { Naslov = "Popravka bojlera", Opis = "Brzo i povoljno" };
            _repo.Setup(r => r.DajBrojAktivnihZaKorisnikaAsync("majstor-1")).ReturnsAsync(2);

            await _servis.ObjaviOglas(dto, "majstor-1");

            _repo.Verify(r => r.DodajAsync(It.Is<OglasMajstora>(o =>
                o.Naslov == "Popravka bojlera" &&
                o.Opis == "Brzo i povoljno" &&
                o.VlasnikOglasaID == "majstor-1")), Times.Once);
        }

        [TestMethod]
        public async Task ObjaviOglas_ValidanOglas_VracaIdKojiJeDodijelilaBaza()
        {
            _repo.Setup(r => r.DajBrojAktivnihZaKorisnikaAsync("majstor-1")).ReturnsAsync(0);
            _repo.Setup(r => r.DodajAsync(It.IsAny<OglasMajstora>()))
                 .Callback<OglasMajstora>(o => o.OglasID = 42)
                 .Returns(Task.CompletedTask);

            var rezultat = await _servis.ObjaviOglas(new ObjaviOglasMajstoraDto(), "majstor-1");

            Assert.AreEqual(42, rezultat);
        }

        [TestMethod]
        public async Task UrediOglas_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasMajstora?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.UrediOglas(new UrediOglasMajstoraDto { OglasID = 99 }));
        }

        [TestMethod]
        public async Task UrediOglas_PostojeciOglas_AzuriraPoljaISpremaIzmjene()
        {
            var oglas = new OglasMajstora { Naslov = "Stari naslov", Opis = "Stari opis" };
            var dto = new UrediOglasMajstoraDto { OglasID = 5, Naslov = "Novi naslov", Opis = "Novi opis" };
            _repo.Setup(r => r.DajPoIdAsync(5)).ReturnsAsync(oglas);

            await _servis.UrediOglas(dto);

            Assert.AreEqual("Novi naslov", oglas.Naslov);
            Assert.AreEqual("Novi opis", oglas.Opis);
            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task ObrisiOglas_PostojeciOglas_PostavljaStatusNeaktivan()
        {
            var oglas = new OglasMajstora();
            _repo.Setup(r => r.DajPoIdAsync(4)).ReturnsAsync(oglas);

            await _servis.ObrisiOglas(4);

            Assert.AreEqual(Status.Neaktivan, oglas.StatusOglasa);
        }

        [TestMethod]
        public async Task ObrisiOglas_PostojeciOglas_SpremaIzmjenuUmjestoBrisanja()
        {
            var oglas = new OglasMajstora();
            _repo.Setup(r => r.DajPoIdAsync(4)).ReturnsAsync(oglas);

            await _servis.ObrisiOglas(4);

            _repo.Verify(r => r.UrediAsync(oglas), Times.Once);
        }

        [TestMethod]
        public async Task ObrisiOglas_NepostojeciOglas_BacaKeyNotFoundException()
        {
            _repo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((OglasMajstora?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.ObrisiOglas(99));
        }
    }
}