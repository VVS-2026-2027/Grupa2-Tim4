
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class PonudaUslugeTests
    {
        [TestMethod]
        public void NovaPonuda_PodrazumijevaneVrijednostiSuIspravne()
        {
            // Arrange i Act
            var ponuda = new PonudaUsluge();

            // Assert
            Assert.AreEqual(0, ponuda.ID);
            Assert.AreEqual(0, ponuda.OglasUslugeID);
            Assert.AreEqual(default(DateTime),
                ponuda.DatumPocetkaUsluge);

            Assert.AreEqual(Status.NaCekanju,
                ponuda.StatusPonude);

            Assert.AreEqual(TipIsplate.PoSatu,
                ponuda.TipIsplate);
        }

        [TestMethod]
        public void NovaPonuda_OpcionalnaSvojstvaSuNull()
        {
            // Arrange i Act
            var ponuda = new PonudaUsluge();

            // Assert
            Assert.IsNull(ponuda.IzvrsilacID);
            Assert.IsNull(ponuda.Izvrsilac);
            Assert.IsNull(ponuda.OglasUsluge);
            Assert.IsNull(ponuda.DatumOcekivanogZavrsetka);
            Assert.IsNull(ponuda.DatumIzvrsavanjaUsluge);
            Assert.IsNull(ponuda.Cijena);
            Assert.IsNull(ponuda.Poruka);
        }

        [TestMethod]
        public void DatumSlanja_NovaPonuda_PostavljaTrenutniUtcDatum()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var ponuda = new PonudaUsluge();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(ponuda.DatumSlanja >= prije);
            Assert.IsTrue(ponuda.DatumSlanja <= poslije);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.ID = 10;

            // Assert
            Assert.AreEqual(10, ponuda.ID);
        }

        [TestMethod]
        public void IzvrsilacID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.IzvrsilacID = "majstor-123";

            // Assert
            Assert.AreEqual(
                "majstor-123",
                ponuda.IzvrsilacID);
        }

        [TestMethod]
        public void Izvrsilac_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            IzvrsilacUsluge izvrsilac = new Majstor
            {
                Id = "majstor-123"
            };

            // Act
            ponuda.Izvrsilac = izvrsilac;

            // Assert
            Assert.AreSame(
                izvrsilac,
                ponuda.Izvrsilac);
        }

        [TestMethod]
        public void OglasUslugeID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.OglasUslugeID = 25;

            // Assert
            Assert.AreEqual(25, ponuda.OglasUslugeID);
        }

        [TestMethod]
        public void OglasUsluge_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            var oglas = new OglasUsluge
            {
                OglasID = 25,
                Naslov = "Popravka bojlera"
            };

            // Act
            ponuda.OglasUsluge = oglas;

            // Assert
            Assert.AreSame(
                oglas,
                ponuda.OglasUsluge);
        }

        [TestMethod]
        public void DatumSlanja_RucnoPostavljanje_CuvaDatum()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            var datum = new DateTime(
                2026, 11, 19, 12, 30, 0,
                DateTimeKind.Utc);

            // Act
            ponuda.DatumSlanja = datum;

            // Assert
            Assert.AreEqual(datum, ponuda.DatumSlanja);
        }

        [TestMethod]
        public void DatumPocetkaUsluge_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            var datum = new DateTime(
                2026, 12, 1, 8, 0, 0,
                DateTimeKind.Utc);

            // Act
            ponuda.DatumPocetkaUsluge = datum;

            // Assert
            Assert.AreEqual(
                datum,
                ponuda.DatumPocetkaUsluge);
        }

        [TestMethod]
        public void DatumOcekivanogZavrsetka_PostavljanjeDatuma_CuvaVrijednost()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            var datum = new DateTime(
                2026, 12, 5, 17, 0, 0,
                DateTimeKind.Utc);

            // Act
            ponuda.DatumOcekivanogZavrsetka = datum;

            // Assert
            Assert.AreEqual(
                datum,
                ponuda.DatumOcekivanogZavrsetka);
        }

        [TestMethod]
        public void DatumOcekivanogZavrsetka_NullVrijednost_CuvaNull()
        {
            // Arrange
            var ponuda = new PonudaUsluge
            {
                DatumOcekivanogZavrsetka = DateTime.UtcNow
            };

            // Act
            ponuda.DatumOcekivanogZavrsetka = null;

            // Assert
            Assert.IsNull(ponuda.DatumOcekivanogZavrsetka);
        }

        [TestMethod]
        public void DatumIzvrsavanjaUsluge_PostavljanjeDatuma_CuvaVrijednost()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            var datum = new DateTime(
                2026, 12, 4, 15, 0, 0,
                DateTimeKind.Utc);

            // Act
            ponuda.DatumIzvrsavanjaUsluge = datum;

            // Assert
            Assert.AreEqual(
                datum,
                ponuda.DatumIzvrsavanjaUsluge);
        }

        [TestMethod]
        public void DatumIzvrsavanjaUsluge_NullVrijednost_CuvaNull()
        {
            // Arrange
            var ponuda = new PonudaUsluge
            {
                DatumIzvrsavanjaUsluge = DateTime.UtcNow
            };

            // Act
            ponuda.DatumIzvrsavanjaUsluge = null;

            // Assert
            Assert.IsNull(ponuda.DatumIzvrsavanjaUsluge);
        }

        [TestMethod]
        public void StatusPonude_PostavljanjeVrijednosti_CuvaStatus()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.StatusPonude = Status.Prihvaceno;

            // Assert
            Assert.AreEqual(
                Status.Prihvaceno,
                ponuda.StatusPonude);
        }

        [DataTestMethod]
        [DataRow(Status.NaCekanju)]
        [DataRow(Status.Prihvaceno)]
        [DataRow(Status.Odbijeno)]
        public void StatusPonude_RazlicitiStatusi_CuvaVrijednost(
            Status status)
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.StatusPonude = status;

            // Assert
            Assert.AreEqual(status, ponuda.StatusPonude);
        }

        [TestMethod]
        public void Cijena_PostavljanjeVrijednosti_CuvaCijenu()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.Cijena = 150;

            // Assert
            Assert.AreEqual(150, ponuda.Cijena);
        }

        [TestMethod]
        public void Cijena_NullVrijednost_CuvaNull()
        {
            // Arrange
            var ponuda = new PonudaUsluge
            {
                Cijena = 200
            };

            // Act
            ponuda.Cijena = null;

            // Assert
            Assert.IsNull(ponuda.Cijena);
        }

        [TestMethod]
        public void TipIsplate_NovaPonuda_PodrazumijevanaVrijednostJePoSatu()
        {
            // Arrange i Act
            var ponuda = new PonudaUsluge();

            // Assert
            Assert.AreEqual(
                TipIsplate.PoSatu,
                ponuda.TipIsplate);
        }

        [TestMethod]
        public void TipIsplate_PostavljanjeVrijednosti_CuvaTip()
        {
            // Arrange
            var ponuda = new PonudaUsluge();
            var tip = (TipIsplate)1;

            // Act
            ponuda.TipIsplate = tip;

            // Assert
            Assert.AreEqual(tip, ponuda.TipIsplate);
        }

        [TestMethod]
        public void Poruka_PostavljanjeVrijednosti_CuvaTekst()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.Poruka =
                "Mogu završiti posao za tri dana.";

            // Assert
            Assert.AreEqual(
                "Mogu završiti posao za tri dana.",
                ponuda.Poruka);
        }

        [TestMethod]
        public void Poruka_PrazanString_CuvaPraznuVrijednost()
        {
            // Arrange
            var ponuda = new PonudaUsluge();

            // Act
            ponuda.Poruka = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, ponuda.Poruka);
        }

        [TestMethod]
        public void PonudaUsluge_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var izvrsilac = new Majstor
            {
                Id = "majstor-001"
            };

            var oglas = new OglasUsluge
            {
                OglasID = 15,
                Naslov = "Popravka slavine"
            };

            var datumSlanja = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            var datumPocetka = new DateTime(
                2026, 11, 20, 8, 0, 0,
                DateTimeKind.Utc);

            var datumZavrsetka = new DateTime(
                2026, 11, 22, 17, 0, 0,
                DateTimeKind.Utc);

            // Act
            var ponuda = new PonudaUsluge
            {
                ID = 1,
                IzvrsilacID = "majstor-001",
                Izvrsilac = izvrsilac,
                OglasUslugeID = 15,
                OglasUsluge = oglas,
                DatumSlanja = datumSlanja,
                DatumPocetkaUsluge = datumPocetka,
                DatumOcekivanogZavrsetka = datumZavrsetka,
                DatumIzvrsavanjaUsluge = null,
                StatusPonude = Status.NaCekanju,
                Cijena = 250,
                TipIsplate = TipIsplate.PoSatu,
                Poruka = "Mogu izvršiti popravku."
            };

            // Assert
            Assert.AreEqual(1, ponuda.ID);
            Assert.AreEqual("majstor-001", ponuda.IzvrsilacID);
            Assert.AreSame(izvrsilac, ponuda.Izvrsilac);
            Assert.AreEqual(15, ponuda.OglasUslugeID);
            Assert.AreSame(oglas, ponuda.OglasUsluge);
            Assert.AreEqual(datumSlanja, ponuda.DatumSlanja);
            Assert.AreEqual(
                datumPocetka,
                ponuda.DatumPocetkaUsluge);
            Assert.AreEqual(
                datumZavrsetka,
                ponuda.DatumOcekivanogZavrsetka);
            Assert.IsNull(ponuda.DatumIzvrsavanjaUsluge);
            Assert.AreEqual(
                Status.NaCekanju,
                ponuda.StatusPonude);
            Assert.AreEqual(250, ponuda.Cijena);
            Assert.AreEqual(
                TipIsplate.PoSatu,
                ponuda.TipIsplate);
            Assert.AreEqual(
                "Mogu izvršiti popravku.",
                ponuda.Poruka);
        }
    }
}
