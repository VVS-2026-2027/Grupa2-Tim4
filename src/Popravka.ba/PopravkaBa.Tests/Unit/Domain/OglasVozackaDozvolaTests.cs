
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasVozackaDozvolaTests
    {
        [TestMethod]
        public void NoviObjekat_IDJeNula()
        {
            // Arrange i Act
            var veza = new OglasVozackaDozvola();

            // Assert
            Assert.AreEqual(0, veza.ID);
        }

        [TestMethod]
        public void NoviObjekat_OglasIDJeNula()
        {
            // Arrange i Act
            var veza = new OglasVozackaDozvola();

            // Assert
            Assert.AreEqual(0, veza.OglasID);
        }

        [TestMethod]
        public void NoviObjekat_VozackaDozvolaImaPodrazumijevanuVrijednost()
        {
            // Arrange i Act
            var veza = new OglasVozackaDozvola();

            // Assert
            Assert.AreEqual(
                default(VozackaDozvola),
                veza.VozackaDozvola);
        }

        [TestMethod]
        public void NoviObjekat_OglasJeNull()
        {
            // Arrange i Act
            var veza = new OglasVozackaDozvola();

            // Assert
            Assert.IsNull(veza.Oglas);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new OglasVozackaDozvola();

            // Act
            veza.ID = 10;

            // Assert
            Assert.AreEqual(10, veza.ID);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new OglasVozackaDozvola();

            // Act
            veza.OglasID = 25;

            // Assert
            Assert.AreEqual(25, veza.OglasID);
        }

        [TestMethod]
        public void VozackaDozvola_PostavljanjeVrijednosti_CuvaEnum()
        {
            // Arrange
            var veza = new OglasVozackaDozvola();
            var dozvola = (VozackaDozvola)1;

            // Act
            veza.VozackaDozvola = dozvola;

            // Assert
            Assert.AreEqual(dozvola, veza.VozackaDozvola);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void VozackaDozvola_RazliciteVrijednosti_CuvaVrijednost(
            int vrijednost)
        {
            // Arrange
            var veza = new OglasVozackaDozvola();
            var ocekivanaDozvola = (VozackaDozvola)vrijednost;

            // Act
            veza.VozackaDozvola = ocekivanaDozvola;

            // Assert
            Assert.AreEqual(
                ocekivanaDozvola,
                veza.VozackaDozvola);
        }

        [TestMethod]
        public void Oglas_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new OglasVozackaDozvola();

            var oglas = new OglasRadnoMjesto
            {
                OglasID = 25,
                Naslov = "Potreban vozač"
            };

            // Act
            veza.Oglas = oglas;

            // Assert
            Assert.AreSame(oglas, veza.Oglas);
        }

        [TestMethod]
        public void Oglas_ZamjenaReference_KoristiNoviOglas()
        {
            // Arrange
            var veza = new OglasVozackaDozvola();

            var prviOglas = new OglasRadnoMjesto
            {
                OglasID = 1
            };

            var drugiOglas = new OglasRadnoMjesto
            {
                OglasID = 2
            };

            veza.Oglas = prviOglas;

            // Act
            veza.Oglas = drugiOglas;

            // Assert
            Assert.AreSame(drugiOglas, veza.Oglas);
        }

        [TestMethod]
        public void OglasID_PromjenaVrijednosti_CuvaNoviID()
        {
            // Arrange
            var veza = new OglasVozackaDozvola
            {
                OglasID = 10
            };

            // Act
            veza.OglasID = 20;

            // Assert
            Assert.AreEqual(20, veza.OglasID);
        }

        [TestMethod]
        public void OglasVozackaDozvola_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                OglasID = 15,
                Naslov = "Potreban dostavljač"
            };

            var dozvola = (VozackaDozvola)1;

            // Act
            var veza = new OglasVozackaDozvola
            {
                ID = 5,
                OglasID = 15,
                Oglas = oglas,
                VozackaDozvola = dozvola
            };

            // Assert
            Assert.AreEqual(5, veza.ID);
            Assert.AreEqual(15, veza.OglasID);
            Assert.AreSame(oglas, veza.Oglas);
            Assert.AreEqual(dozvola, veza.VozackaDozvola);
        }
    }
}
