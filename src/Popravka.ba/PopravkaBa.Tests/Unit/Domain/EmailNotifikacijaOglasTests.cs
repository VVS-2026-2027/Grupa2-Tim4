
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class EmailNotifikacijaOglasTests
    {
        [TestMethod]
        public void ZaBrisanjeZbogNeaktivnosti_ValidniPodaci_KreiraObavijest()
        {
            // Arrange
            var email = "korisnik@example.com";
            var ime = "Emir";
            var naslov = "Popravka slavine";
            var oglasId = 15;

            // Act
            var obavijest =
                EmailNotifikacijaOglas.ZaBrisanjeZbogNeaktivnosti(
                    email, ime, naslov, oglasId);

            // Assert
            Assert.IsNotNull(obavijest);
            Assert.AreEqual(15, obavijest.OglasID);
            Assert.AreEqual(email, obavijest.EmailPrimalac);
            Assert.AreEqual(ime, obavijest.PrimalacIme);
            Assert.AreEqual(naslov, obavijest.OglasNaslov);

            Assert.AreEqual(
                "Vaš oglas \"Popravka slavine\" uklonjen je zbog neaktivnosti.",
                obavijest.Tekst);
        }

        [TestMethod]
        public void ZaBrisanjeZbogNeaktivnosti_DatumSlanja_PostavljenNaTrenutnoVrijeme()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var obavijest =
                EmailNotifikacijaOglas.ZaBrisanjeZbogNeaktivnosti(
                    "korisnik@example.com",
                    "Emir",
                    "Oglas",
                    1);

            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(obavijest.DatumSlanja >= prije);
            Assert.IsTrue(obavijest.DatumSlanja <= poslije);
        }

        [TestMethod]
        public void ZaBrisanjeZbogNeaktivnosti_NullPodaci_CuvaNullVrijednosti()
        {
            // Act
            var obavijest =
                EmailNotifikacijaOglas.ZaBrisanjeZbogNeaktivnosti(
                    "korisnik@example.com",
                    null,
                    null,
                    5);

            // Assert
            Assert.IsNull(obavijest.PrimalacIme);
            Assert.IsNull(obavijest.OglasNaslov);
            Assert.AreEqual(5, obavijest.OglasID);

            Assert.AreEqual(
                "Vaš oglas \"\" uklonjen je zbog neaktivnosti.",
                obavijest.Tekst);
        }

        [TestMethod]
        public void FormatIntoEmail_ValidniPodaci_SadrziImeINaslov()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = "Popravka slavine",
                DatumSlanja = new DateTime(2026, 11, 19, 14, 30, 0)
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Poštovani Emir,");
            StringAssert.Contains(html, "Popravka slavine");
            StringAssert.Contains(html, "19.11.2026. u 14:30");
        }

        [TestMethod]
        public void FormatIntoEmail_NullIme_KoristiPodrazumijevaniPozdrav()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = null,
                OglasNaslov = "Popravka slavine"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Poštovani korisniče,");
        }

        [TestMethod]
        public void FormatIntoEmail_PraznoIme_KoristiPodrazumijevaniPozdrav()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "",
                OglasNaslov = "Popravka slavine"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Poštovani korisniče,");
        }

        [TestMethod]
        public void FormatIntoEmail_ImeSaRazmacima_KoristiPodrazumijevaniPozdrav()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "   ",
                OglasNaslov = "Popravka slavine"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Poštovani korisniče,");
        }

        [TestMethod]
        public void FormatIntoEmail_NullNaslov_KoristiPodrazumijevaniNaslov()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = null
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Vaš oglas");
            StringAssert.Contains(html, "Poštovani Emir,");
        }

        [TestMethod]
        public void FormatIntoEmail_PrazanNaslov_KoristiPodrazumijevaniNaslov()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = ""
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Vaš oglas");
        }

        [TestMethod]
        public void FormatIntoEmail_NaslovSaRazmacima_KoristiPodrazumijevaniNaslov()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = "   "
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Vaš oglas");
        }

        [TestMethod]
        public void FormatIntoEmail_ValidniPodaci_GeneriseHtmlDokument()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = "Oglas"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "<!DOCTYPE html>");
            StringAssert.Contains(html, "<html lang=\"bs\">");
            StringAssert.Contains(html, "</html>");
            StringAssert.Contains(html, "Popravka.ba");
        }

        [TestMethod]
        public void FormatIntoEmail_ValidniPodaci_SadrziLinkZaNoviOglas()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = "Oglas"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "https://popravka.ba/");
            StringAssert.Contains(html, "Objavite novi oglas");
        }

        [TestMethod]
        public void FormatIntoEmail_ValidniPodaci_SadrziTekstOUklanjanju()
        {
            // Arrange
            var obavijest = new EmailNotifikacijaOglas
            {
                PrimalacIme = "Emir",
                OglasNaslov = "Oglas"
            };

            // Act
            var html = obavijest.FormatIntoEmail();

            // Assert
            StringAssert.Contains(html, "Vaš oglas je uklonjen");
            StringAssert.Contains(html, "zbog neaktivnosti");
        }
    }
}
