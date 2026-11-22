using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class ObjaviOglasMajstoraDtoTests
    {
        private static ObjaviOglasMajstoraDto ValidanDto() => new()
        {
            Naslov = "Majstor za vodoinstalacije",
            Opis = "Popravke u domacinstvu",
            MjestoID = 1,
            MinCijena = 20,
            TipIsplate = TipIsplate.PoSatu,
            KategorijeID = new List<int> { 1 },
            Slika = "https://cdn.example.com/slika.jpg"
        };

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual("Majstor za vodoinstalacije", dto.Naslov);
            Assert.AreEqual("Popravke u domacinstvu", dto.Opis);
            Assert.AreEqual(1, dto.MjestoID);
            Assert.AreEqual(20.0, dto.MinCijena);
            Assert.AreEqual(TipIsplate.PoSatu, dto.TipIsplate);
            CollectionAssert.AreEqual(new List<int> { 1 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/slika.jpg", dto.Slika);
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

        [TestMethod]
        public void SlikaNijeObavezna_NemaGresaka()
        {
            // Arrange
            var dto = ValidanDto();
            dto.Slika = null;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(0, rezultati.Count);
        }

        [DataTestMethod]
        [DataRow(0.0, true)]
        [DataRow(20.0, true)]
        [DataRow(-0.01, false)]
        public void MinCijena_PostujeGranicu(double minCijena, bool ocekivanoValidan)
        {
            // Arrange
            var dto = ValidanDto();
            dto.MinCijena = minCijena;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeNaslov_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.Naslov = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasMajstoraDto.Naslov)));
        }

        [TestMethod]
        public void NedostajeOpis_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.Opis = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasMajstoraDto.Opis)));
        }

        [TestMethod]
        public void PraznaListaKategorija_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.KategorijeID = new List<int>();

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasMajstoraDto.KategorijeID)));
        }
    }

    [TestClass]
    public class UrediOglasMajstoraDtoTests
    {
        private static UrediOglasMajstoraDto ValidanDto() => new()
        {
            OglasID = 7,
            Naslov = "Azuriran oglas",
            Opis = "Azuriran opis oglasa",
            MjestoID = 2,
            MinCijena = 35,
            TipIsplate = TipIsplate.Sedmicno,
            KategorijeID = new List<int> { 2, 3 },
            Slika = "https://cdn.example.com/novi.jpg"
        };

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual(7, dto.OglasID);
            Assert.AreEqual("Azuriran oglas", dto.Naslov);
            Assert.AreEqual("Azuriran opis oglasa", dto.Opis);
            Assert.AreEqual(2, dto.MjestoID);
            Assert.AreEqual(35.0, dto.MinCijena);
            Assert.AreEqual(TipIsplate.Sedmicno, dto.TipIsplate);
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/novi.jpg", dto.Slika);
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

        [TestMethod]
        public void NegativnaMinCijena_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.MinCijena = -5;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasMajstoraDto.MinCijena)));
        }

        [TestMethod]
        public void NedostajeNaslov_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.Naslov = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasMajstoraDto.Naslov)));
        }

        [TestMethod]
        public void NedostajeOpis_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.Opis = null!;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasMajstoraDto.Opis)));
        }

        [TestMethod]
        public void PraznaListaKategorija_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.KategorijeID = new List<int>();

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasMajstoraDto.KategorijeID)));
        }
    }
}
