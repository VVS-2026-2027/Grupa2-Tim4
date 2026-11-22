using PopravkaBa.Application.DTOs;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class IzvrsilacUslugeDtoTests
    {
        [TestMethod]
        public void PretragaIzvrsilacUslugeDto_MozeSeKreirati()
        {
            // Act
            var dto = new PretragaIzvrsilacUslugeDto();

            // Assert
            Assert.IsNotNull(dto);
        }

        [TestMethod]
        public void ProfilMajstorDto_MozeSeKreirati()
        {
            // Act
            var dto = new ProfilMajstorDto();

            // Assert
            Assert.IsNotNull(dto);
        }

        [TestMethod]
        public void ProfilFirmaDto_MozeSeKreirati()
        {
            // Act
            var dto = new ProfilFirmaDto();

            // Assert
            Assert.IsNotNull(dto);
        }
    }
}
