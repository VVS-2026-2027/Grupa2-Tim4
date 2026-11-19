
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class MjestoTests
    {
        [TestMethod]
        public void NovoMjesto_IDJeNula()
        {
            // Arrange i Act
            var mjesto = new Mjesto();

            // Assert
            Assert.AreEqual(0, mjesto.MjestoID);
        }

        [TestMethod]
        public void NovoMjesto_NazivJeNull()
        {
            // Arrange i Act
            var mjesto = new Mjesto();

            // Assert
            Assert.IsNull(mjesto.Naziv);
        }

        [TestMethod]
        public void NovoMjesto_KantonImaPodrazumijevanuVrijednost()
        {
            // Arrange i Act
            var mjesto = new Mjesto();

            // Assert
            Assert.AreEqual(default(Kanton), mjesto.Kanton);
        }

        [TestMethod]
        public void MjestoID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var mjesto = new Mjesto();

            // Act
            mjesto.MjestoID = 15;

            // Assert
            Assert.AreEqual(15, mjesto.MjestoID);
        }

        [TestMethod]
        public void Naziv_PostavljanjeVrijednosti_CuvaNaziv()
        {
            // Arrange
            var mjesto = new Mjesto();

            // Act
            mjesto.Naziv = "Mostar";

            // Assert
            Assert.AreEqual("Mostar", mjesto.Naziv);
        }

        [TestMethod]
        public void Naziv_PromjenaVrijednosti_CuvaNoviNaziv()
        {
            // Arrange
            var mjesto = new Mjesto
            {
                Naziv = "Sarajevo"
            };

            // Act
            mjesto.Naziv = "Mostar";

            // Assert
            Assert.AreEqual("Mostar", mjesto.Naziv);
        }

        [TestMethod]
        public void Kanton_PostavljanjeVrijednosti_CuvaKanton()
        {
            // Arrange
            var mjesto = new Mjesto();
            var ocekivaniKanton = (Kanton)1;

            // Act
            mjesto.Kanton = ocekivaniKanton;

            // Assert
            Assert.AreEqual(ocekivaniKanton, mjesto.Kanton);
        }

        [TestMethod]
        public void Kanton_PromjenaVrijednosti_CuvaNoviKanton()
        {
            // Arrange
            var mjesto = new Mjesto
            {
                Kanton = (Kanton)1
            };

            // Act
            mjesto.Kanton = (Kanton)2;

            // Assert
            Assert.AreEqual((Kanton)2, mjesto.Kanton);
        }

        [TestMethod]
        public void Mjesto_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange i Act
            var mjesto = new Mjesto
            {
                MjestoID = 10,
                Naziv = "Mostar",
                Kanton = (Kanton)1
            };

            // Assert
            Assert.AreEqual(10, mjesto.MjestoID);
            Assert.AreEqual("Mostar", mjesto.Naziv);
            Assert.AreEqual((Kanton)1, mjesto.Kanton);
        }

        [TestMethod]
        public void Naziv_PrazanString_CuvaPraznuVrijednost()
        {
            // Arrange
            var mjesto = new Mjesto();

            // Act
            mjesto.Naziv = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, mjesto.Naziv);
        }
    }
}
