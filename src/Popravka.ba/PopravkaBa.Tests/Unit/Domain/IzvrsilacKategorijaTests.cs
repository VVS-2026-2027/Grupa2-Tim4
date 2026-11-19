
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class IzvrsilacKategorijaTests
    {
        [TestMethod]
        public void NoviObjekat_IDJeNula()
        {
            // Arrange i Act
            var veza = new IzvrsilacKategorija();

            // Assert
            Assert.AreEqual(0, veza.ID);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new IzvrsilacKategorija();

            // Act
            veza.ID = 10;

            // Assert
            Assert.AreEqual(10, veza.ID);
        }

        [TestMethod]
        public void IzvrsilacID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new IzvrsilacKategorija();

            // Act
            veza.IzvrsilacID = "izvrsilac-123";

            // Assert
            Assert.AreEqual(
                "izvrsilac-123",
                veza.IzvrsilacID);
        }

        [TestMethod]
        public void KategorijaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new IzvrsilacKategorija();

            // Act
            veza.KategorijaID = 5;

            // Assert
            Assert.AreEqual(5, veza.KategorijaID);
        }

        [TestMethod]
        public void Izvrsilac_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new IzvrsilacKategorija();
            var izvrsilac = new Firma
            {
                NazivFirme = "Test Firma"
            };

            // Act
            veza.Izvrsilac = izvrsilac;

            // Assert
            Assert.AreSame(
                izvrsilac,
                veza.Izvrsilac);
        }

        [TestMethod]
        public void Kategorija_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new IzvrsilacKategorija();
            var kategorija = new Kategorija();

            // Act
            veza.Kategorija = kategorija;

            // Assert
            Assert.AreSame(
                kategorija,
                veza.Kategorija);
        }

        [TestMethod]
        public void IzvrsilacKategorija_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var izvrsilac = new Firma
            {
                NazivFirme = "Popravke Sarajevo"
            };

            var kategorija = new Kategorija();

            // Act
            var veza = new IzvrsilacKategorija
            {
                ID = 1,
                IzvrsilacID = "firma-001",
                Izvrsilac = izvrsilac,
                KategorijaID = 3,
                Kategorija = kategorija
            };

            // Assert
            Assert.AreEqual(1, veza.ID);
            Assert.AreEqual("firma-001", veza.IzvrsilacID);
            Assert.AreSame(izvrsilac, veza.Izvrsilac);
            Assert.AreEqual(3, veza.KategorijaID);
            Assert.AreSame(kategorija, veza.Kategorija);
        }
    }
}
