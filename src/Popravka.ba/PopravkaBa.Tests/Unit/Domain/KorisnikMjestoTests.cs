
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class KorisnikMjestoTests
    {
        [TestMethod]
        public void NoviKorisnikMjesto_IDJeNula()
        {
            // Arrange i Act
            var veza = new KorisnikMjesto();

            // Assert
            Assert.AreEqual(0, veza.ID);
        }

        [TestMethod]
        public void NoviKorisnikMjesto_MjestoIDJeNula()
        {
            // Arrange i Act
            var veza = new KorisnikMjesto();

            // Assert
            Assert.AreEqual(0, veza.MjestoID);
        }

        [TestMethod]
        public void NoviKorisnikMjesto_ReferenceSuNull()
        {
            // Arrange i Act
            var veza = new KorisnikMjesto();

            // Assert
            Assert.IsNull(veza.KorisnikID);
            Assert.IsNull(veza.Korisnik);
            Assert.IsNull(veza.Mjesto);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            // Act
            veza.ID = 10;

            // Assert
            Assert.AreEqual(10, veza.ID);
        }

        [TestMethod]
        public void KorisnikID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            // Act
            veza.KorisnikID = "korisnik-123";

            // Assert
            Assert.AreEqual(
                "korisnik-123",
                veza.KorisnikID);
        }

        [TestMethod]
        public void Korisnik_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            var korisnik = new Klijent
            {
                Id = "korisnik-123"
            };

            // Act
            veza.Korisnik = korisnik;

            // Assert
            Assert.AreSame(
                korisnik,
                veza.Korisnik);
        }

        [TestMethod]
        public void MjestoID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            // Act
            veza.MjestoID = 5;

            // Assert
            Assert.AreEqual(5, veza.MjestoID);
        }

        [TestMethod]
        public void Mjesto_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new KorisnikMjesto();
            var mjesto = new Mjesto();

            // Act
            veza.Mjesto = mjesto;

            // Assert
            Assert.AreSame(
                mjesto,
                veza.Mjesto);
        }

        [TestMethod]
        public void KorisnikMjesto_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var korisnik = new Klijent
            {
                Id = "klijent-001"
            };

            var mjesto = new Mjesto();

            // Act
            var veza = new KorisnikMjesto
            {
                ID = 1,
                KorisnikID = "klijent-001",
                Korisnik = korisnik,
                MjestoID = 3,
                Mjesto = mjesto
            };

            // Assert
            Assert.AreEqual(1, veza.ID);
            Assert.AreEqual("klijent-001", veza.KorisnikID);
            Assert.AreSame(korisnik, veza.Korisnik);
            Assert.AreEqual(3, veza.MjestoID);
            Assert.AreSame(mjesto, veza.Mjesto);
        }

        [TestMethod]
        public void KorisnikMjesto_ZamjenaKorisnika_KoristiNovuReferencu()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            var prviKorisnik = new Klijent
            {
                Id = "korisnik-1"
            };

            var drugiKorisnik = new Klijent
            {
                Id = "korisnik-2"
            };

            veza.Korisnik = prviKorisnik;

            // Act
            veza.Korisnik = drugiKorisnik;

            // Assert
            Assert.AreSame(
                drugiKorisnik,
                veza.Korisnik);
        }

        [TestMethod]
        public void KorisnikMjesto_ZamjenaMjesta_KoristiNovuReferencu()
        {
            // Arrange
            var veza = new KorisnikMjesto();

            var prvoMjesto = new Mjesto();
            var drugoMjesto = new Mjesto();

            veza.Mjesto = prvoMjesto;

            // Act
            veza.Mjesto = drugoMjesto;

            // Assert
            Assert.AreSame(
                drugoMjesto,
                veza.Mjesto);
        }
    }
}
