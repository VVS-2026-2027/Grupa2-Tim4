using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services;
using PopravkaBa.Application.Services.Interface;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class OglasMajstoraFacadeTests
    {
        private Mock<IOglasMajstoraService> _oglasService = null!;
        private Mock<IKategorijaService> _kategorijaService = null!;
        private OglasMajstoraFacade _facade = null!;

        [TestInitialize]
        public void Postavi()
        {
            _oglasService = new Mock<IOglasMajstoraService>();
            _kategorijaService = new Mock<IKategorijaService>();
            _facade = new OglasMajstoraFacade(_oglasService.Object, _kategorijaService.Object);
        }

        [TestMethod]
        public async Task DajSveOglase_ServisImaOglase_VracaOglaseIzServisa()
        {
            var oglasi = new List<OglasMajstora>();
            _oglasService.Setup(s => s.DajSveOglase()).ReturnsAsync(oglasi);

            var rezultat = await _facade.DajSveOglase();

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        public async Task DajOglasPoId_PostojeciId_VracaOglas()
        {
            var oglas = new OglasMajstora();
            _oglasService.Setup(s => s.DajOglasPoId(1)).ReturnsAsync(oglas);

            var rezultat = await _facade.DajOglasPoId(1);

            Assert.AreSame(oglas, rezultat);
        }

        [TestMethod]
        public async Task DajOglasPoId_NepostojeciId_VracaNull()
        {
            _oglasService.Setup(s => s.DajOglasPoId(999)).ReturnsAsync((OglasMajstora?)null);

            var rezultat = await _facade.DajOglasPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task ObjaviOglas_SaKategorijama_VracaIdNovogOglasa()
        {
            var dto = new ObjaviOglasMajstoraDto { KategorijeID = new List<int> { 1, 2 } };
            _oglasService.Setup(s => s.ObjaviOglas(dto, "majstor-1")).ReturnsAsync(15);

            var rezultat = await _facade.ObjaviOglas(dto, "majstor-1");

            Assert.AreEqual(15, rezultat);
        }

        [TestMethod]
        public async Task ObjaviOglas_SaKategorijama_DodajeKategorijeOglasuIIzvrsiocu()
        {
            var kategorije = new List<int> { 1, 2 };
            var dto = new ObjaviOglasMajstoraDto { KategorijeID = kategorije };
            _oglasService.Setup(s => s.ObjaviOglas(dto, "majstor-1")).ReturnsAsync(15);

            await _facade.ObjaviOglas(dto, "majstor-1");

            _kategorijaService.Verify(k => k.DodajKategorijeOglasu(15, kategorije), Times.Once);
            _kategorijaService.Verify(k => k.DodajKategorijeIzvrsiocu("majstor-1", kategorije), Times.Once);
        }

        [TestMethod]
        public async Task ObjaviOglas_BezKategorija_NeDodajeKategorijeIzvrsiocu()
        {
            var dto = new ObjaviOglasMajstoraDto { KategorijeID = new List<int>() };
            _oglasService.Setup(s => s.ObjaviOglas(dto, "majstor-1")).ReturnsAsync(15);

            await _facade.ObjaviOglas(dto, "majstor-1");

            _kategorijaService.Verify(k => k.DodajKategorijeIzvrsiocu(It.IsAny<string>(), It.IsAny<List<int>>()), Times.Never);
        }

        [TestMethod]
        public async Task UrediOglas_ValidanDto_UredjujeOglasIAzuriraKategorije()
        {
            var kategorije = new List<int> { 1 };
            var dto = new UrediOglasMajstoraDto { OglasID = 7, KategorijeID = kategorije };

            await _facade.UrediOglas(dto);

            _oglasService.Verify(s => s.UrediOglas(dto), Times.Once);
            _kategorijaService.Verify(k => k.AzurirajKategorijeOglasa(7, kategorije), Times.Once);
        }

        [TestMethod]
        public async Task UrediOglas_PostojeciOglasSaKategorijama_DodajeKategorijeVlasnikuOglasa()
        {
            var kategorije = new List<int> { 3 };
            var dto = new UrediOglasMajstoraDto { OglasID = 7, KategorijeID = kategorije };
            _oglasService.Setup(s => s.DajOglasPoId(7)).ReturnsAsync(new OglasMajstora { VlasnikOglasaID = "majstor-1" });

            await _facade.UrediOglas(dto);

            _kategorijaService.Verify(k => k.DodajKategorijeIzvrsiocu("majstor-1", kategorije), Times.Once);
        }

        [TestMethod]
        public async Task UrediOglas_NepostojeciOglas_NeDodajeKategorijeIzvrsiocu()
        {
            var dto = new UrediOglasMajstoraDto { OglasID = 99, KategorijeID = new List<int> { 3 } };
            _oglasService.Setup(s => s.DajOglasPoId(99)).ReturnsAsync((OglasMajstora?)null);

            await _facade.UrediOglas(dto);

            _kategorijaService.Verify(k => k.DodajKategorijeIzvrsiocu(It.IsAny<string>(), It.IsAny<List<int>>()), Times.Never);
        }

        [TestMethod]
        public async Task ObrisiOglas_OglasId_PozivaServisJednom()
        {
            await _facade.ObrisiOglas(4);

            _oglasService.Verify(s => s.ObrisiOglas(4), Times.Once);
        }
    }
}