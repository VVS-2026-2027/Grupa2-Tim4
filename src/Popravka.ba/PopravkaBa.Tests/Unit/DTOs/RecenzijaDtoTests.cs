using PopravkaBa.Application.DTOs;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class KreirajRecenzijuDtoTests
    {
        private static KreirajRecenzijuDto ValidanDto() => new()
        {
            IzvrsilacID = "majstor-1",
            Ocjena = 5,
            Komentar = "Odlicna usluga, sve preporuke."
        };

        [TestMethod]
        public void NoviDto_ImaPrazanKomentarINultuOcjenu()
        {
            // Act
            var dto = new KreirajRecenzijuDto();

            // Assert
            Assert.AreEqual(string.Empty, dto.Komentar);
            Assert.AreEqual(0, dto.Ocjena);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual("majstor-1", dto.IzvrsilacID);
            Assert.AreEqual(5, dto.Ocjena);
            Assert.AreEqual("Odlicna usluga, sve preporuke.", dto.Komentar);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Arrange
            var dto = ValidanDto();

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(0, rezultati.Count);
        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        [DataRow(5, true)]
        [DataRow(6, false)]
        public void Ocjena_PostujeOpseg(int ocjena, bool ocekivanoValidan)
        {
            // Arrange
            var dto = ValidanDto();
            dto.Ocjena = ocjena;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow(9, false)]
        [DataRow(10, true)]
        [DataRow(1000, true)]
        [DataRow(1001, false)]
        public void Komentar_PostujeGraniceDuzine(int duzina, bool ocekivanoValidan)
        {
            // Arrange
            var dto = ValidanDto();
            dto.Komentar = new string('k', duzina);

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeIzvrsilac_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.IzvrsilacID = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(KreirajRecenzijuDto.IzvrsilacID)));
        }
    }

    [TestClass]
    public class PrijaviRecenzijuDtoTests
    {
        private static PrijaviRecenzijuDto ValidanDto() => new()
        {
            RecenzijaID = 9,
            RazlogPrijave = "Recenzija sadrzi uvrede."
        };

        [TestMethod]
        public void NoviDto_ImaPrazanRazlog()
        {
            // Act
            var dto = new PrijaviRecenzijuDto();

            // Assert
            Assert.AreEqual(string.Empty, dto.RazlogPrijave);
            Assert.AreEqual(0, dto.RecenzijaID);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual(9, dto.RecenzijaID);
            Assert.AreEqual("Recenzija sadrzi uvrede.", dto.RazlogPrijave);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Arrange
            var dto = ValidanDto();

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(0, rezultati.Count);
        }

        [DataTestMethod]
        [DataRow(9, false)]
        [DataRow(10, true)]
        [DataRow(500, true)]
        [DataRow(501, false)]
        public void RazlogPrijave_PostujeGraniceDuzine(int duzina, bool ocekivanoValidan)
        {
            // Arrange
            var dto = ValidanDto();
            dto.RazlogPrijave = new string('r', duzina);

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeRazlog_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.RazlogPrijave = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(PrijaviRecenzijuDto.RazlogPrijave)));
        }
    }

    [TestClass]
    public class ObradiPrijavuRecenzijeDtoTests
    {
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PostavljeneVrijednosti_SeVracaju(bool obrisi)
        {
            // Arrange
            var dto = new ObradiPrijavuRecenzijeDto
            {
                RecenzijaID = 11,
                Obrisi = obrisi
            };

            // Act + Assert
            Assert.AreEqual(11, dto.RecenzijaID);
            Assert.AreEqual(obrisi, dto.Obrisi);
        }
    }
}
