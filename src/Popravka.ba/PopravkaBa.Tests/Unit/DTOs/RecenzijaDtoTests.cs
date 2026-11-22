using PopravkaBa.Application.DTOs;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class KreirajRecenzijuDtoTests
    {
        private static KreirajRecenzijuDto ValidanDto() => new()
        {
            IzvrsilacID = "majstor-1",
            Ocjena = 5,
            Komentar = "Odlicna usluga, sve preporuke."
        };

        [TestMethod]
        public void NoviDto_ImaPrazanKomentarINultuOcjenu()
        {
            // Akcija
            var dto = new KreirajRecenzijuDto();

            // Provjera
            Assert.AreEqual(string.Empty, dto.Komentar);
            Assert.AreEqual(0, dto.Ocjena);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual("majstor-1", dto.IzvrsilacID);
            Assert.AreEqual(5, dto.Ocjena);
            Assert.AreEqual("Odlicna usluga, sve preporuke.", dto.Komentar);
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
        [DataRow(0, false)]
        [DataRow(1, true)]
        [DataRow(5, true)]
        [DataRow(6, false)]
        public void Ocjena_PostujeOpseg(int ocjena, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Ocjena = ocjena;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow(9, false)]
        [DataRow(10, true)]
        [DataRow(1000, true)]
        [DataRow(1001, false)]
        public void Komentar_PostujeGraniceDuzine(int duzina, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Komentar = new string('k', duzina);

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeIzvrsilac_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.IzvrsilacID = null!;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(KreirajRecenzijuDto.IzvrsilacID)));
        }
    }

    [TestClass]
    public class PrijaviRecenzijuDtoTests
    {
        private static PrijaviRecenzijuDto ValidanDto() => new()
        {
            RecenzijaID = 9,
            RazlogPrijave = "Recenzija sadrzi uvrede."
        };

        [TestMethod]
        public void NoviDto_ImaPrazanRazlog()
        {
            // Akcija
            var dto = new PrijaviRecenzijuDto();

            // Provjera
            Assert.AreEqual(string.Empty, dto.RazlogPrijave);
            Assert.AreEqual(0, dto.RecenzijaID);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual(9, dto.RecenzijaID);
            Assert.AreEqual("Recenzija sadrzi uvrede.", dto.RazlogPrijave);
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
        [DataRow(9, false)]
        [DataRow(10, true)]
        [DataRow(500, true)]
        [DataRow(501, false)]
        public void RazlogPrijave_PostujeGraniceDuzine(int duzina, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.RazlogPrijave = new string('r', duzina);

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeRazlog_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.RazlogPrijave = null!;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(PrijaviRecenzijuDto.RazlogPrijave)));
        }
    }

    [TestClass]
    public class ObradiPrijavuRecenzijeDtoTests
    {
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PostavljeneVrijednosti_SeVracaju(bool obrisi)
        {
            // Priprema
            var dto = new ObradiPrijavuRecenzijeDto
            {
                RecenzijaID = 11,
                Obrisi = obrisi
            };

            // Akcija i provjera
            Assert.AreEqual(11, dto.RecenzijaID);
            Assert.AreEqual(obrisi, dto.Obrisi);
        }
    }
}
