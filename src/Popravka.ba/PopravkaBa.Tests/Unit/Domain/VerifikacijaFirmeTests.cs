
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class VerifikacijaFirmeTests
    {
        // Konkretna implementacija apstraktne klase
        // isključivo za potrebe unit testiranja.
        private sealed class TestnaVerifikacija : VerifikacijaFirme
        {
            public bool PodnesiPozvano { get; private set; }
            public bool ObradiPozvano { get; private set; }
            public bool? Odluka { get; private set; }

            public override void PodnesiVerifikaciju()
            {
                PodnesiPozvano = true;
            }

            public override void ObradiVerifikaciju(bool odobri)
            {
                ObradiPozvano = true;
                Odluka = odobri;
            }
        }

        private static VerifikacijaFirme KreirajVerifikaciju()
        {
            return new TestnaVerifikacija();
        }

        [TestMethod]
        public void NovaVerifikacija_PocetneVrijednostiSuIspravne()
        {
            // Arrange i Act
            var verifikacija = KreirajVerifikaciju();

            // Assert
            Assert.AreEqual(0, verifikacija.VerifikacioniID);
            Assert.AreEqual(
                Status.NaCekanju,
                verifikacija.StatusVerifikacije);

            Assert.AreEqual(string.Empty, verifikacija.JIB);
            Assert.IsNull(verifikacija.PDVBroj);
            Assert.IsNull(verifikacija.DatumObrade);
            Assert.IsNull(verifikacija.RadnoVrijeme);
            Assert.IsNull(verifikacija.WebStranica);
            Assert.IsNull(verifikacija.LicencaDjelatnosti);
        }

        [TestMethod]
        public void NovaVerifikacija_ReferenceNisuPostavljene()
        {
            // Arrange i Act
            var verifikacija = KreirajVerifikaciju();

            // Assert
            Assert.IsNull(verifikacija.Firma);
            Assert.IsNull(verifikacija.FirmaID);
            Assert.IsNull(verifikacija.NazivFirme);
            Assert.IsNull(verifikacija.SjedisteFirme);
        }

        [TestMethod]
        public void DatumPodnosenja_NovaVerifikacija_PostavljaUtcVrijeme()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var verifikacija = KreirajVerifikaciju();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.AreEqual(
                DateTimeKind.Utc,
                verifikacija.DatumPodnosenja.Kind);

            Assert.IsTrue(
                verifikacija.DatumPodnosenja >= prije);

            Assert.IsTrue(
                verifikacija.DatumPodnosenja <= poslije);
        }

        [TestMethod]
        public void VerifikacioniID_PostavljanjeVrijednosti_CuvaID()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.VerifikacioniID = 15;

            Assert.AreEqual(15, verifikacija.VerifikacioniID);
        }

        [TestMethod]
        public void FirmaID_PostavljanjeVrijednosti_CuvaID()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.FirmaID = "firma-123";

            Assert.AreEqual(
                "firma-123",
                verifikacija.FirmaID);
        }

        [TestMethod]
        public void Firma_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var firma = new Firma { Id = "firma-123" };

            // Act
            verifikacija.Firma = firma;

            // Assert
            Assert.AreSame(firma, verifikacija.Firma);
        }

        [TestMethod]
        public void Firma_ZamjenaReference_CuvaNovuFirmu()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var prvaFirma = new Firma { Id = "firma-1" };
            var drugaFirma = new Firma { Id = "firma-2" };

            verifikacija.Firma = prvaFirma;

            // Act
            verifikacija.Firma = drugaFirma;

            // Assert
            Assert.AreSame(drugaFirma, verifikacija.Firma);
        }

        [TestMethod]
        public void NazivFirme_PostavljanjeVrijednosti_CuvaNaziv()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.NazivFirme = "Primjer d.o.o.";

            Assert.AreEqual(
                "Primjer d.o.o.",
                verifikacija.NazivFirme);
        }

        [TestMethod]
        public void StatusVerifikacije_PostavljanjeVrijednosti_CuvaStatus()
        {
            var verifikacija = KreirajVerifikaciju();
            var status = Status.Aktivan;

            verifikacija.StatusVerifikacije = status;

            Assert.AreEqual(status, verifikacija.StatusVerifikacije);
        }

        [TestMethod]
        public void JIB_PostavljanjeVrijednosti_CuvaJIB()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.JIB = "4201234560001";

            Assert.AreEqual(
                "4201234560001",
                verifikacija.JIB);
        }

        [TestMethod]
        public void PDVBroj_PostavljanjeVrijednosti_CuvaBroj()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.PDVBroj = "201234560001";

            Assert.AreEqual(
                "201234560001",
                verifikacija.PDVBroj);
        }

        [TestMethod]
        public void PDVBroj_NullVrijednost_CuvaNull()
        {
            var verifikacija = KreirajVerifikaciju();
            verifikacija.PDVBroj = "201234560001";

            verifikacija.PDVBroj = null;

            Assert.IsNull(verifikacija.PDVBroj);
        }

        [TestMethod]
        public void SjedisteFirme_PostavljanjeVrijednosti_CuvaAdresu()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.SjedisteFirme = "Mostar, BiH";

            Assert.AreEqual(
                "Mostar, BiH",
                verifikacija.SjedisteFirme);
        }

        [TestMethod]
        public void RadnoVrijeme_PostavljanjeVrijednosti_CuvaTekst()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.RadnoVrijeme = "08:00 - 16:00";

            Assert.AreEqual(
                "08:00 - 16:00",
                verifikacija.RadnoVrijeme);
        }

        [TestMethod]
        public void WebStranica_PostavljanjeVrijednosti_CuvaURL()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.WebStranica = "https://primjer.ba";

            Assert.AreEqual(
                "https://primjer.ba",
                verifikacija.WebStranica);
        }

        [TestMethod]
        public void KontaktTelefon_PostavljanjeVrijednosti_CuvaTelefon()
        {
            var verifikacija = KreirajVerifikaciju();

            verifikacija.KontaktTelefon = "+38761123456";

            Assert.AreEqual(
                "+38761123456",
                verifikacija.KontaktTelefon);
        }

        [TestMethod]
        public void OdgovornaOsoba_PodaciSeIspravnoCuvaju()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            verifikacija.OdgovornaOsobaIme = "Amir";
            verifikacija.OdgovornaOsobaPrezime = "Hodžić";
            verifikacija.OdgovornaOsobaEmail = "amir@example.com";
            verifikacija.OdgovornaOsobaPozicija = "Direktor";
            verifikacija.OdgovornaOsobaTelefon = "+38762111222";

            // Assert
            Assert.AreEqual(
                "Amir",
                verifikacija.OdgovornaOsobaIme);
            Assert.AreEqual(
                "Hodžić",
                verifikacija.OdgovornaOsobaPrezime);
            Assert.AreEqual(
                "amir@example.com",
                verifikacija.OdgovornaOsobaEmail);
            Assert.AreEqual(
                "Direktor",
                verifikacija.OdgovornaOsobaPozicija);
            Assert.AreEqual(
                "+38762111222",
                verifikacija.OdgovornaOsobaTelefon);
        }

        [TestMethod]
        public void Dokumentacija_PostavljanjeVrijednosti_CuvaPodatke()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();

            // Act
            verifikacija.Rjesenje = "/docs/rjesenje.pdf";
            verifikacija.PoreznoUvjerenje = "/docs/porezno.pdf";
            verifikacija.LicencaDjelatnosti = "/docs/licenca.pdf";
            verifikacija.Logotip = "/images/logo.png";

            // Assert
            Assert.AreEqual(
                "/docs/rjesenje.pdf",
                verifikacija.Rjesenje);
            Assert.AreEqual(
                "/docs/porezno.pdf",
                verifikacija.PoreznoUvjerenje);
            Assert.AreEqual(
                "/docs/licenca.pdf",
                verifikacija.LicencaDjelatnosti);
            Assert.AreEqual(
                "/images/logo.png",
                verifikacija.Logotip);
        }

        [TestMethod]
        public void LicencaDjelatnosti_NullVrijednost_CuvaNull()
        {
            var verifikacija = KreirajVerifikaciju();
            verifikacija.LicencaDjelatnosti = "/docs/licenca.pdf";

            verifikacija.LicencaDjelatnosti = null;

            Assert.IsNull(verifikacija.LicencaDjelatnosti);
        }

        [TestMethod]
        public void DatumPodnosenja_RucnoPostavljanje_CuvaDatum()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var datum = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            // Act
            verifikacija.DatumPodnosenja = datum;

            // Assert
            Assert.AreEqual(
                datum,
                verifikacija.DatumPodnosenja);
        }

        [TestMethod]
        public void DatumObrade_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju();
            var datum = new DateTime(
                2026, 11, 20, 12, 0, 0,
                DateTimeKind.Utc);

            // Act
            verifikacija.DatumObrade = datum;

            // Assert
            Assert.IsNotNull(verifikacija.DatumObrade);
            Assert.AreEqual(
                datum,
                verifikacija.DatumObrade.Value);
        }

        [TestMethod]
        public void DatumObrade_NullVrijednost_CuvaNull()
        {
            // Arrange
            var verifikacija = KreirajVerifikaciju
            ();
            verifikacija.DatumObrade = DateTime.UtcNow;

            // Act
            verifikacija.DatumObrade = null;

            // Assert
            Assert.IsNull(verifikacija.DatumObrade);
        }

        [TestMethod]
        public void PodnesiVerifikaciju_PozivKrozBaznuKlasu_PozivaImplementaciju()
        {
            // Arrange
            var testna = new TestnaVerifikacija();
            VerifikacijaFirme verifikacija = testna;

            // Act
            verifikacija.PodnesiVerifikaciju();

            // Assert
            Assert.IsTrue(testna.PodnesiPozvano);
        }

        [TestMethod]
        public void ObradiVerifikaciju_OdobriTrue_ProsljedjujeOdluku()
        {
            // Arrange
            var testna = new TestnaVerifikacija();
            VerifikacijaFirme verifikacija = testna;

            // Act
            verifikacija.ObradiVerifikaciju(true);

            // Assert
            Assert.IsTrue(testna.ObradiPozvano);
            Assert.AreEqual(true, testna.Odluka);
        }

        [TestMethod]
        public void ObradiVerifikaciju_OdobriFalse_ProsljedjujeOdluku()
        {
            // Arrange
            var testna = new TestnaVerifikacija();
            VerifikacijaFirme verifikacija = testna;

            // Act
            verifikacija.ObradiVerifikaciju(false);

            // Assert
            Assert.IsTrue(testna.ObradiPozvano);
            Assert.AreEqual(false, testna.Odluka);
        }

        [TestMethod]
        public void VerifikacijaFirme_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var firma = new Firma { Id = "firma-001" };

            var datumPodnosenja = new DateTime(
                2026, 11, 19, 8, 0, 0,
                DateTimeKind.Utc);

            var datumObrade = new DateTime(
                2026, 11, 20, 14, 0, 0,
                DateTimeKind.Utc);

            // Act
            VerifikacijaFirme verifikacija =
                new TestnaVerifikacija
                {
                    VerifikacioniID = 1,
                    FirmaID = "firma-001",
                    Firma = firma,
                    NazivFirme = "Primjer d.o.o.",
                    StatusVerifikacije = Status.NaCekanju,
                    JIB = "4201234560001",
                    PDVBroj = "201234560001",
                    SjedisteFirme = "Mostar",
                    RadnoVrijeme = "08:00 - 16:00",
                    WebStranica = "https://primjer.ba",
                    KontaktTelefon = "+38761123456",
                    OdgovornaOsobaIme = "Amir",
                    OdgovornaOsobaPrezime = "Hodžić",
                    OdgovornaOsobaEmail = "amir@example.com",
                    OdgovornaOsobaPozicija = "Direktor",
                    OdgovornaOsobaTelefon = "+38762111222",
                    Rjesenje = "/docs/rjesenje.pdf",
                    PoreznoUvjerenje = "/docs/porezno.pdf",
                    LicencaDjelatnosti = "/docs/licenca.pdf",
                    Logotip = "/images/logo.png",
                    DatumPodnosenja = datumPodnosenja,
                    DatumObrade = datumObrade
                };

            // Assert
            Assert.AreEqual(1, verifikacija.VerifikacioniID);
            Assert.AreEqual("firma-001", verifikacija.FirmaID);
            Assert.AreSame(firma, verifikacija.Firma);
            Assert.AreEqual("Primjer d.o.o.", verifikacija.NazivFirme);
            Assert.AreEqual(
                Status.NaCekanju,
                verifikacija.StatusVerifikacije);
            Assert.AreEqual("4201234560001", verifikacija.JIB);
            Assert.AreEqual("201234560001", verifikacija.PDVBroj);
            Assert.AreEqual("Mostar", verifikacija.SjedisteFirme);
            Assert.AreEqual("08:00 - 16:00", verifikacija.RadnoVrijeme);
            Assert.AreEqual("https://primjer.ba", verifikacija.WebStranica);
            Assert.AreEqual("+38761123456", verifikacija.KontaktTelefon);
            Assert.AreEqual("Amir", verifikacija.OdgovornaOsobaIme);
            Assert.AreEqual("Hodžić", verifikacija.OdgovornaOsobaPrezime);
            Assert.AreEqual(
                "amir@example.com",
                verifikacija.OdgovornaOsobaEmail);
            Assert.AreEqual(
                "Direktor",
                verifikacija.OdgovornaOsobaPozicija);
            Assert.AreEqual(
                "+38762111222",
                verifikacija.OdgovornaOsobaTelefon);
            Assert.AreEqual("/docs/rjesenje.pdf", verifikacija.Rjesenje);
            Assert.AreEqual(
                "/docs/porezno.pdf",
                verifikacija.PoreznoUvjerenje);
            Assert.AreEqual(
                "/docs/licenca.pdf",
                verifikacija.LicencaDjelatnosti);
            Assert.AreEqual("/images/logo.png", verifikacija.Logotip);
            Assert.AreEqual(
                datumPodnosenja,
                verifikacija.DatumPodnosenja);
            Assert.AreEqual(
                datumObrade,
                verifikacija.DatumObrade);
        }
    }
}
