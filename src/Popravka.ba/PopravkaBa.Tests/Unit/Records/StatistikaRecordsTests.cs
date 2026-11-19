
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Records;

namespace PopravkaBa.Tests.Unit.Records
{
    [TestClass]
    public class StatistikaRecordsTests
    {
        [TestMethod]
        public void StatistikaRecord_Konstruktor_IspravnoPostavljaVrijednosti()
        {
            var record = new StatistikaRecord(1, "Elektricar");

            Assert.AreEqual(1, record.Id);
            Assert.AreEqual("Elektricar", record.Naziv);
        }

        [TestMethod]
        public void StatistikaRecord_DvaJednakaRecorda_SuJednaka()
        {
            var prvi = new StatistikaRecord(1, "Elektricar");
            var drugi = new StatistikaRecord(1, "Elektricar");

            Assert.AreEqual(prvi, drugi);
            Assert.IsTrue(prvi == drugi);
            Assert.IsFalse(prvi != drugi);
            Assert.AreEqual(prvi.GetHashCode(), drugi.GetHashCode());
        }

        [TestMethod]
        public void StatistikaRecord_RazlicitId_NisuJednaki()
        {
            var prvi = new StatistikaRecord(1, "Elektricar");
            var drugi = new StatistikaRecord(2, "Elektricar");

            Assert.AreNotEqual(prvi, drugi);
            Assert.IsTrue(prvi != drugi);
        }

        [TestMethod]
        public void StatistikaRecord_RazlicitNaziv_NisuJednaki()
        {
            var prvi = new StatistikaRecord(1, "Elektricar");
            var drugi = new StatistikaRecord(1, "Vodoinstalater");

            Assert.AreNotEqual(prvi, drugi);
        }

        [TestMethod]
        public void StatistikaRecord_With_MijenjaSamoOdabranoSvojstvo()
        {
            var original = new StatistikaRecord(1, "Elektricar");

            var kopija = original with
            {
                Naziv = "Vodoinstalater"
            };

            Assert.AreEqual(1, kopija.Id);
            Assert.AreEqual("Vodoinstalater", kopija.Naziv);

            Assert.AreEqual("Elektricar", original.Naziv);
        }

        [TestMethod]
        public void StatistikaRecord_Deconstruction_VracaIspravneVrijednosti()
        {
            var record = new StatistikaRecord(5, "Mostar");

            var (id, naziv) = record;

            Assert.AreEqual(5, id);
            Assert.AreEqual("Mostar", naziv);
        }

        [TestMethod]
        public void StatistikaRecord_ToString_SadrziPodatke()
        {
            var record = new StatistikaRecord(5, "Mostar");

            var tekst = record.ToString();

            StringAssert.Contains(tekst, "5");
            StringAssert.Contains(tekst, "Mostar");
        }

        [TestMethod]
        public void StatistikaRecord_NullNaziv_ZadrzavaNull()
        {
            var record = new StatistikaRecord(1, null!);

            Assert.IsNull(record.Naziv);
        }

        [TestMethod]
        public void StatistikaMetaRecord_Konstruktor_IspravnoPostavljaVrijednosti()
        {
            var datum = new DateTime(2026, 11, 19);

            IReadOnlyList<StatistikaRecord> kategorije =
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(1, "Elektricar"),
                    new StatistikaRecord(2, "Vodoinstalater")
                };

            IReadOnlyList<StatistikaRecord> lokacije =
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(3, "Mostar")
                };

            var record = new StatistikaMetaRecord(
                datum,
                kategorije,
                lokacije);

            Assert.AreEqual(datum, record.ZadnjeAzuriranje);
            Assert.AreSame(kategorije, record.Kategorije);
            Assert.AreSame(lokacije, record.Lokacije);
            Assert.AreEqual(2, record.Kategorije.Count);
            Assert.AreEqual(1, record.Lokacije.Count);
        }

        [TestMethod]
        public void StatistikaMetaRecord_NullDatum_PrihvataVrijednost()
        {
            var record = new StatistikaMetaRecord(
                null,
                Array.Empty<StatistikaRecord>(),
                Array.Empty<StatistikaRecord>());

            Assert.IsNull(record.ZadnjeAzuriranje);
        }

        [TestMethod]
        public void StatistikaMetaRecord_PrazneListe_PrihvataVrijednosti()
        {
            var record = new StatistikaMetaRecord(
                DateTime.UtcNow,
                Array.Empty<StatistikaRecord>(),
                Array.Empty<StatistikaRecord>());

            Assert.AreEqual(0, record.Kategorije.Count);
            Assert.AreEqual(0, record.Lokacije.Count);
        }

        [TestMethod]
        public void StatistikaMetaRecord_JednakeVrijednosti_SuJednake()
        {
            var datum = new DateTime(2026, 1, 1);

            var kategorije = new List<StatistikaRecord>
            {
                new StatistikaRecord(1, "Elektricar")
            };

            var lokacije = new List<StatistikaRecord>
            {
                new StatistikaRecord(2, "Mostar")
            };

            var prvi = new StatistikaMetaRecord(
                datum, kategorije, lokacije);

            var drugi = new StatistikaMetaRecord(
                datum, kategorije, lokacije);

            Assert.AreEqual(prvi, drugi);
            Assert.IsTrue(prvi == drugi);
            Assert.AreEqual(prvi.GetHashCode(), drugi.GetHashCode());
        }

        [TestMethod]
        public void StatistikaMetaRecord_RazlicitDatum_NisuJednaki()
        {
            var kategorije = Array.Empty<StatistikaRecord>();
            var lokacije = Array.Empty<StatistikaRecord>();

            var prvi = new StatistikaMetaRecord(
                new DateTime(2026, 1, 1),
                kategorije,
                lokacije);

            var drugi = new StatistikaMetaRecord(
                new DateTime(2026, 1, 2),
                kategorije,
                lokacije);

            Assert.AreNotEqual(prvi, drugi);
        }

        [TestMethod]
        public void StatistikaMetaRecord_RazliciteReferenceLista_NisuJednake()
        {
            var prvi = new StatistikaMetaRecord(
                null,
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(1, "Elektricar")
                },
                Array.Empty<StatistikaRecord>());

            var drugi = new StatistikaMetaRecord(
                null,
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(1, "Elektricar")
                },
                Array.Empty<StatistikaRecord>());

            // Record poredi liste po referenci,
            // a ne prema njihovom sadrzaju.
            Assert.AreNotEqual(prvi, drugi);
        }

        [TestMethod]
        public void StatistikaMetaRecord_With_MijenjaDatum()
        {
            var original = new StatistikaMetaRecord(
                null,
                Array.Empty<StatistikaRecord>(),
                Array.Empty<StatistikaRecord>());

            var datum = new DateTime(2026, 11, 19);

            var kopija = original with
            {
                ZadnjeAzuriranje = datum
            };

            Assert.AreEqual(datum, kopija.ZadnjeAzuriranje);
            Assert.IsNull(original.ZadnjeAzuriranje);
        }

        [TestMethod]
        public void StatistikaMetaRecord_Deconstruction_VracaIspravneVrijednosti()
        {
            var datum = new DateTime(2026, 11, 19);

            IReadOnlyList<StatistikaRecord> kategorije =
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(1, "Elektricar")
                };

            IReadOnlyList<StatistikaRecord> lokacije =
                new List<StatistikaRecord>
                {
                    new StatistikaRecord(2, "Mostar")
                };

            var record = new StatistikaMetaRecord(
                datum, kategorije, lokacije);

            var (zadnjeAzuriranje, kat, lok) = record;

            Assert.AreEqual(datum, zadnjeAzuriranje);
            Assert.AreSame(kategorije, kat);
            Assert.AreSame(lokacije, lok);
        }

        [TestMethod]
        public void StatistikaMetaRecord_ToString_SadrziNaziveSvojstava()
        {
            var record = new StatistikaMetaRecord(
                null,
                Array.Empty<StatistikaRecord>(),
                Array.Empty<StatistikaRecord>());

            var tekst = record.ToString();

            StringAssert.Contains(tekst, "ZadnjeAzuriranje");
            StringAssert.Contains(tekst, "Kategorije");
            StringAssert.Contains(tekst, "Lokacije");
        }

        [TestMethod]
        public void StatistikaMetaRecord_NullListe_ZadrzavaNull()
        {
            var record = new StatistikaMetaRecord(
                null,
                null!,
                null!);

            Assert.IsNull(record.Kategorije);
            Assert.IsNull(record.Lokacije);
        }
    }
}
