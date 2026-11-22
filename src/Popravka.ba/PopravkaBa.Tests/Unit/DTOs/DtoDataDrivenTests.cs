using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using PopravkaBa.Application.DTOs;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class DtoDataDrivenTests
    {
        public static IEnumerable<object[]> PrihodCsvSlucajevi()
            => CsvHelper.Ucitaj("dto_prihod.csv", MapirajPrihod);

        [DataTestMethod]
        [DynamicData(nameof(PrihodCsvSlucajevi), DynamicDataSourceType.Method)]
        public void ObjaviOglasRadnoMjesto_MinMaxPrihod_PostujePravilo(
            string opis,
            int? minPrihod,
            int? maxPrihod,
            int ocekivanoGresaka)
        {
            // Priprema
            var dto = new ObjaviOglasRadnoMjestoDto
            {
                MinPrihod = minPrihod,
                MaxPrihod = maxPrihod
            };

            // Akcija
            var rezultati = dto.Validate(new ValidationContext(dto)).ToList();

            // Provjera
            Assert.AreEqual(ocekivanoGresaka, rezultati.Count, opis);
        }

        public static IEnumerable<object[]> RecenzijaJsonSlucajevi()
            => JsonHelper.Ucitaj("dto_recenzija.json", MapirajRecenziju);

        [DataTestMethod]
        [DynamicData(nameof(RecenzijaJsonSlucajevi), DynamicDataSourceType.Method)]
        public void KreirajRecenziju_PodaciIzJson_PostujeValidaciju(
            string opis,
            int ocjena,
            string komentar,
            bool ocekivanoValidan)
        {
            // Priprema
            var dto = new KreirajRecenzijuDto
            {
                IzvrsilacID = "majstor-1",
                Ocjena = ocjena,
                Komentar = komentar
            };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0, opis);
        }

        private static object[] MapirajPrihod(string[] kolone) => new object[]
        {
            kolone[0].Trim(),
            ProcitajNullableInt(kolone[1])!,
            ProcitajNullableInt(kolone[2])!,
            int.Parse(kolone[3].Trim(), CultureInfo.InvariantCulture)
        };

        private static int? ProcitajNullableInt(string tekst)
            => string.IsNullOrWhiteSpace(tekst)
                ? null
                : int.Parse(tekst.Trim(), CultureInfo.InvariantCulture);

        private static object[] MapirajRecenziju(JsonElement element) => new object[]
        {
            element.GetProperty("opis").GetString()!,
            element.GetProperty("ocjena").GetInt32(),
            element.GetProperty("komentar").GetString()!,
            element.GetProperty("validan").GetBoolean()
        };
    }
}
