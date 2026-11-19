
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasMajstoraTests
    {
        [TestMethod]
        public void NoviOglasMajstora_MinCijenaJeNula()
        {
            // Arrange i Act
            var oglas = new OglasMajstora();

            // Assert
            Assert.AreEqual(0.0, oglas.MinCijena);
        }

        [TestMethod]
        public void NoviOglasMajstora_TipIsplateImaPocetnuVrijednost()
        {
            // Arrange i Act
            var oglas = new OglasMajstora();

            // Assert
            Assert.AreEqual(
                default(TipIsplate),
                oglas.TipIsplate);
        }

        [TestMethod]
        public void MinCijena_PostavljanjeVrijednosti_CuvaCijenu()
        {
            // Arrange
            var oglas = new OglasMajstora();

            // Act
            oglas.MinCijena = 150.0;

            // Assert
            Assert.AreEqual(150.0, oglas.MinCijena);
        }

        [TestMethod]
        public void MinCijena_DecimalnaVrijednost_CuvaCijenu()
        {
            // Arrange
            var oglas = new OglasMajstora();

            // Act
            oglas.MinCijena = 75.50;

            // Assert
            Assert.AreEqual(75.50, oglas.MinCijena);
        }

        [TestMethod]
        public void MinCijena_NegativnaVrijednost_CuvaVrijednost()
        {
            // Arrange
            var oglas = new OglasMajstora();

            // Act
            oglas.MinCijena = -20.0;

            // Assert
            Assert.AreEqual(-20.0, oglas.MinCijena);
        }

        [TestMethod]
        public void MinCijena_VelikaVrijednost_CuvaCijenu()
        {
            // Arrange
            var oglas = new OglasMajstora();

            // Act
            oglas.MinCijena = 1000000.0;

            // Assert
            Assert.AreEqual(
                1000000.0,
                oglas.MinCijena);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void TipIsplate_PostavljanjeVrijednosti_CuvaEnum(
            int vrijednost)
        {
            // Arrange
            var oglas = new OglasMajstora();
            var tip = (TipIsplate)vrijednost;

            // Act
            oglas.TipIsplate = tip;

            // Assert
            Assert.AreEqual(tip, oglas.TipIsplate);
        }

        [TestMethod]
        public void TipIsplate_PromjenaVrijednosti_CuvaNoviTip()
        {
            // Arrange
            var oglas = new OglasMajstora
            {
                TipIsplate = (TipIsplate)0
            };

            // Act
            oglas.TipIsplate = (TipIsplate)1;

            // Assert
            Assert.AreEqual(
                (TipIsplate)1,
                oglas.TipIsplate);
        }

        [TestMethod]
        public void OglasMajstora_NasljedjujeOglas()
        {
            // Arrange i Act
            var oglas = new OglasMajstora();

            // Assert
            Assert.IsInstanceOfType(
                oglas,
                typeof(Oglas));
        }

        [TestMethod]
        public void NoviOglasMajstora_NaslijedjeniStatusJeAktivan()
        {
            // Arrange i Act
            var oglas = new OglasMajstora();

            // Assert
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }

        [TestMethod]
        public void NoviOglasMajstora_BrojPrijavaJeNula()
        {
            // Arrange i Act
            var oglas = new OglasMajstora();

            // Assert
            Assert.AreEqual(0, oglas.BrojPrijava);
        }

        [TestMethod]
        public void OglasMajstora_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange i Act
            var oglas = new OglasMajstora
            {
                OglasID = 10,
                Naslov = "Popravka bojlera",
                Opis = "Nudim profesionalnu popravku bojlera.",
                MinCijena = 80.0,
                TipIsplate = (TipIsplate)1,
                StatusOglasa = Status.Aktivan
            };

            // Assert
            Assert.AreEqual(10, oglas.OglasID);
            Assert.AreEqual(
                "Popravka bojlera",
                oglas.Naslov);
            Assert.AreEqual(
                "Nudim profesionalnu popravku bojlera.",
                oglas.Opis);
            Assert.AreEqual(80.0, oglas.MinCijena);
            Assert.AreEqual(
                (TipIsplate)1,
                oglas.TipIsplate);
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }
    }
}
