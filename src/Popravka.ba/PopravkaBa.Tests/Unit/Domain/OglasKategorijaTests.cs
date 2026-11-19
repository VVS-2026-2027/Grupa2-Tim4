
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasKategorijaTests
    {
        // Pomoćna konkretna klasa za testiranje
        // apstraktne klase Oglas.
        private sealed class TestniOglas : Oglas
        {
        }

        [TestMethod]
        public void NoviOglasKategorija_IDJeNula()
        {
            // Arrange i Act
            var veza = new OglasKategorija();

            // Assert
            Assert.AreEqual(0, veza.ID);
        }

        [TestMethod]
        public void NoviOglasKategorija_OglasIDJeNula()
        {
            // Arrange i Act
            var veza = new OglasKategorija();

            // Assert
            Assert.AreEqual(0, veza.OglasID);
        }

        [TestMethod]
        public void NoviOglasKategorija_KategorijaIDJeNula()
        {
            // Arrange i Act
            var veza = new OglasKategorija();

            // Assert
            Assert.AreEqual(0, veza.KategorijaID);
        }

        [TestMethod]
        public void NoviOglasKategorija_ReferenceSuNull()
        {
            // Arrange i Act
            var veza = new OglasKategorija();

            // Assert
            Assert.IsNull(veza.Oglas);
            Assert.IsNull(veza.Kategorija);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new OglasKategorija();

            // Act
            veza.ID = 10;

            // Assert
            Assert.AreEqual(10, veza.ID);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new OglasKategorija();

            // Act
            veza.OglasID = 25;

            // Assert
            Assert.AreEqual(25, veza.OglasID);
        }

        [TestMethod]
        public void KategorijaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var veza = new OglasKategorija();

            // Act
            veza.KategorijaID = 5;

            // Assert
            Assert.AreEqual(5, veza.KategorijaID);
        }

        [TestMethod]
        public void Oglas_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new OglasKategorija();

            var oglas = new TestniOglas
            {
                OglasID = 25,
                Naslov = "Popravka bojlera"
            };

            // Act
            veza.Oglas = oglas;

            // Assert
            Assert.AreSame(
                oglas,
                veza.Oglas);
        }

        [TestMethod]
        public void Kategorija_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var veza = new OglasKategorija();

            var kategorija = new Kategorija
            {
                ID = 5,
                Naziv = "Vodoinstalaterski radovi"
            };

            // Act
            veza.Kategorija = kategorija;

            // Assert
            Assert.AreSame(
                kategorija,
                veza.Kategorija);
        }

        [TestMethod]
        public void OglasKategorija_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var oglas = new TestniOglas
            {
                OglasID = 15,
                Naslov = "Popravka slavine"
            };

            var kategorija = new Kategorija
            {
                ID = 3,
                Naziv = "Vodoinstalaterski radovi"
            };

            // Act
            var veza = new OglasKategorija
            {
                ID = 1,
                OglasID = 15,
                Oglas = oglas,
                KategorijaID = 3,
                Kategorija = kategorija
            };

            // Assert
            Assert.AreEqual(1, veza.ID);
            Assert.AreEqual(15, veza.OglasID);
            Assert.AreSame(oglas, veza.Oglas);
            Assert.AreEqual(3, veza.KategorijaID);
            Assert.AreSame(kategorija, veza.Kategorija);
        }

        [TestMethod]
        public void Oglas_ZamjenaReference_KoristiNoviOglas()
        {
            // Arrange
            var veza = new OglasKategorija();

            var prviOglas = new TestniOglas
            {
                OglasID = 1
            };

            var drugiOglas = new TestniOglas
            {
                OglasID = 2
            };

            veza.Oglas = prviOglas;

            // Act
            veza.Oglas = drugiOglas;

            // Assert
            Assert.AreSame(
                drugiOglas,
                veza.Oglas);
        }

        [TestMethod]
        public void Kategorija_ZamjenaReference_KoristiNovuKategoriju()
        {
            // Arrange
            var veza = new OglasKategorija();

            var prvaKategorija = new Kategorija
            {
                ID = 1,
                Naziv = "Električarski radovi"
            };

            var drugaKategorija = new Kategorija
            {
                ID = 2,
                Naziv = "Vodoinstalaterski radovi"
            };

            veza.Kategorija = prvaKategorija;

            // Act
            veza.Kategorija = drugaKategorija;

            // Assert
            Assert.AreSame(
                drugaKategorija,
                veza.Kategorija);
        }
    }
}
