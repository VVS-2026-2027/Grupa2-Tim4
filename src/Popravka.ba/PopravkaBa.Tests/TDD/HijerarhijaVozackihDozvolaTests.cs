using System.Xml.Linq;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Shared;

namespace PopravkaBa.Tests.TDD
{
    [TestClass]
    public class HijerarhijaVozackihDozvolaTests
    {
        private static readonly string XmlPutanja =
            Path.Combine(AppContext.BaseDirectory, "TestData", "vozacke_dozvole.xml");

        public static IEnumerable<object[]> IzvuciIzXml() =>
            XDocument.Load(XmlPutanja)
                .Descendants("slucaj")
                .Select(s => new object[]
                {
                    ((string)s.Attribute("posjedovane")!)
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(Enum.Parse<VozackaDozvola>)
                        .ToArray(),
                    Enum.Parse<VozackaDozvola>((string)s.Attribute("trazena")!),
                    (bool)s.Attribute("ocekivano")!,
                    (string)s.Attribute("opis")!
                });

        public static IEnumerable<object[]> SveKategorije =>
            Enum.GetValues<VozackaDozvola>().Select(d => new object[] { d });

        [TestMethod]
        [DynamicData(nameof(IzvuciIzXml), DynamicDataSourceType.Method)]
        public void Pokriva_PodaciIzXml_VracaOcekivanoPokrivanje(
            VozackaDozvola[] posjedovane, VozackaDozvola trazena, bool ocekivano, string opis)
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva(posjedovane, trazena);

            Assert.AreEqual(ocekivano, rezultat, opis);
        }

        [TestMethod]
        [DynamicData(nameof(SveKategorije))]
        public void Pokriva_PosjedujeSve_PokrivaSvakuKategoriju(VozackaDozvola trazena)
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([VozackaDozvola.Sve], trazena);

            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void Pokriva_IstaKategorija_VracaTrue()
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([VozackaDozvola.C], VozackaDozvola.C);

            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void Pokriva_CEPokrivaB1Tranzitivno_VracaTrue()
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([VozackaDozvola.CE], VozackaDozvola.B1);

            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void Pokriva_NizaKategorijaTraziVisu_VracaFalse()
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([VozackaDozvola.B], VozackaDozvola.C);

            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void Pokriva_RazliciteGrupe_VracaFalse()
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([VozackaDozvola.DE], VozackaDozvola.A);

            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void Pokriva_PraznaListaDozvola_VracaFalse()
        {
            var rezultat = HijerarhijaVozackihDozvola.Pokriva([], VozackaDozvola.B1);

            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void Pokriva_PosjedovaneNull_BacaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => HijerarhijaVozackihDozvola.Pokriva(null!, VozackaDozvola.B));
        }
    }
}
