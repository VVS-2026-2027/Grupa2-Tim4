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
            // Priprema
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 8,
                MajstorID = "majstor-1"
            };

            // Akcija i provjera
            Assert.AreEqual(8, dto.OglasID);
            Assert.AreEqual("majstor-1", dto.MajstorID);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Priprema
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 1,
                MajstorID = "majstor-1"
            };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }

        [TestMethod]
        public void NedostajeMajstor_VracaGresku()
        {
            // Priprema
            var dto = new KreirajPrijavaRadnoMjestoDto
            {
                OglasID = 1,
                MajstorID = null!
            };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(KreirajPrijavaRadnoMjestoDto.MajstorID)));
        }
    }
}
