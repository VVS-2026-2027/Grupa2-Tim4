using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Interfaces.Repositories;
using PopravkaBa.Domain.Models;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class MjestoServiceTests
    {
        private Mock<IMjestoRepository> _repo = null!;
        private MjestoService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IMjestoRepository>();
            _servis = new MjestoService(_repo.Object);
        }

        public static IEnumerable<object[]> PretrageIzXml() =>
            XmlHelper.Ucitaj("pretrage_mjesta.xml", "pretraga", e => new object[]
            {
                (string)e.Attribute("tekst")!
            });

        [TestMethod]
        [DynamicData(nameof(PretrageIzXml), DynamicDataSourceType.Method)]
        public async Task PronadjiMjestaAsync_PretragaIzXml_VracaRezultatRepozitorija(string pretraga)
        {
            var mjesta = new List<Mjesto>();
            _repo.Setup(r => r.PronadjiMjestaAsync(pretraga)).ReturnsAsync(mjesta);

            var rezultat = await _servis.PronadjiMjestaAsync(pretraga);

            Assert.AreSame(mjesta, rezultat);
            _repo.Verify(r => r.PronadjiMjestaAsync(pretraga), Times.Once);
        }

        [TestMethod]
        public async Task DajSvaMjestaAsync_RepozitorijImaMjesta_VracaSvaMjesta()
        {
            var mjesta = new List<Mjesto> { new Mjesto(), new Mjesto() };
            _repo.Setup(r => r.DajSvaMjestaAsync()).ReturnsAsync(mjesta);

            var rezultat = await _servis.DajSvaMjestaAsync();

            CollectionAssert.AreEqual(mjesta, rezultat.ToList());
        }

        [TestMethod]
        public async Task DajMjestaPoKantonuAsync_PostojeciKanton_VracaMjestaKantona()
        {
            var mjesta = new List<Mjesto>();
            _repo.Setup(r => r.DajMjestaPoKantonuAsync(9)).ReturnsAsync(mjesta);

            var rezultat = await _servis.DajMjestaPoKantonuAsync(9);

            Assert.AreSame(mjesta, rezultat);
        }

        [TestMethod]
        public async Task DajMjestaPoKantonuAsync_NepostojeciKanton_VracaNull()
        {
            _repo.Setup(r => r.DajMjestaPoKantonuAsync(99)).ReturnsAsync((IEnumerable<Mjesto>?)null);

            var rezultat = await _servis.DajMjestaPoKantonuAsync(99);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task DajMjestoPoIdAsync_PostojeciId_VracaMjesto()
        {
            var mjesto = new Mjesto();
            _repo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(mjesto);

            var rezultat = await _servis.DajMjestoPoIdAsync(1);

            Assert.AreSame(mjesto, rezultat);
        }

        [TestMethod]
        public async Task DajMjestoPoIdAsync_NepostojeciId_VracaNull()
        {
            _repo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((Mjesto?)null);

            var rezultat = await _servis.DajMjestoPoIdAsync(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task DajSveKorisnikeMjestaAsync_KorisnikId_VracaMjestaKorisnika()
        {
            var mjesta = new List<Mjesto>();
            _repo.Setup(r => r.DajSveKorisnikeMjestaAsync("korisnik-1")).ReturnsAsync(mjesta);

            var rezultat = await _servis.DajSveKorisnikeMjestaAsync("korisnik-1");

            Assert.AreSame(mjesta, rezultat);
        }

        [TestMethod]
        public async Task DodajMjestaKorisniku_ListaMjesta_PozivaRepozitorijJednom()
        {
            var mjesta = new List<int> { 1, 2, 3 };

            await _servis.DodajMjestaKorisniku("korisnik-1", mjesta);

            _repo.Verify(r => r.DodajMjestaKorisniku("korisnik-1", mjesta), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajMjestaKorisnika_NovaMjesta_PozivaRepozitorijSaIstimPodacima()
        {
            var mjesta = new List<int> { 4 };

            await _servis.AzurirajMjestaKorisnika("korisnik-2", mjesta);

            _repo.Verify(r => r.AzurirajMjestaKorisnika("korisnik-2", mjesta), Times.Once);
        }

        [TestMethod]
        public async Task DajOglaseMajstoraPoMjestu_MjestoId_VracaOglaseIzRepozitorija()
        {
            var oglasi = new List<OglasMajstora>();
            _repo.Setup(r => r.DajOglaseMajstoraPoMjestu(3)).ReturnsAsync(oglasi);

            var rezultat = await _servis.DajOglaseMajstoraPoMjestu(3);

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        public async Task DajOglaseRadnogMjestaPoMjestu_MjestoId_VracaOglaseIzRepozitorija()
        {
            var oglasi = new List<OglasRadnoMjesto>();
            _repo.Setup(r => r.DajOglaseRadnogMjestaPoMjestu(3)).ReturnsAsync(oglasi);

            var rezultat = await _servis.DajOglaseRadnogMjestaPoMjestu(3);

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        public async Task DajOglaseUslugePoMjestu_MjestoId_VracaOglaseIzRepozitorija()
        {
            var oglasi = new List<OglasUsluge>();
            _repo.Setup(r => r.DajOglaseUslugePoMjestu(3)).ReturnsAsync(oglasi);

            var rezultat = await _servis.DajOglaseUslugePoMjestu(3);

            Assert.AreSame(oglasi, rezultat);
        }
    }
}