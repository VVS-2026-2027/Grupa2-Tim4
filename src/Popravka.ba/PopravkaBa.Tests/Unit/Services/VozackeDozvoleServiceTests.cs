using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class VozackeDozvoleServiceTests
    {
        private Mock<IVozackeDozvoleRepository> _repo = null!;
        private VozackeDozvoleService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IVozackeDozvoleRepository>();
            _servis = new VozackeDozvoleService(_repo.Object);
        }

        private static List<VozackaDozvola> SveDozvole() => Enum.GetValues<VozackaDozvola>().ToList();

        [TestMethod]
        public async Task DajSveVozackeDozvoleOglasa_OglasId_VracaDozvoleIzRepozitorija()
        {
            var dozvole = SveDozvole();
            _repo.Setup(r => r.DajSveVozackeDozvoleOglasa(5)).ReturnsAsync(dozvole);

            var rezultat = await _servis.DajSveVozackeDozvoleOglasa(5);

            Assert.AreSame(dozvole, rezultat);
        }

        [TestMethod]
        public async Task DajSveVozackeDozvoleOglasa_OglasBezDozvola_VracaPraznuKolekciju()
        {
            _repo.Setup(r => r.DajSveVozackeDozvoleOglasa(6)).ReturnsAsync(new List<VozackaDozvola>());

            var rezultat = await _servis.DajSveVozackeDozvoleOglasa(6);

            Assert.AreEqual(0, rezultat.Count());
        }

        [TestMethod]
        public async Task DodajVozackeDozvoleOglasu_ListaDozvola_PozivaRepozitorijSaIstimDozvolama()
        {
            var dozvole = SveDozvole();

            await _servis.DodajVozackeDozvoleOglasu(5, dozvole);

            _repo.Verify(r => r.DodajVozackeDozvoleOglasu(5, dozvole), Times.Once);
        }

        [TestMethod]
        public async Task DodajVozackeDozvoleOglasu_NullDozvole_ProsljedjujeNullRepozitoriju()
        {
            await _servis.DodajVozackeDozvoleOglasu(5, null);

            _repo.Verify(r => r.DodajVozackeDozvoleOglasu(5, null), Times.Once);
        }

        [TestMethod]
        public async Task UkloniSveVozackeDozvoleOglasa_OglasId_PozivaRepozitorijJednom()
        {
            await _servis.UkloniSveVozackeDozvoleOglasa(4);

            _repo.Verify(r => r.UkloniSveVozackeDozvoleOglasa(4), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajVozackeDozvoleOglasa_NoveDozvole_PozivaRepozitorijSaIstimPodacima()
        {
            var dozvole = SveDozvole();

            await _servis.AzurirajVozackeDozvoleOglasa(7, dozvole);

            _repo.Verify(r => r.AzurirajVozackeDozvoleOglasa(7, dozvole), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajVozackeDozvoleOglasa_NoveDozvole_NeBriseNiDodajeZasebno()
        {
            await _servis.AzurirajVozackeDozvoleOglasa(7, SveDozvole());

            _repo.Verify(r => r.UkloniSveVozackeDozvoleOglasa(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.DodajVozackeDozvoleOglasu(It.IsAny<int>(), It.IsAny<List<VozackaDozvola>?>()), Times.Never);
        }
    }
}