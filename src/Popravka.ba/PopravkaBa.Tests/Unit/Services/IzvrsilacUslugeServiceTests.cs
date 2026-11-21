using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class IzvrsilacUslugeServiceTests
    {
        private Mock<IIzvrsilacUslugeRepository> _repository = null!;
        private IzvrsilacUslugeService _servis = null!;

        // Pokreće se prije SVAKOG testa -> svaki test dobije nov mock (testovi su nezavisni)
        [TestInitialize]
        public void Postavi()
        {
            _repository = new Mock<IIzvrsilacUslugeRepository>();
            _servis = new IzvrsilacUslugeService(_repository.Object);
        }

        // IzvrsilacUsluge je apstraktna klasa -> objekat pravimo preko Moq-a
        private static IzvrsilacUsluge NoviIzvrsilac() => new Mock<IzvrsilacUsluge>().Object;


        [TestMethod]
        public async Task DajProfilPoIdAsync_PostojeciId_VracaProfilIzRepozitorija()
        {
            var izvrsilac = NoviIzvrsilac();
            _repository.Setup(r => r.DajProfilPoIdAsync("1")).ReturnsAsync(izvrsilac);

           
            var rezultat = await _servis.DajProfilPoIdAsync("1");

            Assert.AreSame(izvrsilac, rezultat);
        }

        [TestMethod]
        public async Task DajProfilPoIdAsync_NepostojeciId_VracaNull()
        {
            // Arrange
            _repository.Setup(r => r.DajProfilPoIdAsync("999")).ReturnsAsync((IzvrsilacUsluge?)null);

            // Act
            var rezultat = await _servis.DajProfilPoIdAsync("999");

            // Assert
            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public async Task DajProfilPoIdAsync_ProslijedjenId_PozivaRepozitorijSaIstimIdJednom()
        {
            // Act
            await _servis.DajProfilPoIdAsync("abc");

            // Assert
            _repository.Verify(r => r.DajProfilPoIdAsync("abc"), Times.Once);
        }


        [TestMethod]
        public async Task UrediAsync_ProslijedjenIzvrsilac_PozivaRepozitorijJednom()
        {
            // Arrange
            var izvrsilac = NoviIzvrsilac();

            // Act
            await _servis.UrediAsync(izvrsilac);

            // Assert
            _repository.Verify(r => r.UrediAsync(izvrsilac), Times.Once);
        }


        [TestMethod]
        public async Task DajSveAsync_RepozitorijImaIzvrsioce_VracaSveIzvrsioce()
        {
            // Arrange
            var izvrsioci = new List<IzvrsilacUsluge> { NoviIzvrsilac(), NoviIzvrsilac(), NoviIzvrsilac() };
            _repository.Setup(r => r.DajSveAsync()).ReturnsAsync(izvrsioci);

            // Act
            var rezultat = await _servis.DajSveAsync();

            // Assert
            CollectionAssert.AreEqual(izvrsioci, rezultat.ToList());
        }

        [TestMethod]
        public async Task DajSveAsync_RepozitorijPrazan_VracaPraznuKolekciju()
        {
            // Arrange
            _repository.Setup(r => r.DajSveAsync()).ReturnsAsync(new List<IzvrsilacUsluge>());

            // Act
            var rezultat = await _servis.DajSveAsync();

            // Assert
            Assert.AreEqual(0, rezultat.Count());
        }
    }
}