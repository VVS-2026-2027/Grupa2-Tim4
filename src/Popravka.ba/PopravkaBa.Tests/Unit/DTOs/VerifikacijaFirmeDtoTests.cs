using PopravkaBa.Application.DTOs;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class PodnesiVerifikacijuDtoTests
    {
        private static PodnesiVerifikacijuDto ValidanDto() => new()
        {
            NazivFirme = "Popravka d.o.o.",
            JIB = "1234567890123",
            PDVBroj = "AB123456789012",
            SjedisteFirme = "Ferhadija 1, Sarajevo",
            KontaktTelefon = "033 123 456",
            OdgovornaOsobaIme = "Amina",
            OdgovornaOsobaPrezime = "Hadzic",
            OdgovornaOsobaEmail = "amina@example.com",
            OdgovornaOsobaPozicija = "Direktor",
            OdgovornaOsobaTelefon = "061 123 456",
            Rjesenje = "dokumenti/rjesenje.pdf",
            PoreznoUvjerenje = "dokumenti/porezno-uvjerenje.pdf",
            LicencaDjelatnosti = "dokumenti/licenca.pdf",
            Logotip = "https://cdn.example.com/logo.png",
            RadnoVrijeme = "08:00-16:00",
            WebStranica = "https://popravka.ba"
        };

        [TestMethod]
        public void NoviDto_OpcijaPoljaSuNull()
        {
            // Akcija
            var dto = new PodnesiVerifikacijuDto();

            // Provjera
            Assert.IsNull(dto.PDVBroj);
            Assert.IsNull(dto.LicencaDjelatnosti);
            Assert.IsNull(dto.Logotip);
            Assert.IsNull(dto.RadnoVrijeme);
            Assert.IsNull(dto.WebStranica);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual("Popravka d.o.o.", dto.NazivFirme);
            Assert.AreEqual("1234567890123", dto.JIB);
            Assert.AreEqual("AB123456789012", dto.PDVBroj);
            Assert.AreEqual("Ferhadija 1, Sarajevo", dto.SjedisteFirme);
            Assert.AreEqual("033 123 456", dto.KontaktTelefon);
            Assert.AreEqual("Amina", dto.OdgovornaOsobaIme);
            Assert.AreEqual("Hadzic", dto.OdgovornaOsobaPrezime);
            Assert.AreEqual("amina@example.com", dto.OdgovornaOsobaEmail);
            Assert.AreEqual("Direktor", dto.OdgovornaOsobaPozicija);
            Assert.AreEqual("061 123 456", dto.OdgovornaOsobaTelefon);
            Assert.AreEqual("dokumenti/rjesenje.pdf", dto.Rjesenje);
            Assert.AreEqual("dokumenti/porezno-uvjerenje.pdf", dto.PoreznoUvjerenje);
            Assert.AreEqual("dokumenti/licenca.pdf", dto.LicencaDjelatnosti);
            Assert.AreEqual("https://cdn.example.com/logo.png", dto.Logotip);
            Assert.AreEqual("08:00-16:00", dto.RadnoVrijeme);
            Assert.AreEqual("https://popravka.ba", dto.WebStranica);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }

        [DataTestMethod]
        [DataRow("1234567890123", true)]
        [DataRow("123456789012", false)]
        [DataRow("12345678901234", false)]
        [DataRow("12345678901a3", false)]
        public void JIB_PostujePraviloTrinaestCifara(string jib, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.JIB = jib;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow("AB123456789012", true)]
        [DataRow("123456789012", true)]
        [DataRow("ab123456789012", true)]
        [DataRow("ABC123456789012", false)]
        [DataRow("AB12345678901", false)]
        [DataRow(null, true)]
        public void PDVBroj_PostujePravilo(string? pdvBroj, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.PDVBroj = pdvBroj;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow("https://popravka.ba", true)]
        [DataRow("nije url", false)]
        [DataRow(null, true)]
        public void WebStranica_PostujeUrlPravilo(string? webStranica, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.WebStranica = webStranica;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NeispravanEmailOdgovorneOsobe_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.OdgovornaOsobaEmail = "amina@";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(
                rezultati,
                nameof(PodnesiVerifikacijuDto.OdgovornaOsobaEmail)));
        }

        [DataTestMethod]
        [DataRow("abc", false)]
        [DataRow("061 123 456", true)]
        public void KontaktTelefon_PostujePravilo(string telefon, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.KontaktTelefon = telefon;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void BezObaveznihPolja_VracaGreskeZaSvaObaveznaPolja()
        {
            // Priprema
            var dto = new PodnesiVerifikacijuDto
            {
                NazivFirme = null!,
                JIB = null!,
                SjedisteFirme = null!,
                KontaktTelefon = null!,
                OdgovornaOsobaIme = null!,
                OdgovornaOsobaPrezime = null!,
                OdgovornaOsobaEmail = null!,
                OdgovornaOsobaPozicija = null!,
                OdgovornaOsobaTelefon = null!,
                Rjesenje = null!,
                PoreznoUvjerenje = null!
            };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(11, rezultati.Count);
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(PodnesiVerifikacijuDto.JIB)));
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(PodnesiVerifikacijuDto.Rjesenje)));
        }
    }

    [TestClass]
    public class ObradiVerifikacijuDtoTests
    {
        [TestMethod]
        public void NoviDto_ImaZadaneVrijednosti()
        {
            // Akcija
            var dto = new ObradiVerifikacijuDto();

            // Provjera
            Assert.AreEqual(0, dto.VerifikacioniID);
            Assert.IsFalse(dto.Odobri);
            Assert.IsNull(dto.Napomena);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PostavljeneVrijednosti_SeVracaju(bool odobri)
        {
            // Priprema
            var dto = new ObradiVerifikacijuDto
            {
                VerifikacioniID = 4,
                Odobri = odobri,
                Napomena = "Napomena uz odluku."
            };

            // Akcija i provjera
            Assert.AreEqual(4, dto.VerifikacioniID);
            Assert.AreEqual(odobri, dto.Odobri);
            Assert.AreEqual("Napomena uz odluku.", dto.Napomena);
        }
    }
}
