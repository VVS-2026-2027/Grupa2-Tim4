
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class KategorijaTests
    {
        [TestMethod]
        public void NovaKategorija_IDJeNula()
        {
            // Arrange i Act
            var kategorija = new Kategorija();

            // Assert
            Assert.AreEqual(0, kategorija.ID);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var kategorija = new Kategorija();

            // Act
            kategorija.ID = 10;

            // Assert
            Assert.AreEqual(10, kategorija.ID);
        }

        [TestMethod]
        public void Naziv_PostavljanjeVrijednosti_CuvaNaziv()
        {
            // Arrange
            var kategorija = new Kategorija();

            // Act
            kategorija.Naziv = "Vodoinstalaterski radovi";

            // Assert
            Assert.AreEqual(
                "Vodoinstalaterski radovi",
                kategorija.Naziv);
        }

        [TestMethod]
        public void NadkategorijaID_NovaKategorija_JeNull()
        {
            // Arrange i Act
            var kategorija = new Kategorija();

            // Assert
            Assert.IsNull(kategorija.NadkategorijaID);
        }

        [TestMethod]
        public void NadkategorijaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var kategorija = new Kategorija();

            // Act
            kategorija.NadkategorijaID = 5;

            // Assert
            Assert.AreEqual(5, kategorija.NadkategorijaID);
        }

        [TestMethod]
        public void Nadkategorija_NovaKategorija_JeNull()
        {
            // Arrange i Act
            var kategorija = new Kategorija();

            // Assert
            Assert.IsNull(kategorija.Nadkategorija);
        }

        [TestMethod]
        public void Nadkategorija_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var glavnaKategorija = new Kategorija
            {
                ID = 1,
                Naziv = "Građevinski radovi"
            };

            var potkategorija = new Kategorija
            {
                ID = 2,
                Naziv = "Keramičarski radovi"
            };

            // Act
            potkategorija.Nadkategorija = glavnaKategorija;
            potkategorija.NadkategorijaID = glavnaKategorija.ID;

            // Assert
            Assert.AreSame(
                glavnaKategorija,
                potkategorija.Nadkategorija);

            Assert.AreEqual(
                1,
                potkategorija.NadkategorijaID);
        }

        [TestMethod]
        public void Potkategorije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var glavnaKategorija = new Kategorija();

            var potkategorije = new List<Kategorija>
            {
                new Kategorija
                {
                    ID = 2,
                    Naziv = "Keramičarski radovi"
                },
                new Kategorija
                {
                    ID = 3,
                    Naziv = "Molerski radovi"
                }
            };

            // Act
            glavnaKategorija.Potkategorije = potkategorije;

            // Assert
            Assert.AreSame(
                potkategorije,
                glavnaKategorija.Potkategorije);

            Assert.AreEqual(
                2,
                glavnaKategorija.Potkategorije.Count);
        }

        [TestMethod]
        public void Potkategorije_DodavanjePotkategorije_PovecavaBroj()
        {
            // Arrange
            var glavnaKategorija = new Kategorija
            {
                ID = 1,
                Naziv = "Građevinski radovi",
                Potkategorije = new List<Kategorija>()
            };

            var novaPotkategorija = new Kategorija
            {
                ID = 2,
                Naziv = "Keramičarski radovi",
                NadkategorijaID = 1,
                Nadkategorija = glavnaKategorija
            };

            // Act
            glavnaKategorija.Potkategorije.Add(
                novaPotkategorija);

            // Assert
            Assert.AreEqual(
                1,
                glavnaKategorija.Potkategorije.Count);

            Assert.AreSame(
                novaPotkategorija,
                glavnaKategorija.Potkategorije.First());
        }

        [TestMethod]
        public void Potkategorije_VisePotkategorija_SadrziSveKategorije()
        {
            // Arrange
            var glavnaKategorija = new Kategorija
            {
                ID = 1,
                Naziv = "Kućne popravke",
                Potkategorije = new List<Kategorija>()
            };

            var prva = new Kategorija
            {
                ID = 2,
                Naziv = "Električarski radovi"
            };

            var druga = new Kategorija
            {
                ID = 3,
                Naziv = "Vodoinstalaterski radovi"
            };

            // Act
            glavnaKategorija.Potkategorije.Add(prva);
            glavnaKategorija.Potkategorije.Add(druga);

            // Assert
            Assert.AreEqual(
                2,
                glavnaKategorija.Potkategorije.Count);

            Assert.IsTrue(
                glavnaKategorija.Potkategorije.Contains(prva));

            Assert.IsTrue(
                glavnaKategorija.Potkategorije.Contains(druga));
        }

        [TestMethod]
        public void Hijerarhija_NadkategorijaIPotkategorija_IspravnoPovezane()
        {
            // Arrange
            var nadkategorija = new Kategorija
            {
                ID = 10,
                Naziv = "Građevinski radovi",
                Potkategorije = new List<Kategorija>()
            };

            var potkategorija = new Kategorija
            {
                ID = 11,
                Naziv = "Keramičarski radovi",
                NadkategorijaID = 10,
                Nadkategorija = nadkategorija
            };

            // Act
            nadkategorija.Potkategorije.Add(potkategorija);

            // Assert
            Assert.AreEqual(
                nadkategorija.ID,
                potkategorija.NadkategorijaID);

            Assert.AreSame(
                nadkategorija,
                potkategorija.Nadkategorija);

            Assert.IsTrue(
                nadkategorija.Potkategorije.Contains(potkategorija));
        }

        [TestMethod]
        public void NovaKategorija_PotkategorijeSuNull()
        {
            // Arrange i Act
            var kategorija = new Kategorija();

            // Assert
            Assert.IsNull(kategorija.Potkategorije);
        }
    }
}
