using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class OglasServiceDajNedavneTests
    {
        private Mock<IOglasRepository> _repo = null!;
        private OglasService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IOglasRepository>();
            _servis = new OglasService(_repo.Object);
        }

        [TestMethod]
        public async Task DajNedavne_TopN_VracaOglaseIzRepozitorija()
        {
            var oglasi = new List<Oglas>();
            _repo.Setup(r => r.DajNedavneAsync(5)).ReturnsAsync(oglasi);

            var rezultat = await _servis.DajNedavne(5);

            Assert.AreSame(oglasi, rezultat);
        }

        [TestMethod]
        public async Task DajNedavne_NemaOglasa_VracaNull()
        {
            _repo.Setup(r => r.DajNedavneAsync(5)).ReturnsAsync((IEnumerable<Oglas>?)null);

            var rezultat = await _servis.DajNedavne(5);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        public async Task DajNedavne_TopN_PozivaRepozitorijSaIstimBrojem(int topN)
        {
            await _servis.DajNedavne(topN);

            _repo.Verify(r => r.DajNedavneAsync(topN), Times.Once);
        }
    }
}