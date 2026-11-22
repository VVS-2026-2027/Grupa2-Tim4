using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class FilterPretrageDtoTests
    {
        [TestMethod]
        public void NoviDto_ImaZadaneVrijednosti()
        {
            // Priprema + Act
            var dto = new FilterPretrageDto();

            // Provjera
            Assert.IsNull(dto.AktivniTab);
            Assert.IsNotNull(dto.Lokacija);
            Assert.AreEqual(0, dto.Lokacija.Count);
            Assert.IsNotNull(dto.KategorijaId);
            Assert.AreEqual(0, dto.KategorijaId.Count);
            Assert.IsNull(dto.MinBudzet);
            Assert.IsNull(dto.MaxBudzet);
            Assert.IsNull(dto.MinOcjena);
            Assert.IsNull(dto.MaxOcjena);
            Assert.IsNull(dto.TipZaposlenja);
            Assert.IsNull(dto.GodineIskustva);
            Assert.IsNull(dto.KljucneRijeci);
            Assert.AreEqual(1, dto.Stranica);
            Assert.AreEqual(20, dto.StavkiPoStranici);
            Assert.AreEqual(SortiranjeIzvrsilacaUsluga.ProsjecnaOcjena_Desc, dto.SortiranjeIzvrsilaca);
            Assert.AreEqual(SortiranjeOglasaUsluge.MinBudzet_Desc, dto.SortiranjeUsluge);
            Assert.AreEqual(SortiranjeOglasaRadnoMjesto.MinPrihod_Desc, dto.SortiranjeRadnoMjesto);
        }

        [TestMethod]
        public void SvaPolja_PostavljeneVrijednostiSeVracaju()
        {
            // Priprema
            var mjesta = new List<int> { 1, 2 };
            var kategorije = new List<int> { 5, 8 };

            var dto = new FilterPretrageDto
            {
                AktivniTab = "izvrsioci",
                Lokacija = mjesta,
                KategorijaId = kategorije,
                MinBudzet = 100,
                MaxBudzet = 5000,
                MinOcjena = 3.5m,
                MaxOcjena = 4.9m,
                TipZaposlenja = VrstaZaposlenja.Sezonski,
                GodineIskustva = 5,
                KljucneRijeci = "vodoinstalater",
                Stranica = 3,
                StavkiPoStranici = 10,
                SortiranjeIzvrsilaca = SortiranjeIzvrsilacaUsluga.MinCijena_Asc,
                SortiranjeUsluge = SortiranjeOglasaUsluge.MaxBudzet_Desc,
                SortiranjeRadnoMjesto = SortiranjeOglasaRadnoMjesto.MaxPrihod_Asc
            };

            // Akcija i provjera
            Assert.AreEqual("izvrsioci", dto.AktivniTab);
            CollectionAssert.AreEqual(mjesta, dto.Lokacija);
            CollectionAssert.AreEqual(kategorije, dto.KategorijaId);
            Assert.AreEqual(100, dto.MinBudzet);
            Assert.AreEqual(5000, dto.MaxBudzet);
            Assert.AreEqual(3.5m, dto.MinOcjena);
            Assert.AreEqual(4.9m, dto.MaxOcjena);
            Assert.AreEqual(VrstaZaposlenja.Sezonski, dto.TipZaposlenja);
            Assert.AreEqual(5, dto.GodineIskustva);
            Assert.AreEqual("vodoinstalater", dto.KljucneRijeci);
            Assert.AreEqual(3, dto.Stranica);
            Assert.AreEqual(10, dto.StavkiPoStranici);
            Assert.AreEqual(SortiranjeIzvrsilacaUsluga.MinCijena_Asc, dto.SortiranjeIzvrsilaca);
            Assert.AreEqual(SortiranjeOglasaUsluge.MaxBudzet_Desc, dto.SortiranjeUsluge);
            Assert.AreEqual(SortiranjeOglasaRadnoMjesto.MaxPrihod_Asc, dto.SortiranjeRadnoMjesto);
        }
    }
}
