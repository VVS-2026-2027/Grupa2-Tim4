
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class UvjetOglasaTests
    {
        [TestMethod]
        public void NoviUvjet_IDJeNula()
        {
            // Arrange i Act
            var uvjet = new UvjetOglasa();

            // Assert
            Assert.AreEqual(0, uvjet.ID);
        }

        [TestMethod]
        public void NoviUvjet_OglasIDJeNula()
        {
            // Arrange i Act
            var uvjet = new UvjetOglasa();

            // Assert
            Assert.AreEqual(0, uvjet.OglasID);
        }

        [TestMethod]
        public void NoviUvjet_ReferenceSuNull()
        {
            // Arrange i Act
            var uvjet = new UvjetOglasa();

            // Assert
            Assert.IsNull(uvjet.Oglas);
            Assert.IsNull(uvjet.TekstUvjeta);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            // Act
            uvjet.ID = 10;

            // Assert
            Assert.AreEqual(10, uvjet.ID);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            // Act
            uvjet.OglasID = 25;

            // Assert
            Assert.AreEqual(25, uvjet.OglasID);
        }

        [TestMethod]
        public void Oglas_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            var oglas = new OglasRadnoMjesto
            {
                OglasID = 25,
                Naslov = "Potreban elektricar"
            };

            // Act
            uvjet.Oglas = oglas;

            // Assert
            Assert.AreSame(oglas, uvjet.Oglas);
        }

        [TestMethod]
        public void Oglas_ZamjenaReference_KoristiNoviOglas()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            var prviOglas = new OglasRadnoMjesto
            {
                OglasID = 1
            };

            var drugiOglas = new OglasRadnoMjesto
            {
                OglasID = 2
            };

            uvjet.Oglas = prviOglas;

            // Act
            uvjet.Oglas = drugiOglas;

            // Assert
            Assert.AreSame(
                drugiOglas,
                uvjet.Oglas);
        }

        [TestMethod]
        public void TekstUvjeta_PostavljanjeVrijednosti_CuvaTekst()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            // Act
            uvjet.TekstUvjeta =
                "Minimalno 3 godine radnog iskustva";

            // Assert
            Assert.AreEqual(
                "Minimalno 3 godine radnog iskustva",
                uvjet.TekstUvjeta);
        }

        [TestMethod]
        public void TekstUvjeta_PrazanString_CuvaPraznuVrijednost()
        {
            // Arrange
            var uvjet = new UvjetOglasa();

            // Act
            uvjet.TekstUvjeta = string.Empty;

            // Assert
            Assert.AreEqual(
                string.Empty,
                uvjet.TekstUvjeta);
        }

        [TestMethod]
        public void TekstUvjeta_PromjenaVrijednosti_CuvaNoviTekst()
        {
            // Arrange
            var uvjet = new UvjetOglasa
            {
                TekstUvjeta = "Stari uslov"
            };

            // Act
            uvjet.TekstUvjeta =
                "Obavezna vozacka dozvola B kategorije";

            // Assert
            Assert.AreEqual(
                "Obavezna vozacka dozvola B kategorije",
                uvjet.TekstUvjeta);
        }

        [TestMethod]
        public void OglasID_PromjenaVrijednosti_CuvaNoviID()
        {
            // Arrange
            var uvjet = new UvjetOglasa
            {
                OglasID = 10
            };

            // Act
            uvjet.OglasID = 20;

            // Assert
            Assert.AreEqual(20, uvjet.OglasID);
        }

        [TestMethod]
        public void UvjetOglasa_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                OglasID = 15,
                Naslov = "Potreban vodoinstalater"
            };

            // Act
            var uvjet = new UvjetOglasa
            {
                ID = 1,
                OglasID = 15,
                Oglas = oglas,
                TekstUvjeta =
                    "Minimalno 2 godine iskustva"
            };

            // Assert
            Assert.AreEqual(1, uvjet.ID);
            Assert.AreEqual(15, uvjet.OglasID);
            Assert.AreSame(oglas, uvjet.Oglas);

            Assert.AreEqual(
                "Minimalno 2 godine iskustva",
                uvjet.TekstUvjeta);
        }
    }
}
