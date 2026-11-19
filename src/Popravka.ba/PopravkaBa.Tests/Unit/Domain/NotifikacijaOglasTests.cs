
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class NotifikacijaOglasTests
    {
        private static NotifikacijaOglas KreirajNotifikaciju()
        {
            return new EmailNotifikacijaOglas
            {
                EmailPrimalac = "korisnik@example.com"
            };
        }

        [TestMethod]
        public void NovaNotifikacija_PodrazumijevaneVrijednostiSuIspravne()
        {
            // Arrange i Act
            var notifikacija = KreirajNotifikaciju();

            // Assert
            Assert.AreEqual(0, notifikacija.ID);
            Assert.AreEqual(0, notifikacija.OglasID);
            Assert.AreEqual(
                default(DateTime),
                notifikacija.DatumSlanja);

            Assert.IsNull(notifikacija.Oglas);
            Assert.IsNull(notifikacija.Tekst);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();

            // Act
            notifikacija.ID = 10;

            // Assert
            Assert.AreEqual(10, notifikacija.ID);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();

            // Act
            notifikacija.OglasID = 25;

            // Assert
            Assert.AreEqual(25, notifikacija.OglasID);
        }

        [TestMethod]
        public void Oglas_PostavljanjeNullVrijednosti_CuvaNull()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();

            // Act
            notifikacija.Oglas = null!;

            // Assert
            Assert.IsNull(notifikacija.Oglas);
        }

        [TestMethod]
        public void DatumSlanja_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();

            var datum = new DateTime(
                2026, 11, 19, 14, 30, 0,
                DateTimeKind.Utc);

            // Act
            notifikacija.DatumSlanja = datum;

            // Assert
            Assert.AreEqual(
                datum,
                notifikacija.DatumSlanja);
        }

        [TestMethod]
        public void Tekst_PostavljanjeVrijednosti_CuvaTekst()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();

            // Act
            notifikacija.Tekst = "Vaš oglas je uklonjen.";

            // Assert
            Assert.AreEqual(
                "Vaš oglas je uklonjen.",
                notifikacija.Tekst);
        }

        [TestMethod]
        public void OglasID_PromjenaVrijednosti_CuvaNoviID()
        {
            // Arrange
            var notifikacija = KreirajNotifikaciju();
            notifikacija.OglasID = 10;

            // Act
            notifikacija.OglasID = 20;

            // Assert
            Assert.AreEqual(
                20,
                notifikacija.OglasID);
        }

        [TestMethod]
        public void NotifikacijaOglas_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var datum = new DateTime(
                2026, 11, 19, 12, 0, 0,
                DateTimeKind.Utc);

            // Act
            NotifikacijaOglas notifikacija =
                new EmailNotifikacijaOglas
                {
                    ID = 1,
                    OglasID = 5,
                    Oglas = null!,
                    DatumSlanja = datum,
                    Tekst = "Obavijest o oglasu",
                    EmailPrimalac = "korisnik@example.com"
                };

            // Assert
            Assert.AreEqual(1, notifikacija.ID);
            Assert.AreEqual(5, notifikacija.OglasID);
            Assert.IsNull(notifikacija.Oglas);
            Assert.AreEqual(
                datum,
                notifikacija.DatumSlanja);

            Assert.AreEqual(
                "Obavijest o oglasu",
                notifikacija.Tekst);
        }

        [TestMethod]
        public void EmailNotifikacijaOglas_NasljedjujeNotifikacijaOglas()
        {
            // Arrange i Act
            var notifikacija = new EmailNotifikacijaOglas();

            // Assert
            Assert.IsInstanceOfType(
                notifikacija,
                typeof(NotifikacijaOglas));
        }
    }
}
