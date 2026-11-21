using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Interfaces.Repositories;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class UvjetOglasaServiceTests
    {
        private Mock<IUvjetOglasaRepository> _repo = null!;
        private UvjetOglasaService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IUvjetOglasaRepository>();
            _servis = new UvjetOglasaService(_repo.Object);
        }

        [TestMethod]
        public async Task DajSveUvjeteOglasa_OglasId_VracaUvjeteIzRepozitorija()
        {
            var uvjeti = new List<UvjetOglasa?>();
            _repo.Setup(r => r.DajSveUvjeteOglasa(5)).ReturnsAsync(uvjeti);

            var rezultat = await _servis.DajSveUvjeteOglasa(5);

            Assert.AreSame(uvjeti, rezultat);
        }

        [TestMethod]
        public async Task DajUvjetPoIdAsync_PostojeciId_VracaUvjet()
        {
            var uvjet = new UvjetOglasa();
            _repo.Setup(r => r.DajUvjetPoIdAsync(1)).ReturnsAsync(uvjet);

            var rezultat = await _servis.DajUvjetPoIdAsync(1);

            Assert.AreSame(uvjet, rezultat);
        }

        [TestMethod]
        public async Task DajUvjetPoIdAsync_NepostojeciId_VracaNull()
        {
            _repo.Setup(r => r.DajUvjetPoIdAsync(999)).ReturnsAsync((UvjetOglasa?)null);

            var rezultat = await _servis.DajUvjetPoIdAsync(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task DodajUvjeteOglasu_ListaUvjeta_PozivaRepozitorijSaIstimUvjetima()
        {
            var uvjeti = new List<string> { "Vozačka dozvola B", "Minimalno 2 godine iskustva" };

            await _servis.DodajUvjeteOglasu(5, uvjeti);

            _repo.Verify(r => r.DodajUvjeteOglasu(5, uvjeti), Times.Once);
        }

        [TestMethod]
        public async Task DodajUvjeteOglasu_PraznaLista_PozivaRepozitorij()
        {
            var uvjeti = new List<string>();

            await _servis.DodajUvjeteOglasu(5, uvjeti);

            _repo.Verify(r => r.DodajUvjeteOglasu(5, uvjeti), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajUvjeteOglasa_NoviUvjeti_PozivaRepozitorijSaIstimPodacima()
        {
            var uvjeti = new List<string> { "Rad vikendom" };

            await _servis.AzurirajUvjeteOglasa(7, uvjeti);

            _repo.Verify(r => r.AzurirajUvjeteOglasa(7, uvjeti), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajUvjeteOglasa_NoviUvjeti_NeBriseNiDodajeZasebno()
        {
            await _servis.AzurirajUvjeteOglasa(7, new List<string> { "Rad vikendom" });

            _repo.Verify(r => r.UkloniSveUvjeteOglasa(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.DodajUvjeteOglasu(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestMethod]
        public async Task UkloniSveUvjeteOglasa_OglasId_PozivaRepozitorijJednom()
        {
            await _servis.UkloniSveUvjeteOglasa(4);

            _repo.Verify(r => r.UkloniSveUvjeteOglasa(4), Times.Once);
        }
    }
}