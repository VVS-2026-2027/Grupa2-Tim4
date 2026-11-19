
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Infrastructure.Wrappers;

namespace PopravkaBa.Tests.Unit.Infrastructure.Wrappers
{
    [TestClass]
    public class StraniceniRezultatDataDrivenTests
    {
        // CSV DATA-DRIVEN TESTIRANJE

        public static IEnumerable<object[]> CsvSlucajevi()
        {
            var putanja = Path.Combine(
                AppContext.BaseDirectory,
                "TestData",
                "StraniceniRezultat.csv");

            foreach (var red in File.ReadLines(putanja))
            {
                if (string.IsNullOrWhiteSpace(red))
                    continue;

                var kolone = red.Split(';');

                if (kolone.Length != 3)
                    throw new FormatException(
                        $"Neispravan CSV red: {red}");

                // Preskoci zaglavlje, cak i ako postoji BOM
                var naziv = kolone[0]
                    .Trim()
                    .TrimStart('\uFEFF');

                if (naziv.Equals(
                    "naziv",
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return new object[]
                {
                    naziv,
                    ProcitajStavke(kolone[1]),
                    int.Parse(
                        kolone[2].Trim(),
                        CultureInfo.InvariantCulture)
                };
            }
        }

        // XML DATA-DRIVEN TESTIRANJE

        public static IEnumerable<object[]> XmlSlucajevi()
        {
            var putanja = Path.Combine(
                AppContext.BaseDirectory,
                "TestData",
                "StraniceniRezultat.xml");

            var dokument = XDocument.Load(putanja);

            if (dokument.Root == null)
                throw new FormatException(
                    "XML dokument nema korijenski element.");

            foreach (var slucaj in dokument.Root.Elements("Slucaj"))
            {
                var naziv = (string?)slucaj.Attribute("naziv")
                    ?? throw new FormatException(
                        "Nedostaje naziv XML slucaja.");

                var stavkeTekst =
                    (string?)slucaj.Element("Stavke") ?? "";

                var ukupnoTekst =
                    (string?)slucaj.Element("Ukupno")
                    ?? throw new FormatException(
                        "Nedostaje Ukupno u XML slucaju.");

                yield return new object[]
                {
                    naziv,
                    ProcitajStavke(stavkeTekst),
                    int.Parse(
                        ukupnoTekst.Trim(),
                        CultureInfo.InvariantCulture)
                };
            }
        }

        // POMOCNA METODA ZA PARSIRANJE STAVKI

        private static int[] ProcitajStavke(string tekst)
        {
            if (string.IsNullOrWhiteSpace(tekst))
                return Array.Empty<int>();

            return tekst.Split('|')
                .Select(broj => int.Parse(
                    broj.Trim(),
                    CultureInfo.InvariantCulture))
                .ToArray();
        }

        // TESTIRANJE PODACIMA IZ CSV DATOTEKE

        [DataTestMethod]
        [DynamicData(
            nameof(CsvSlucajevi),
            DynamicDataSourceType.Method)]
        public void Rezultat_PodaciIzCsv_ZadrzavaStavkeIUkupno(
            string naziv,
            int[] stavke,
            int ukupno)
        {
            // Arrange
            var rezultat = new StraniceniRezultat<int>
            {
                Stavke = stavke.ToList(),
                Ukupno = ukupno
            };

            // Act
            var stvarneStavke = rezultat.Stavke.ToArray();
            var stvarnoUkupno = rezultat.Ukupno;

            // Assert
            CollectionAssert.AreEqual(
                stavke,
                stvarneStavke,
                naziv);

            Assert.AreEqual(
                ukupno,
                stvarnoUkupno,
                naziv);
        }

        // TESTIRANJE PODACIMA IZ XML DATOTEKE

        [DataTestMethod]
        [DynamicData(
            nameof(XmlSlucajevi),
            DynamicDataSourceType.Method)]
        public void Rezultat_PodaciIzXml_ZadrzavaStavkeIUkupno(
            string naziv,
            int[] stavke,
            int ukupno)
        {
            // Arrange
            var rezultat = new StraniceniRezultat<int>
            {
                Stavke = stavke.ToList(),
                Ukupno = ukupno
            };

            // Act
            var stvarneStavke = rezultat.Stavke.ToArray();
            var stvarnoUkupno = rezultat.Ukupno;

            // Assert
            CollectionAssert.AreEqual(
                stavke,
                stvarneStavke,
                naziv);

            Assert.AreEqual(
                ukupno,
                stvarnoUkupno,
                naziv);
        }
    }
}
