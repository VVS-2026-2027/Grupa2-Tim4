
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class ApplicationUserTests
    {
        [TestMethod]
        public void NoviKorisnik_StatusVerifikacijeJeNaCekanju()
        {
            // Arrange
            var korisnik = new ApplicationUser();

            // Act
            var status = korisnik.StatusVerifikacije;

            // Assert
            Assert.AreEqual(Status.NaCekanju, status);
        }

        [TestMethod]
        public void DisplayName_ImeIPrezime_VracaPunoIme()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                Ime = "Emir",
                Prezime = "Hodzic"
            };

            // Act
            var rezultat = korisnik.DisplayName;

            // Assert
            Assert.AreEqual("Emir Hodzic", rezultat);
        }

        [TestMethod]
        public void SkracenoIme_PostojiPrezime_VracaSkraceniOblik()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                Ime = "Emir",
                Prezime = "Hodzic"
            };

            // Act
            var rezultat = korisnik.SkracenoIme;

            // Assert
            Assert.AreEqual("Emir H.", rezultat);
        }

        [TestMethod]
        public void SkracenoIme_NemaPrezime_VracaDisplayName()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                Ime = "Emir",
                Prezime = ""
            };

            // Act
            var rezultat = korisnik.SkracenoIme;

            // Assert
            Assert.AreEqual(korisnik.DisplayName, rezultat);
        }

        [TestMethod]
        public void Ime_NullVrijednost_NormalizujeUPrazanString()
        {
            // Arrange
            var korisnik = new ApplicationUser();

            // Act
            korisnik.Ime = null;

            // Assert
            Assert.AreEqual(string.Empty, korisnik.Ime);
        }

        [TestMethod]
        public void Prezime_NullVrijednost_NormalizujeUPrazanString()
        {
            // Arrange
            var korisnik = new ApplicationUser();

            // Act
            korisnik.Prezime = null;

            // Assert
            Assert.AreEqual(string.Empty, korisnik.Prezime);
        }

        [TestMethod]
        public void DatumRegistracije_NoviKorisnik_ImaTrenutniDatum()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var korisnik = new ApplicationUser();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(korisnik.DatumRegistracije >= prije);
            Assert.IsTrue(korisnik.DatumRegistracije <= poslije);
        }

        [DataTestMethod]
        [DataRow(true, Status.Aktivan)]
        [DataRow(false, Status.NaCekanju)]
        public void Aktivan_EmailPotvrda_VracaOcekivaniStatus(
            bool emailConfirmed,
            Status ocekivaniStatus)
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = emailConfirmed
            };

            // Act
            var rezultat = korisnik.Aktivan();

            // Assert
            Assert.AreEqual(ocekivaniStatus, rezultat);
        }

        [TestMethod]
        public void SuspendirajNalog_PozvanaMetoda_StatusJeNeaktivan()
        {
            // Arrange
            var korisnik = new ApplicationUser();

            // Act
            korisnik.SuspendirajNalog();

            // Assert
            Assert.AreEqual(
                Status.Neaktivan,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void OsvjeziStatusAktivnosti_PotvrdjenEmail_StatusAktivan()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = true
            };

            // Act
            korisnik.OsvjeziStatusAktivnosti();

            // Assert
            Assert.AreEqual(
                Status.Aktivan,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void OsvjeziStatusAktivnosti_NepotvrdjenEmail_StatusNaCekanju()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = false
            };

            // Act
            korisnik.OsvjeziStatusAktivnosti();

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void OsvjeziStatusAktivnosti_SuspendovanNalog_OstajeNeaktivan()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = true
            };

            korisnik.SuspendirajNalog();

            // Act
            korisnik.OsvjeziStatusAktivnosti();

            // Assert
            Assert.AreEqual(
                Status.Neaktivan,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void OdsuspendirajNalog_PotvrdjenEmail_StatusAktivan()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = true
            };

            korisnik.SuspendirajNalog();

            // Act
            korisnik.OdsuspendirajNalog();

            // Assert
            Assert.AreEqual(
                Status.Aktivan,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void OdsuspendirajNalog_NepotvrdjenEmail_StatusNaCekanju()
        {
            // Arrange
            var korisnik = new ApplicationUser
            {
                EmailConfirmed = false
            };

            korisnik.SuspendirajNalog();

            // Act
            korisnik.OdsuspendirajNalog();

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                korisnik.StatusVerifikacije);
        }

        [TestMethod]
        public void SvojstvaKorisnika_PostavljanjeVrijednosti_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var korisnik = new ApplicationUser();

            var oglasi = new List<Oglas>();
            var mjesta = new List<KorisnikMjesto>();
            var tokeni = new List<VerifikacijskiToken>();

            // Act
            korisnik.Slika = "profil.jpg";
            korisnik.Oglasi = oglasi;
            korisnik.Mjesta = mjesta;
            korisnik.Tokeni = tokeni;

            // Assert
            Assert.AreEqual("profil.jpg", korisnik.Slika);
            Assert.AreSame(oglasi, korisnik.Oglasi);
            Assert.AreSame(mjesta, korisnik.Mjesta);
            Assert.AreSame(tokeni, korisnik.Tokeni);
        }
    }
}
