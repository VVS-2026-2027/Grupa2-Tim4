
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class MjesecnaStatistikaKompozicijaTests
    {
        [TestMethod]
        public void NoviObjekat_NumerickaSvojstvaImajuPocetneVrijednosti()
        {
            // Arrange i Act
            var statistika = new MjesecnaStatistikaKompozicija();

            // Assert
            Assert.AreEqual(0, statistika.ID);
            Assert.AreEqual(0, statistika.Godina);
            Assert.AreEqual(0, statistika.Mjesec);
            Assert.AreEqual(0M, statistika.ProsjecnaOcjena);
            Assert.AreEqual(0, statistika.BrojPoslova);
            Assert.AreEqual(0, statistika.RangStandardni);
        }

        [TestMethod]
        public void NoviObjekat_OpcionalnaSvojstvaSuNull()
        {
            // Arrange i Act
            var statistika = new MjesecnaStatistikaKompozicija();

            // Assert
            Assert.IsNull(statistika.Slika);
            Assert.IsNull(statistika.KategorijaID);
            Assert.IsNull(statistika.KategorijaNaziv);
            Assert.IsNull(statistika.MjestoID);
            Assert.IsNull(statistika.MjestoNaziv);
        }

        [TestMethod]
        public void NoviObjekat_VrijemeAzuriranjaJeTrenutniUtcDatum()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var statistika = new MjesecnaStatistikaKompozicija();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(statistika.VrijemeAzuriranja >= prije);
            Assert.IsTrue(statistika.VrijemeAzuriranja <= poslije);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.ID = 15;

            // Assert
            Assert.AreEqual(15, statistika.ID);
        }

        [TestMethod]
        public void Username_PostavljanjeVrijednosti_CuvaUsername()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.Username = "majstor123";

            // Assert
            Assert.AreEqual(
                "majstor123",
                statistika.Username);
        }

        [TestMethod]
        public void TipKorisnika_PostavljanjeVrijednosti_CuvaEnum()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();
            var tip = default(KorisnickeUloge);

            // Act
            statistika.TipKorisnika = tip;

            // Assert
            Assert.AreEqual(tip, statistika.TipKorisnika);
        }

        [TestMethod]
        public void GodinaIMjesec_PostavljanjeVrijednosti_CuvaPeriod()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.Godina = 2026;
            statistika.Mjesec = 11;

            // Assert
            Assert.AreEqual(2026, statistika.Godina);
            Assert.AreEqual(11, statistika.Mjesec);
        }

        [TestMethod]
        public void IzvrsilacID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.IzvrsilacID = "izvrsilac-001";

            // Assert
            Assert.AreEqual(
                "izvrsilac-001",
                statistika.IzvrsilacID);
        }

        [TestMethod]
        public void DisplayName_PostavljanjeVrijednosti_CuvaNaziv()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.DisplayName = "Firma Sarajevo";

            // Assert
            Assert.AreEqual(
                "Firma Sarajevo",
                statistika.DisplayName);
        }

        [TestMethod]
        public void Slika_PostavljanjeVrijednosti_CuvaPutanju()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.Slika = "/images/firma.png";

            // Assert
            Assert.AreEqual(
                "/images/firma.png",
                statistika.Slika);
        }

        [TestMethod]
        public void Kategorija_PostavljanjeVrijednosti_CuvaPodatke()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.KategorijaID = 5;
            statistika.KategorijaNaziv = "Vodoinstalaterski radovi";

            // Assert
            Assert.AreEqual(5, statistika.KategorijaID);
            Assert.AreEqual(
                "Vodoinstalaterski radovi",
                statistika.KategorijaNaziv);
        }

        [TestMethod]
        public void Mjesto_PostavljanjeVrijednosti_CuvaPodatke()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.MjestoID = 10;
            statistika.MjestoNaziv = "Sarajevo";

            // Assert
            Assert.AreEqual(10, statistika.MjestoID);
            Assert.AreEqual(
                "Sarajevo",
                statistika.MjestoNaziv);
        }

        [TestMethod]
        public void ProsjecnaOcjena_PostavljanjeVrijednosti_CuvaOcjenu()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.ProsjecnaOcjena = 4.85M;

            // Assert
            Assert.AreEqual(
                4.85M,
                statistika.ProsjecnaOcjena);
        }

        [TestMethod]
        public void BrojPoslova_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.BrojPoslova = 25;

            // Assert
            Assert.AreEqual(25, statistika.BrojPoslova);
        }

        [TestMethod]
        public void RangStandardni_PostavljanjeVrijednosti_CuvaRang()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();

            // Act
            statistika.RangStandardni = 1;

            // Assert
            Assert.AreEqual(1, statistika.RangStandardni);
        }

        [TestMethod]
        public void VrijemeAzuriranja_RucnoPostavljanje_CuvaDatum()
        {
            // Arrange
            var statistika = new MjesecnaStatistikaKompozicija();
            var datum = new DateTime(
                2026, 11, 19, 12, 30, 0, DateTimeKind.Utc);

            // Act
            statistika.VrijemeAzuriranja = datum;

            // Assert
            Assert.AreEqual(
                datum,
                statistika.VrijemeAzuriranja);
        }

        [TestMethod]
        public void SvaSvojstva_PostavljanjeVrijednosti_CuvaKompletanSnapshot()
        {
            // Arrange
            var datum = new DateTime(
                2026, 10, 31, 23, 59, 0, DateTimeKind.Utc);

            // Act
            var statistika = new MjesecnaStatistikaKompozicija
            {
                ID = 1,
                Username = "majstor123",
                TipKorisnika = default(KorisnickeUloge),
                Godina = 2026,
                Mjesec = 10,
                IzvrsilacID = "majstor-001",
                DisplayName = "Test Majstor",
                Slika = "/images/majstor.png",
                KategorijaID = 2,
                KategorijaNaziv = "Električarski radovi",
                MjestoID = 3,
                MjestoNaziv = "Mostar",
                ProsjecnaOcjena = 4.9M,
                BrojPoslova = 30,
                RangStandardni = 2,
                VrijemeAzuriranja = datum
            };

            // Assert
            Assert.AreEqual(1, statistika.ID);
            Assert.AreEqual("majstor123", statistika.Username);
            Assert.AreEqual(
                default(KorisnickeUloge),
                statistika.TipKorisnika);
            Assert.AreEqual(2026, statistika.Godina);
            Assert.AreEqual(10, statistika.Mjesec);
            Assert.AreEqual("majstor-001", statistika.IzvrsilacID);
            Assert.AreEqual("Test Majstor", statistika.DisplayName);
            Assert.AreEqual("/images/majstor.png", statistika.Slika);
            Assert.AreEqual(2, statistika.KategorijaID);
            Assert.AreEqual(
                "Električarski radovi",
                statistika.KategorijaNaziv);
            Assert.AreEqual(3, statistika.MjestoID);
            Assert.AreEqual("Mostar", statistika.MjestoNaziv);
            Assert.AreEqual(4.9M, statistika.ProsjecnaOcjena);
            Assert.AreEqual(30, statistika.BrojPoslova);
            Assert.AreEqual(2, statistika.RangStandardni);
            Assert.AreEqual(datum, statistika.VrijemeAzuriranja);
        }
    }
}
