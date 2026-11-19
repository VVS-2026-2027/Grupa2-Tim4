
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class PrijavaRadnoMjestoTests
    {
        [TestMethod]
        public void NovaPrijava_IDJeNula()
        {
            // Arrange i Act
            var prijava = new PrijavaRadnoMjesto();

            // Assert
            Assert.AreEqual(0, prijava.ID);
        }

        [TestMethod]
        public void NovaPrijava_OglasIDJeNula()
        {
            // Arrange i Act
            var prijava = new PrijavaRadnoMjesto();

            // Assert
            Assert.AreEqual(0, prijava.OglasID);
        }

        [TestMethod]
        public void NovaPrijava_PovezaniObjektiSuNull()
        {
            // Arrange i Act
            var prijava = new PrijavaRadnoMjesto();

            // Assert
            Assert.IsNull(prijava.OglasRadnoMjesto);
            Assert.IsNull(prijava.MajstorID);
            Assert.IsNull(prijava.Majstor);
        }

        [TestMethod]
        public void NovaPrijava_StatusJeNaCekanju()
        {
            // Arrange i Act
            var prijava = new PrijavaRadnoMjesto();

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                prijava.StatusPrijave);
        }

        [TestMethod]
        public void VrijemePrijave_NovaPrijava_PostavljaTrenutniUtcDatum()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var prijava = new PrijavaRadnoMjesto();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.AreEqual(
                DateTimeKind.Utc,
                prijava.VrijemePrijave.Kind);

            Assert.IsTrue(
                prijava.VrijemePrijave >= prije);

            Assert.IsTrue(
                prijava.VrijemePrijave <= poslije);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            // Act
            prijava.ID = 10;

            // Assert
            Assert.AreEqual(10, prijava.ID);
        }

        [TestMethod]
        public void OglasID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            // Act
            prijava.OglasID = 25;

            // Assert
            Assert.AreEqual(25, prijava.OglasID);
        }

        [TestMethod]
        public void OglasRadnoMjesto_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var oglas = new OglasRadnoMjesto
            {
                OglasID = 25,
                Naslov = "Potreban elektricar"
            };

            // Act
            prijava.OglasRadnoMjesto = oglas;

            // Assert
            Assert.AreSame(
                oglas,
                prijava.OglasRadnoMjesto);
        }

        [TestMethod]
        public void OglasRadnoMjesto_ZamjenaReference_CuvaNoviOglas()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var prviOglas = new OglasRadnoMjesto
            {
                OglasID = 1
            };

            var drugiOglas = new OglasRadnoMjesto
            {
                OglasID = 2
            };

            prijava.OglasRadnoMjesto = prviOglas;

            // Act
            prijava.OglasRadnoMjesto = drugiOglas;

            // Assert
            Assert.AreSame(
                drugiOglas,
                prijava.OglasRadnoMjesto);
        }

        [TestMethod]
        public void MajstorID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            // Act
            prijava.MajstorID = "majstor-123";

            // Assert
            Assert.AreEqual(
                "majstor-123",
                prijava.MajstorID);
        }

        [TestMethod]
        public void Majstor_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var majstor = new Majstor
            {
                Id = "majstor-123"
            };

            // Act
            prijava.Majstor = majstor;

            // Assert
            Assert.AreSame(
                majstor,
                prijava.Majstor);
        }

        [TestMethod]
        public void Majstor_ZamjenaReference_CuvaNovogMajstora()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var prviMajstor = new Majstor
            {
                Id = "majstor-1"
            };

            var drugiMajstor = new Majstor
            {
                Id = "majstor-2"
            };

            prijava.Majstor = prviMajstor;

            // Act
            prijava.Majstor = drugiMajstor;

            // Assert
            Assert.AreSame(
                drugiMajstor,
                prijava.Majstor);
        }

        [TestMethod]
        public void VrijemePrijave_RucnoPostavljanje_CuvaDatum()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var datum = new DateTime(
                2026, 11, 19, 12, 30, 0,
                DateTimeKind.Utc);

            // Act
            prijava.VrijemePrijave = datum;

            // Assert
            Assert.AreEqual(
                datum,
                prijava.VrijemePrijave);
        }

        [TestMethod]
        public void StatusPrijave_PostavljanjeVrijednosti_CuvaStatus()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var noviStatus = (Status)1;

            // Act
            prijava.StatusPrijave = noviStatus;

            // Assert
            Assert.AreEqual(
                noviStatus,
                prijava.StatusPrijave);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void StatusPrijave_RazliciteVrijednosti_CuvaStatus(
            int vrijednost)
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();
            var status = (Status)vrijednost;

            // Act
            prijava.StatusPrijave = status;

            // Assert
            Assert.AreEqual(status, prijava.StatusPrijave);
        }

        [TestMethod]
        public void PrijavaRadnoMjesto_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                OglasID = 15,
                Naslov = "Potreban vodoinstalater"
            };

            var majstor = new Majstor
            {
                Id = "majstor-001"
            };

            var datum = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            var status = Status.NaCekanju;

            // Act
            var prijava = new PrijavaRadnoMjesto
            {
                ID = 1,
                OglasID = 15,
                OglasRadnoMjesto = oglas,
                MajstorID = "majstor-001",
                Majstor = majstor,
                VrijemePrijave = datum,
                StatusPrijave = status
            };

            // Assert
            Assert.AreEqual(1, prijava.ID);
            Assert.AreEqual(15, prijava.OglasID);

            Assert.AreSame(
                oglas,
                prijava.OglasRadnoMjesto);

            Assert.AreEqual(
                "majstor-001",
                prijava.MajstorID);

            Assert.AreSame(
                majstor,
                prijava.Majstor);

            Assert.AreEqual(
                datum,
                prijava.VrijemePrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                prijava.StatusPrijave);
        }
    }
}
