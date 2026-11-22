using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Records;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class StatistikaDtoTests
    {
        [TestMethod]
        public void NoviDto_RedoviSuPrazni()
        {
            // Act
            var dto = new StatistikaDTO();

            // Assert
            Assert.IsNotNull(dto.Redovi);
            Assert.AreEqual(0, dto.Redovi.Count);
            Assert.IsNull(dto.MetaPodaci);
            Assert.IsNull(dto.Filter);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var zadnjeAzuriranje = new DateTime(2026, 11, 21, 12, 0, 0);
            var meta = new StatistikaMetaRecord(
                zadnjeAzuriranje,
                new List<StatistikaRecord> { new(1, "Vodovod") },
                new List<StatistikaRecord> { new(1, "Sarajevo") });
            var filter = new StatistikaFilterDTO { KategorijaId = 1, Mjesec = 11 };
            var redovi = new List<StatistikaRedDto>
            {
                new() { Rang = 1, IzvrsilacID = "majstor-1", DisplayName = "Milan Jovanovic" }
            };

            var dto = new StatistikaDTO
            {
                MetaPodaci = meta,
                Redovi = redovi,
                Filter = filter
            };

            // Act + Assert
            Assert.AreEqual(meta, dto.MetaPodaci);
            CollectionAssert.AreEqual(redovi, dto.Redovi.ToList());
            Assert.AreSame(filter, dto.Filter);
        }
    }

    [TestClass]
    public class StatistikaFilterDtoTests
    {
        [TestMethod]
        public void NoviDto_ImaZadaneVrijednosti()
        {
            // Act
            var dto = new StatistikaFilterDTO();

            // Assert
            Assert.IsNull(dto.KategorijaId);
            Assert.IsNull(dto.MjestoId);
            Assert.AreEqual(KoloneStatistike.Ocjena, dto.AktivnaKolona);
            Assert.AreEqual(Sortiranje.Descending, dto.Smjer);
            Assert.IsNull(dto.Godina);
            Assert.IsNull(dto.Mjesec);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = new StatistikaFilterDTO
            {
                KategorijaId = 2,
                MjestoId = 3,
                AktivnaKolona = KoloneStatistike.Poslovi,
                Smjer = Sortiranje.Ascending,
                Godina = 2026,
                Mjesec = 11
            };

            // Act + Assert
            Assert.AreEqual(2, dto.KategorijaId);
            Assert.AreEqual(3, dto.MjestoId);
            Assert.AreEqual(KoloneStatistike.Poslovi, dto.AktivnaKolona);
            Assert.AreEqual(Sortiranje.Ascending, dto.Smjer);
            Assert.AreEqual(2026, dto.Godina);
            Assert.AreEqual(11, dto.Mjesec);
        }
    }

    [TestClass]
    public class StatistikaRedDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Arrange
            var dto = new StatistikaRedDto
            {
                Rang = 1,
                IzvrsilacID = "majstor-1",
                Username = "majstor1",
                TipKorisnika = KorisnickeUloge.Majstor,
                DisplayName = "Milan Jovanovic",
                Slika = "https://cdn.example.com/majstor.jpg",
                KategorijaNaziv = "Vodovod",
                MjestoNaziv = "Sarajevo",
                ProsjecnaOcjena = 4.8m,
                BrojPoslova = 32
            };

            // Act + Assert
            Assert.AreEqual(1, dto.Rang);
            Assert.AreEqual("majstor-1", dto.IzvrsilacID);
            Assert.AreEqual("majstor1", dto.Username);
            Assert.AreEqual(KorisnickeUloge.Majstor, dto.TipKorisnika);
            Assert.AreEqual("Milan Jovanovic", dto.DisplayName);
            Assert.AreEqual("https://cdn.example.com/majstor.jpg", dto.Slika);
            Assert.AreEqual("Vodovod", dto.KategorijaNaziv);
            Assert.AreEqual("Sarajevo", dto.MjestoNaziv);
            Assert.AreEqual(4.8m, dto.ProsjecnaOcjena);
            Assert.AreEqual(32, dto.BrojPoslova);
        }
    }
}
