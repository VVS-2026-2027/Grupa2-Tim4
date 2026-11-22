using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class PonudaDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var pocetak = new DateTime(2026, 11, 25, 9, 0, 0);
            var kraj = new DateTime(2026, 11, 27, 17, 0, 0);

            var dto = new PonudaDto
            {
                PonudaId = 15,
                IzvrsilacId = "majstor-1",
                IzvrsilacUsername = "majstor1",
                IzvrsilacIme = "Milan Jovanovic",
                IzvrsilacSlika = "https://cdn.example.com/majstor.jpg",
                IzvrsilacKategorija = "Vodovod",
                Verificiran = true,
                Cijena = 300,
                TipIsplate = TipIsplate.PoSatu,
                ProsjecnaOcjena = 4.7m,
                BrojRecenzija = 25,
                DatumPocetka = pocetak,
                DatumKraja = kraj,
                RazlikaOdProsjeka = 0.3m,
                StatusPonude = Status.NaCekanju,
                Poruka = "Mogu poceti odmah."
            };

            // Akcija i provjera
            Assert.AreEqual(15, dto.PonudaId);
            Assert.AreEqual("majstor-1", dto.IzvrsilacId);
            Assert.AreEqual("majstor1", dto.IzvrsilacUsername);
            Assert.AreEqual("Milan Jovanovic", dto.IzvrsilacIme);
            Assert.AreEqual("https://cdn.example.com/majstor.jpg", dto.IzvrsilacSlika);
            Assert.AreEqual("Vodovod", dto.IzvrsilacKategorija);
            Assert.IsTrue(dto.Verificiran);
            Assert.AreEqual(300, dto.Cijena);
            Assert.AreEqual(TipIsplate.PoSatu, dto.TipIsplate);
            Assert.AreEqual(4.7m, dto.ProsjecnaOcjena);
            Assert.AreEqual(25, dto.BrojRecenzija);
            Assert.AreEqual(pocetak, dto.DatumPocetka);
            Assert.AreEqual(kraj, dto.DatumKraja);
            Assert.AreEqual(0.3m, dto.RazlikaOdProsjeka);
            Assert.AreEqual(Status.NaCekanju, dto.StatusPonude);
            Assert.AreEqual("Mogu poceti odmah.", dto.Poruka);
        }
    }

    [TestClass]
    public class KreirajPonudaUslugeDtoTests
    {
        private static KreirajPonudaUslugeDto ValidanDto() => new()
        {
            OglasUslugeID = 5,
            DatumPocetkaUsluge = new DateTime(2026, 12, 1),
            DatumOcekivanogZavrsetka = new DateTime(2026, 12, 3),
            Cijena = 250,
            TipIsplate = TipIsplate.PoSatu,
            Poruka = "Dostupan sam u navedenom terminu."
        };

        [TestMethod]
        public void NoviDto_TipIsplateJePoSatu()
        {
            // Akcija
            var dto = new KreirajPonudaUslugeDto();

            // Provjera
            Assert.AreEqual(TipIsplate.PoSatu, dto.TipIsplate);
            Assert.IsNull(dto.DatumOcekivanogZavrsetka);
            Assert.IsNull(dto.Cijena);
            Assert.IsNull(dto.Poruka);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual(5, dto.OglasUslugeID);
            Assert.AreEqual(new DateTime(2026, 12, 1), dto.DatumPocetkaUsluge);
            Assert.AreEqual(new DateTime(2026, 12, 3), dto.DatumOcekivanogZavrsetka);
            Assert.AreEqual(250, dto.Cijena);
            Assert.AreEqual(TipIsplate.PoSatu, dto.TipIsplate);
            Assert.AreEqual("Dostupan sam u navedenom terminu.", dto.Poruka);
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
        [DataRow(-1, false)]
        [DataRow(0, true)]
        [DataRow(500, true)]
        [DataRow(null, true)]
        public void Cijena_PostujeGranicu(int? cijena, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Cijena = cijena;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow(499, true)]
        [DataRow(500, true)]
        [DataRow(501, false)]
        public void Poruka_PostujeMaksimalnuDuzinu(int duzina, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Poruka = new string('p', duzina);

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }
    }

    [TestClass]
    public class UrediPonudaUslugeDtoTests
    {
        [TestMethod]
        public void MozeSeKreirati()
        {
            // Akcija
            var dto = new UrediPonudaUslugeDto();

            // Provjera
            Assert.IsNotNull(dto);
        }
    }
}
