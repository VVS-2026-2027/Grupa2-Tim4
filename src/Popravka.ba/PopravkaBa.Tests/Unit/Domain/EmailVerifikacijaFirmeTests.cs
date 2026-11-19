
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class EmailVerifikacijaFirmeTests
    {
        private static EmailVerifikacijaFirme KreirajVerifikaciju()
        {
            return new EmailVerifikacijaFirme
            {
                AdminEmail = "admin@popravka.ba",
                NazivFirme = "Test Firma",
                JIB = "1234567890123",
                PDVBroj = "123456789012",
                SjedisteFirme = "Sarajevo",
                KontaktTelefon = "033123456",
                WebStranica = "https://firma.ba",
                OdgovornaOsobaIme = "Emir",
                OdgovornaOsobaPrezime = "Hodzic",
                OdgovornaOsobaPozicija = "Direktor",
                OdgovornaOsobaEmail = "emir@firma.ba",
                OdgovornaOsobaTelefon = "061123456"
            };
        }

        [TestMethod]
        public void NoviObjekat_AdminEmailJePrazanString()
        {
            // Arrange i Act
            var verifikacija = new EmailVerifikacijaFirme();

            // Assert
            Assert.AreEqual(string.Empty, verifikacija.AdminEmail);
        }

        [TestMethod]
        public void AdminEmail_PostavljanjeVrijednosti_CuvaEmail()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var email = verifikacija.AdminEmail;

            // Assert
            Assert.AreEqual("admin@popravka.ba", email);
        }

        [TestMethod]
        public void PodnesiVerifikaciju_PozvanaMetoda_StatusNaCekanju()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            verifikacija.PodnesiVerifikaciju();

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                verifikacija.StatusVerifikacije);
        }

        [TestMethod]
        public void PodnesiVerifikaciju_PozvanaMetoda_PostavljaTrenutniDatum()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var prije = DateTime.UtcNow;

            // Act
            verifikacija.PodnesiVerifikaciju();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(verifikacija.DatumPodnosenja >= prije);
            Assert.IsTrue(verifikacija.DatumPodnosenja <= poslije);
        }

        [DataTestMethod]
        [DataRow(true, Status.Prihvaceno)]
        [DataRow(false, Status.Odbijeno)]
        public void ObradiVerifikaciju_Odluka_PostavljaOcekivaniStatus(
            bool odobri,
            Status ocekivaniStatus)
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            verifikacija.ObradiVerifikaciju(odobri);

            // Assert
            Assert.AreEqual(
                ocekivaniStatus,
                verifikacija.StatusVerifikacije);
        }

        [TestMethod]
        public void ObradiVerifikaciju_PozvanaMetoda_PostavljaDatumObrade()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var prije = DateTime.UtcNow;

            // Act
            verifikacija.ObradiVerifikaciju(true);
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(verifikacija.DatumObrade);
            Assert.IsTrue(verifikacija.DatumObrade >= prije);
            Assert.IsTrue(verifikacija.DatumObrade <= poslije);
        }

        [TestMethod]
        public void FormatIntoEmail_ZaAdmina_SadrziPodatkeFirme()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.PodnesiVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            StringAssert.Contains(
                html, "Novi zahtjev za verifikaciju firme");
            StringAssert.Contains(html, "Test Firma");
            StringAssert.Contains(html, "1234567890123");
            StringAssert.Contains(html, "Sarajevo");
            StringAssert.Contains(html, "033123456");
            StringAssert.Contains(html, "Emir Hodzic");
            StringAssert.Contains(html, "Direktor");
            StringAssert.Contains(html, "emir@firma.ba");
            StringAssert.Contains(html, "061123456");
        }

        [TestMethod]
        public void FormatIntoEmail_PostojiPdvIWeb_PrikazujeObaPolja()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            StringAssert.Contains(html, "PDV broj");
            StringAssert.Contains(html, "123456789012");
            StringAssert.Contains(html, "Web stranica");
            StringAssert.Contains(html, "https://firma.ba");
        }

        [TestMethod]
        public void FormatIntoEmail_NemaPdvIWeb_IzostavljaPolja()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.PDVBroj = null;
            verifikacija.WebStranica = null;

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            Assert.IsFalse(html.Contains("PDV broj"));
            Assert.IsFalse(html.Contains("Web stranica"));
        }

        [TestMethod]
        public void FormatIntoEmail_PrazniPdvIWeb_IzostavljaPolja()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.PDVBroj = "   ";
            verifikacija.WebStranica = "";

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            Assert.IsFalse(html.Contains("PDV broj"));
            Assert.IsFalse(html.Contains("Web stranica"));
        }

        [TestMethod]
        public void FormatIntoEmail_SviDokumenti_PrikazujeLinkove()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(
                true,
                "https://dokumenti.ba/rjesenje.pdf",
                "https://dokumenti.ba/porezno.pdf",
                "https://dokumenti.ba/licenca.pdf",
                "https://popravka.ba/admin/verifikacije/1");

            // Assert
            StringAssert.Contains(
                html, "https://dokumenti.ba/rjesenje.pdf");
            StringAssert.Contains(
                html, "https://dokumenti.ba/porezno.pdf");
            StringAssert.Contains(
                html, "https://dokumenti.ba/licenca.pdf");
            StringAssert.Contains(
                html, "Pregled i odobrenje zahtjeva");
            StringAssert.Contains(
                html, "https://popravka.ba/admin/verifikacije/1");
        }

        [TestMethod]
        public void FormatIntoEmail_BezDokumenata_PrikazujeOznake()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            StringAssert.Contains(html, "Rješenje o registraciji");
            StringAssert.Contains(
                html, "Uvjerenje o poreznoj registraciji");
            StringAssert.Contains(html, "nije priloženo");
            StringAssert.Contains(html, "—");

            Assert.IsFalse(
                html.Contains("Pregled i odobrenje zahtjeva"));
        }

        [TestMethod]
        public void FormatIntoEmail_PrazniUrlovi_NePrikazujeLinkove()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(
                true, "", "", "", "");

            // Assert
            StringAssert.Contains(html, "nije priloženo");

            Assert.IsFalse(
                html.Contains("Pregled i odobrenje zahtjeva"));
            Assert.IsFalse(html.Contains("Otvori dokument"));
        }

        [TestMethod]
        public void FormatIntoEmail_PrihvacenaVerifikacija_SadrziPoruku()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.ObradiVerifikaciju(true);

            // Act
            var html = verifikacija.FormatIntoEmail(false);

            // Assert
            StringAssert.Contains(
                html, "Čestitamo! Vaša firma je verificirana.");
            StringAssert.Contains(
                html, "Vaša firma je uspješno verificirana");
            StringAssert.Contains(html, "Posjetite vaš profil");
            StringAssert.Contains(
                html, "https://popravka.ba/profil");
            StringAssert.Contains(html, "#065f46");
            StringAssert.Contains(html, "✔");
        }

        [TestMethod]
        public void FormatIntoEmail_OdbijenaVerifikacija_SadrziPoruku()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.ObradiVerifikaciju(false);

            // Act
            var html = verifikacija.FormatIntoEmail(false);

            // Assert
            StringAssert.Contains(
                html, "Zahtjev za verifikaciju je odbijen.");
            StringAssert.Contains(
                html, "Nažalost, vaš zahtjev za verifikaciju");
            StringAssert.Contains(html, "#991b1b");
            StringAssert.Contains(html, "✖");

            Assert.IsFalse(
                html.Contains("Posjetite vaš profil"));
        }

        [TestMethod]
        public void FormatIntoEmail_Prihvaceno_SadrziDatumObrade()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.ObradiVerifikaciju(true);

            // Act
            var html = verifikacija.FormatIntoEmail(false);

            // Assert
            Assert.IsNotNull(verifikacija.DatumObrade);

            StringAssert.Contains(
                html,
                verifikacija.DatumObrade.Value.ToString("dd.MM.yyyy."));
        }

        [TestMethod]
        public void FormatIntoEmail_ZaAdmina_GeneriseHtmlDokument()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            var html = verifikacija.FormatIntoEmail(true);

            // Assert
            StringAssert.Contains(html, "<!DOCTYPE html>");
            StringAssert.Contains(html, "<html lang=\"bs\">");
            StringAssert.Contains(html, "</html>");
            StringAssert.Contains(html, "Popravka.ba");
            StringAssert.Contains(
                html, "Administratorska obavijest");
        }

        [TestMethod]
        public void FormatIntoEmail_ZaFirmu_GeneriseHtmlDokument()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            verifikacija.ObradiVerifikaciju(false);

            // Act
            var html = verifikacija.FormatIntoEmail(false);

            // Assert
            StringAssert.Contains(html, "<!DOCTYPE html>");
            StringAssert.Contains(html, "<html lang=\"bs\">");
            StringAssert.Contains(html, "</html>");
            StringAssert.Contains(
                html, "Obavijest o verifikaciji");
        }
    }
}
