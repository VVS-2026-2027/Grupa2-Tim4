
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Infrastructure.Wrappers;

namespace PopravkaBa.Tests.Unit.Infrastructure.Wrappers
{
    [TestClass]
    public class StraniceniRezultatTests
    {
        [TestMethod]
        public void Konstruktor_StavkeSuPraznaLista()
        {
            var rezultat = new StraniceniRezultat<int>();

            Assert.IsNotNull(rezultat.Stavke);
            Assert.AreEqual(0, rezultat.Stavke.Count);
        }

        [TestMethod]
        public void Konstruktor_UkupnoJeNula()
        {
            var rezultat = new StraniceniRezultat<int>();

            Assert.AreEqual(0, rezultat.Ukupno);
        }

        [TestMethod]
        public void Stavke_DodavanjeJedneStavke_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<string>();

            rezultat.Stavke.Add("Prva stavka");

            Assert.AreEqual(1, rezultat.Stavke.Count);
            Assert.AreEqual("Prva stavka", rezultat.Stavke[0]);
        }

        [TestMethod]
        public void Stavke_DodavanjeViseStavki_CuvaRedoslijed()
        {
            var rezultat = new StraniceniRezultat<int>();

            rezultat.Stavke.AddRange(new[] { 10, 20, 30 });

            CollectionAssert.AreEqual(
                new List<int> { 10, 20, 30 },
                rezultat.Stavke);
        }

        [TestMethod]
        public void Stavke_PostavljanjeListe_RadiIspravno()
        {
            var lista = new List<string>
            {
                "Elektricar",
                "Vodoinstalater"
            };

            var rezultat = new StraniceniRezultat<string>
            {
                Stavke = lista
            };

            Assert.AreSame(lista, rezultat.Stavke);
            Assert.AreEqual(2, rezultat.Stavke.Count);
        }

        [TestMethod]
        public void Stavke_ZamjenaListe_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Stavke = new List<int> { 1, 2 }
            };

            rezultat.Stavke = new List<int> { 5, 6, 7 };

            CollectionAssert.AreEqual(
                new List<int> { 5, 6, 7 },
                rezultat.Stavke);
        }

        [TestMethod]
        public void Stavke_BrisanjeStavke_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Stavke = new List<int> { 1, 2, 3 }
            };

            rezultat.Stavke.Remove(2);

            CollectionAssert.AreEqual(
                new List<int> { 1, 3 },
                rezultat.Stavke);
        }

        [TestMethod]
        public void Stavke_NullVrijednost_PrihvataNull()
        {
            var rezultat = new StraniceniRezultat<string>
            {
                Stavke = null!
            };

            Assert.IsNull(rezultat.Stavke);
        }

        [TestMethod]
        public void Ukupno_PostavljanjePozitivneVrijednosti_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Ukupno = 100
            };

            Assert.AreEqual(100, rezultat.Ukupno);
        }

        [TestMethod]
        public void Ukupno_PostavljanjeNule_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Ukupno = 0
            };

            Assert.AreEqual(0, rezultat.Ukupno);
        }

        [TestMethod]
        public void Ukupno_NegativnaVrijednost_TrenutnoJeDozvoljena()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Ukupno = -10
            };

            Assert.AreEqual(-10, rezultat.Ukupno);
        }

        [TestMethod]
        public void Ukupno_PromjenaVrijednosti_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Ukupno = 50
            };

            rezultat.Ukupno = 200;

            Assert.AreEqual(200, rezultat.Ukupno);
        }

        [TestMethod]
        public void Ukupno_NezavisnoOdBrojaStavki()
        {
            var rezultat = new StraniceniRezultat<int>
            {
                Stavke = new List<int> { 1, 2, 3 },
                Ukupno = 100
            };

            Assert.AreEqual(3, rezultat.Stavke.Count);
            Assert.AreEqual(100, rezultat.Ukupno);
        }

        [TestMethod]
        public void GenerickiTip_String_RadiIspravno()
        {
            var rezultat = new StraniceniRezultat<string>
            {
                Stavke = new List<string>
                {
                    "Mostar",
                    "Sarajevo"
                },
                Ukupno = 2
            };

            Assert.AreEqual("Mostar", rezultat.Stavke[0]);
            Assert.AreEqual("Sarajevo", rezultat.Stavke[1]);
            Assert.AreEqual(2, rezultat.Ukupno);
        }

        [TestMethod]
        public void GenerickiTip_Objekat_RadiIspravno()
        {
            var prva = new TestnaStavka
            {
                Id = 1,
                Naziv = "Elektricar"
            };

            var rezultat = new StraniceniRezultat<TestnaStavka>
            {
                Stavke = new List<TestnaStavka> { prva },
                Ukupno = 10
            };

            Assert.AreEqual(1, rezultat.Stavke.Count);
            Assert.AreSame(prva, rezultat.Stavke[0]);
            Assert.AreEqual(10, rezultat.Ukupno);
        }

        [TestMethod]
        public void DvaObjekta_ImajuNezavisneListe()
        {
            var prvi = new StraniceniRezultat<int>();
            var drugi = new StraniceniRezultat<int>();

            prvi.Stavke.Add(10);

            Assert.AreEqual(1, prvi.Stavke.Count);
            Assert.AreEqual(0, drugi.Stavke.Count);
            Assert.AreNotSame(prvi.Stavke, drugi.Stavke);
        }

        public sealed class TestnaStavka
        {
            public int Id { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }
    }
}
