using PopravkaBa.Application.DTOs;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class KreirajPrijavaRadnoMjestoDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 8,
                MajstorID = "majstor-1"
            };

            // Act + Assert
            Assert.AreEqual(8, dto.OglasID);
            Assert.AreEqual("majstor-1", dto.MajstorID);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Arrange
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 1,
                MajstorID = "majstor-1"
            };

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(0, rezultati.Count);
        }

        [TestMethod]
        public void NedostajeMajstor_VracaGresku()
        {
            // Arrange
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 1,
                MajstorID = null!
            };

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(KreirajPrijavaRadnoMjestoDto.MajstorID)));
        }
    }
}
