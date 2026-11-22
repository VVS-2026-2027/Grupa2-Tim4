using PopravkaBa.Application.DTOs;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class IzvrsilacUslugeDtoTests
    {
        [TestMethod]
        public void PretragaIzvrsilacUslugeDto_MozeSeKreirati()
        {
            // Akcija
            var dto = new PretragaIzvrsilacUslugeDto();

            // Provjera
            Assert.IsNotNull(dto);
        }

        [TestMethod]
        public void ProfilMajstorDto_MozeSeKreirati()
        {
            // Akcija
            var dto = new ProfilMajstorDto();

            // Provjera
            Assert.IsNotNull(dto);
        }

        [TestMethod]
        public void ProfilFirmaDto_MozeSeKreirati()
        {
            // Akcija
            var dto = new ProfilFirmaDto();

            // Provjera
            Assert.IsNotNull(dto);
        }
    }
}
