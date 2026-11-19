
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasTests
    {
        // Pomoćna konkretna klasa za testiranje
        // apstraktne klase Oglas.
        private sealed class TestniOglas : Oglas
        {
        }

        private static Oglas KreirajOglas()
        {
            return new TestniOglas();
        }

        [TestMethod]
        public void NoviOglas_IDJeNula()
        {
            // Arrange i Act
            var oglas = KreirajOglas();

            // Assert
            Assert.AreEqual(0, oglas.OglasID);
        }

        [TestMethod]
        public void NoviOglas_StatusJeAktivan()
        {
            // Arrange i Act
            var oglas = KreirajOglas();

            // Assert
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }

        [TestMethod]
        public void NoviOglas_BrojPrijavaJeNula()
        {
            // Arrange i Act
            var oglas = KreirajOglas();

            // Assert
            Assert.AreEqual(0, oglas.BrojPrijava);
        }

        [TestMethod]
        public void NoviOglas_OpcionalnaSvojstvaSuNull()
        {
            // Arrange i Act
            var oglas = KreirajOglas();

            // Assert
            Assert.IsNull(oglas.Slika);
            Assert.IsNull(oglas.Kategorije);
            Assert.IsNull(oglas.Notifikacije);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.OglasID = 15;

            // Assert
            Assert.AreEqual(15, oglas.OglasID);
        }

        [TestMethod]
        public void Naslov_PostavljanjeVrijednosti_CuvaNaslov()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.Naslov = "Popravka slavine";

            // Assert
            Assert.AreEqual(
                "Popravka slavine",
                oglas.Naslov);
        }

        [TestMethod]
        public void Opis_PostavljanjeVrijednosti_CuvaOpis()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.Opis = "Potrebna popravka kuhinjske slavine.";

            // Assert
            Assert.AreEqual(
                "Potrebna popravka kuhinjske slavine.",
                oglas.Opis);
        }

        [TestMethod]
        public void DatumObjave_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var oglas = KreirajOglas();

            var datum = new DateTime(
                2026, 11, 19, 12, 30, 0,
                DateTimeKind.Utc);

            // Act
            oglas.DatumObjave = datum;

            // Assert
            Assert.AreEqual(
                datum,
                oglas.DatumObjave);
        }

        [TestMethod]
        public void MjestoID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.MjestoID = 5;

            // Assert
            Assert.AreEqual(5, oglas.MjestoID);
        }

        [TestMethod]
        public void Mjesto_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var oglas = KreirajOglas();

            var mjesto = new Mjesto
            {
                MjestoID = 5,
                Naziv = "Mostar"
            };

            // Act
            oglas.Mjesto = mjesto;

            // Assert
            Assert.AreSame(
                mjesto,
                oglas.Mjesto);
        }

        [TestMethod]
        public void VlasnikOglasaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.VlasnikOglasaID = "korisnik-123";

            // Assert
            Assert.AreEqual(
                "korisnik-123",
                oglas.VlasnikOglasaID);
        }

        [TestMethod]
        public void VlasnikOglasa_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var oglas = KreirajOglas();

            var korisnik = new Klijent
            {
                Id = "korisnik-123"
            };

            // Act
            oglas.VlasnikOglasa = korisnik;

            // Assert
            Assert.AreSame(
                korisnik,
                oglas.VlasnikOglasa);
        }

        [TestMethod]
        public void Kategorije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = KreirajOglas();
            var kategorije = new List<OglasKategorija>();

            // Act
            oglas.Kategorije = kategorije;

            // Assert
            Assert.AreSame(
                kategorije,
                oglas.Kategorije);
        }

        [TestMethod]
        public void Notifikacije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = KreirajOglas();
            var notifikacije = new List<NotifikacijaOglas>();

            // Act
            oglas.Notifikacije = notifikacije;

            // Assert
            Assert.AreSame(
                notifikacije,
                oglas.Notifikacije);
        }

        [TestMethod]
        public void Slika_PostavljanjeVrijednosti_CuvaUrl()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.Slika = "https://popravka.ba/slike/oglas1.jpg";

            // Assert
            Assert.AreEqual(
                "https://popravka.ba/slike/oglas1.jpg",
                oglas.Slika);
        }

        [TestMethod]
        public void StatusOglasa_PostavljanjeNeaktivan_CuvaStatus()
        {
            // Arrange
            var oglas = KreirajOglas();

            // Act
            oglas.StatusOglasa = Status.Neaktivan;

            // Assert
            Assert.AreEqual(
                Status.Neaktivan,
                oglas.StatusOglasa);
        }

        [TestMethod]
        public void Oglas_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var datum = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            var mjesto = new Mjesto
            {
                MjestoID = 2,
                Naziv = "Sarajevo"
            };

            var vlasnik = new Klijent
            {
                Id = "klijent-001"
            };

            var kategorije = new List<OglasKategorija>();
            var notifikacije = new List<NotifikacijaOglas>();

            // Act
            Oglas oglas = new TestniOglas
            {
                OglasID = 1,
                Naslov = "Popravka bojlera",
                Opis = "Potrebna popravka bojlera.",
                DatumObjave = datum,
                MjestoID = 2,
                Mjesto = mjesto,
                VlasnikOglasaID = "klijent-001",
                VlasnikOglasa = vlasnik,
                Kategorije = kategorije,
                Notifikacije = notifikacije,
                Slika = "/images/bojler.jpg",
                StatusOglasa = Status.Aktivan
            };

            // Assert
            Assert.AreEqual(1, oglas.OglasID);
            Assert.AreEqual("Popravka bojlera", oglas.Naslov);
            Assert.AreEqual(
                "Potrebna popravka bojlera.",
                oglas.Opis);
            Assert.AreEqual(datum, oglas.DatumObjave);
            Assert.AreEqual(2, oglas.MjestoID);
            Assert.AreSame(mjesto, oglas.Mjesto);
            Assert.AreEqual(
                "klijent-001",
                oglas.VlasnikOglasaID);
            Assert.AreSame(vlasnik, oglas.VlasnikOglasa);
            Assert.AreSame(kategorije, oglas.Kategorije);
            Assert.AreSame(notifikacije, oglas.Notifikacije);
            Assert.AreEqual(
                "/images/bojler.jpg",
                oglas.Slika);
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }
    }
}
