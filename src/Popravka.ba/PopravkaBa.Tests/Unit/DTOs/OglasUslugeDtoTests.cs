using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class OglasUslugeDtoTests
    {
        [TestMethod]
        public void NoviDto_KategorijeSuPraznaLista()
        {
            // Act
            var dto = new OglasUslugeDto();

            // Assert
            Assert.IsNotNull(dto.Kategorije);
            Assert.AreEqual(0, dto.Kategorije.Count);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var datum = new DateTime(2026, 11, 20, 10, 30, 0);
            var dto = new OglasUslugeDto
            {
                Id = 12,
                Naslov = "Popravka ves masine",
                Opis = "Masina ne centrifuge",
                DatumObjave = datum,
                StatusOglasa = Status.Aktivan,
                Lokacija = "Sarajevo",
                VlasnikId = "korisnik-1",
                VlasnikIme = "Amina Hadzic",
                BrojPonuda = 3,
                Kategorije = new List<string> { "Bijela tehnika", "Vodovod" },
                MinBudzet = 50,
                MaxBudzet = 200
            };

            // Act + Assert
            Assert.AreEqual(12, dto.Id);
            Assert.AreEqual("Popravka ves masine", dto.Naslov);
            Assert.AreEqual("Masina ne centrifuge", dto.Opis);
            Assert.AreEqual(datum, dto.DatumObjave);
            Assert.AreEqual(Status.Aktivan, dto.StatusOglasa);
            Assert.AreEqual("Sarajevo", dto.Lokacija);
            Assert.AreEqual("korisnik-1", dto.VlasnikId);
            Assert.AreEqual("Amina Hadzic", dto.VlasnikIme);
            Assert.AreEqual(3, dto.BrojPonuda);
            CollectionAssert.AreEqual(new List<string> { "Bijela tehnika", "Vodovod" }, dto.Kategorije);
            Assert.AreEqual(50, dto.MinBudzet);
            Assert.AreEqual(200, dto.MaxBudzet);
        }
    }

    [TestClass]
    public class ObjaviOglasUslugeDtoTests
    {
        private static ObjaviOglasUslugeDto ValidanDto() => new()
        {
            Naslov = "Potrebna stolar usluga",
            Opis = "Izrada plakara po mjeri",
            MjestoID = 1,
            MinBudzet = 100,
            MaxBudzet = 500,
            KategorijeID = new List<int> { 1 },
            Slika = "https://cdn.example.com/oglas.jpg"
        };

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual("Potrebna stolar usluga", dto.Naslov);
            Assert.AreEqual("Izrada plakara po mjeri", dto.Opis);
            Assert.AreEqual(1, dto.MjestoID);
            Assert.AreEqual(100, dto.MinBudzet);
            Assert.AreEqual(500, dto.MaxBudzet);
            CollectionAssert.AreEqual(new List<int> { 1 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/oglas.jpg", dto.Slika);
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
        [DataRow(-1, false)]
        [DataRow(0, true)]
        [DataRow(10, true)]
        public void Budzet_PostujeGranicu(int budzet, bool ocekivanoValidan)
        {
            // Arrange
            var dto = ValidanDto();
            dto.MinBudzet = budzet;
            dto.MaxBudzet = budzet;

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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasUslugeDto.Naslov)));
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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasUslugeDto.Opis)));
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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasUslugeDto.KategorijeID)));
        }
    }

    [TestClass]
    public class UrediOglasUslugeDtoTests
    {
        private static UrediOglasUslugeDto ValidanDto() => new()
        {
            OglasID = 4,
            Naslov = "Azuriran oglas usluge",
            Opis = "Azuriran opis usluge",
            MjestoID = 2,
            MinBudzet = 50,
            MaxBudzet = 300,
            KategorijeID = new List<int> { 2 },
            Slika = "https://cdn.example.com/uredi.jpg"
        };

        [TestMethod]
        public void NoviDto_StringoviSuPrazni()
        {
            // Act
            var dto = new UrediOglasUslugeDto();

            // Assert
            Assert.AreEqual(string.Empty, dto.Naslov);
            Assert.AreEqual(string.Empty, dto.Opis);
            Assert.IsNull(dto.Slika);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = ValidanDto();

            // Act + Assert
            Assert.AreEqual(4, dto.OglasID);
            Assert.AreEqual("Azuriran oglas usluge", dto.Naslov);
            Assert.AreEqual("Azuriran opis usluge", dto.Opis);
            Assert.AreEqual(2, dto.MjestoID);
            Assert.AreEqual(50, dto.MinBudzet);
            Assert.AreEqual(300, dto.MaxBudzet);
            CollectionAssert.AreEqual(new List<int> { 2 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/uredi.jpg", dto.Slika);
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
        public void NevalidnoMjesto_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.MjestoID = 0;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasUslugeDto.MjestoID)));
        }

        [TestMethod]
        public void MinimalniBudzetNegativan_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.MinBudzet = -1;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasUslugeDto.MinBudzet)));
        }

        [TestMethod]
        public void MaksimalniBudzetNula_VracaGresku()
        {
            // Arrange
            var dto = ValidanDto();
            dto.MaxBudzet = 0;

            // Act
            var rezultati = DtoValidacija.Validiraj(dto);

            // Assert
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasUslugeDto.MaxBudzet)));
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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasUslugeDto.KategorijeID)));
        }
    }
}
