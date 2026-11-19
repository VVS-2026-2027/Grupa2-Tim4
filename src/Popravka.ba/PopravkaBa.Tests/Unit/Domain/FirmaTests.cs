
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class FirmaTests
    {
        [TestMethod]
        public void NovaFirma_PodrazumijevanaVelicinaJeMikro()
        {
            // Arrange i Act
            var firma = new Firma();

            // Assert
            Assert.AreEqual(
                VelicinaFirme.Mikro,
                firma.VelicinaFirme);
        }

        [TestMethod]
        public void NovaFirma_PodrazumijevaniRasponZaposlenihJeOd1Do9()
        {
            // Arrange i Act
            var firma = new Firma();

            // Assert
            Assert.AreEqual(1, firma.MinZaposlenih);
            Assert.AreEqual(9, firma.MaxZaposlenih);
        }

        [TestMethod]
        public void NovaFirma_AdminVerificiraoJeFalse()
        {
            // Arrange i Act
            var firma = new Firma();

            // Assert
            Assert.IsFalse(firma.AdminVerificirao);
        }

        [TestMethod]
        public void DisplayName_PostojiNazivFirme_VracaNaziv()
        {
            // Arrange
            var firma = new Firma
            {
                NazivFirme = "Popravke Sarajevo"
            };

            // Act
            var rezultat = firma.DisplayName;

            // Assert
            Assert.AreEqual(
                "Popravke Sarajevo",
                rezultat);
        }

        [TestMethod]
        public void SkracenoIme_PostojiNazivFirme_VracaNaziv()
        {
            // Arrange
            var firma = new Firma
            {
                NazivFirme = "Popravke Sarajevo"
            };

            // Act
            var rezultat = firma.SkracenoIme;

            // Assert
            Assert.AreEqual(
                "Popravke Sarajevo",
                rezultat);
        }

        [DataTestMethod]
        [DataRow(VelicinaFirme.Mikro, 1, 9)]
        [DataRow(VelicinaFirme.Mala, 10, 50)]
        [DataRow(VelicinaFirme.Srednja, 51, 100)]
        [DataRow(VelicinaFirme.Velika, 100, 0)]
        public void VelicinaFirme_PostavljenaKategorija_PostavljaRasponZaposlenih(
            VelicinaFirme kategorija,
            int ocekivaniMinimum,
            int ocekivaniMaksimum)
        {
            // Arrange
            var firma = new Firma();

            // Act
            firma.VelicinaFirme = kategorija;

            // Assert
            Assert.AreEqual(kategorija, firma.VelicinaFirme);
            Assert.AreEqual(
                ocekivaniMinimum,
                firma.MinZaposlenih);
            Assert.AreEqual(
                ocekivaniMaksimum,
                firma.MaxZaposlenih);
        }

        [TestMethod]
        public void VelicinaFirme_NepoznataKategorija_PostavljaPodrazumijevaniRaspon()
        {
            // Arrange
            var firma = new Firma();

            // Act
            firma.VelicinaFirme = (VelicinaFirme)999;

            // Assert
            Assert.AreEqual(1, firma.MinZaposlenih);
            Assert.AreEqual(9, firma.MaxZaposlenih);
        }

        [DataTestMethod]
        [DataRow(true, true, Status.Aktivan)]
        [DataRow(true, false, Status.NaCekanju)]
        [DataRow(false, true, Status.NaCekanju)]
        [DataRow(false, false, Status.NaCekanju)]
        public void Aktivan_KombinacijaPotvrda_VracaOcekivaniStatus(
            bool emailConfirmed,
            bool adminVerificirao,
            Status ocekivaniStatus)
        {
            // Arrange
            var firma = new Firma
            {
                EmailConfirmed = emailConfirmed,
                AdminVerificirao = adminVerificirao
            };

            // Act
            var rezultat = firma.Aktivan();

            // Assert
            Assert.AreEqual(ocekivaniStatus, rezultat);
        }

        [TestMethod]
        public void RadnoVrijeme_PostavljanjeVrijednosti_CuvaVrijednosti()
        {
            // Arrange
            var firma = new Firma();
            var otvorenoOd = new TimeSpan(8, 0, 0);
            var otvorenoDo = new TimeSpan(17, 0, 0);

            // Act
            firma.OtvorenoOd = otvorenoOd;
            firma.OtvorenoDo = otvorenoDo;
            firma.RadnoVrijeme = "08:00-17:00";

            // Assert
            Assert.AreEqual(otvorenoOd, firma.OtvorenoOd);
            Assert.AreEqual(otvorenoDo, firma.OtvorenoDo);
            Assert.AreEqual(
                "08:00-17:00",
                firma.RadnoVrijeme);
        }

        [TestMethod]
        public void WebStranica_PostavljanjeVrijednosti_CuvaUrl()
        {
            // Arrange
            var firma = new Firma();

            // Act
            firma.WebStranica = "https://firma.ba";

            // Assert
            Assert.AreEqual(
                "https://firma.ba",
                firma.WebStranica);
        }

        [TestMethod]
        public void DatumOsnivanja_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var firma = new Firma();
            var datum = new DateOnly(2015, 5, 20);

            // Act
            firma.DatumOsnivanja = datum;

            // Assert
            Assert.AreEqual(
                datum,
                firma.DatumOsnivanja);
        }

        [TestMethod]
        public void ZahtjeviVerifikacije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var firma = new Firma();
            var zahtjevi = new List<VerifikacijaFirme>();

            // Act
            firma.ZahtjeviVerifikacije = zahtjevi;

            // Assert
            Assert.AreSame(
                zahtjevi,
                firma.ZahtjeviVerifikacije);
        }

        [TestMethod]
        public void MinMaxZaposlenih_RucnoPostavljanje_CuvaVrijednosti()
        {
            // Arrange
            var firma = new Firma();

            // Act
            firma.MinZaposlenih = 20;
            firma.MaxZaposlenih = 40;

            // Assert
            Assert.AreEqual(20, firma.MinZaposlenih);
            Assert.AreEqual(40, firma.MaxZaposlenih);
        }

        [TestMethod]
        public void VelicinaFirme_PromjenaKategorije_AzuriraRaspon()
        {
            // Arrange
            var firma = new Firma
            {
                VelicinaFirme = VelicinaFirme.Velika
            };

            // Act
            firma.VelicinaFirme = VelicinaFirme.Mala;

            // Assert
            Assert.AreEqual(10, firma.MinZaposlenih);
            Assert.AreEqual(50, firma.MaxZaposlenih);
        }
    }
}
