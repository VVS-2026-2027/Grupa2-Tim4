using System.Globalization;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.TDD
{
    [TestClass]
    public class NivoIzvrsiocaTests
    {
        private static readonly string CsvPutanja =
            Path.Combine(AppContext.BaseDirectory, "TestData", "nivo_izvrsioca.csv");

        public static IEnumerable<object[]> IzvuciIzCsv() =>
            File.ReadLines(CsvPutanja)
                .Skip(1)
                .Where(red => !string.IsNullOrWhiteSpace(red))
                .Select(red => red.Split(','))
                .Select(k => new object[]
                {
                    int.Parse(k[0], CultureInfo.InvariantCulture),
                    int.Parse(k[1], CultureInfo.InvariantCulture),
                    decimal.Parse(k[2], CultureInfo.InvariantCulture),
                    Enum.Parse<NivoIzvrsioca>(k[3]),
                    k[4]
                });

        private static Majstor KreirajIzvrsioca(int poslova, int recenzija, decimal ocjena) => new()
        {
            BrojZavrsenihPoslova = poslova,
            BrojRecenzija = recenzija,
            ProsjecnaOcjena = ocjena
        };

        [TestMethod]
        [DynamicData(nameof(IzvuciIzCsv), DynamicDataSourceType.Method)]
        public void DajNivo_PodaciIzCsv_VracaOcekivaniNivo(
            int poslova, int recenzija, decimal ocjena, NivoIzvrsioca ocekivano, string opis)
        {
            var izvrsilac = KreirajIzvrsioca(poslova, recenzija, ocjena);

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(ocekivano, nivo, opis);
        }

        [TestMethod]
        public void DajNivo_NoviIzvrsilacBezPodataka_VracaNovi()
        {
            var izvrsilac = new Majstor();

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(NivoIzvrsioca.Novi, nivo);
        }

        [TestMethod]
        public void DajNivo_TacnoNaPraguZaPouzdan_VracaPouzdan()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 3, recenzija: 3, ocjena: 4.0m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(NivoIzvrsioca.Pouzdan, nivo);
        }

        [TestMethod]
        public void DajNivo_TacnoNaPraguZaTopMajstor_VracaTopMajstor()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 10, recenzija: 5, ocjena: 4.5m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(NivoIzvrsioca.TopMajstor, nivo);
        }

        [TestMethod]
        public void DajNivo_IspunjavaTopOsimOcjene_VracaPouzdan()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 10, recenzija: 5, ocjena: 4.49m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(NivoIzvrsioca.Pouzdan, nivo);
        }

        [TestMethod]
        public void DajNivo_MnogoPoslovaLosaOcjena_VracaNovi()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 100, recenzija: 50, ocjena: 2.0m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreEqual(NivoIzvrsioca.Novi, nivo);
        }

        [TestMethod]
        public void DajNivo_IskustvoBezDobreOcjene_NijeTopMajstor()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 100, recenzija: 50, ocjena: 2.0m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreNotEqual(NivoIzvrsioca.TopMajstor, nivo);
        }

        [TestMethod]
        public void DajNivo_IspunjavaUsloveZaPouzdan_NijeNovi()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 5, recenzija: 4, ocjena: 4.2m);

            var nivo = izvrsilac.DajNivo();

            Assert.AreNotEqual(NivoIzvrsioca.Novi, nivo);
        }

        [TestMethod]
        public void DajNivo_IspunjavaUsloveZaTopMajstor_NivoIznadPouzdan()
        {
            var izvrsilac = KreirajIzvrsioca(poslova: 20, recenzija: 10, ocjena: 4.9m);

            var nivo = izvrsilac.DajNivo();

            Assert.IsTrue(nivo > NivoIzvrsioca.Pouzdan);
        }

        [TestMethod]
        public void NivoIzvrsioca_Vrijednosti_PoredaneOdNajnizegDoNajviseg()
        {
            NivoIzvrsioca[] ocekivano = [NivoIzvrsioca.Novi, NivoIzvrsioca.Pouzdan, NivoIzvrsioca.TopMajstor];

            CollectionAssert.AreEqual(ocekivano, Enum.GetValues<NivoIzvrsioca>());
        }
    }
}
