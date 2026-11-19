
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class GreaterSpecificationTests
    {
        private sealed class TestniProizvod
        {
            public int Cijena { get; set; }
            public decimal Tezina { get; set; }
            public DateTime Datum { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }

        [DataTestMethod]
        [DataRow(50, false)]
        [DataRow(99, false)]
        [DataRow(100, false)]
        [DataRow(101, true)]
        [DataRow(150, true)]
        [DataRow(1000, true)]
        public void GreaterThan_RazliciteVrijednosti_VracaOcekivaniRezultat(
            int cijena, bool ocekivano)
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = cijena };

            var rezultat = specifikacija.ToExpression()
                .Compile()(proizvod);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow(50, false)]
        [DataRow(99, false)]
        [DataRow(100, true)]
        [DataRow(101, true)]
        [DataRow(150, true)]
        [DataRow(1000, true)]
        public void GreaterThanOrEqual_RazliciteVrijednosti_VracaOcekivaniRezultat(
            int cijena, bool ocekivano)
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = cijena };

            var rezultat = specifikacija.ToExpression()
                .Compile()(proizvod);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void GreaterThan_JednakaGranica_VracaFalse()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsFalse(specifikacija.ToExpression()
                .Compile()(new TestniProizvod { Cijena = 100 }));
        }

        [TestMethod]
        public void GreaterThanOrEqual_JednakaGranica_VracaTrue()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsTrue(specifikacija.ToExpression()
                .Compile()(new TestniProizvod { Cijena = 100 }));
        }

        [TestMethod]
        public void GreaterThan_NegativneVrijednosti_RadiIspravno()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, -50);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = -49 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = -50 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = -100 }));
        }

        [TestMethod]
        public void GreaterThanOrEqual_NegativneVrijednosti_RadiIspravno()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, -50);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = -50 }));

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = 0 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = -51 }));
        }

        [TestMethod]
        public void GreaterThan_DecimalneVrijednosti_RadiIspravno()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, decimal>(
                    p => p.Tezina, 2.5m);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Tezina = 2.6m }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Tezina = 2.5m }));
        }

        [TestMethod]
        public void GreaterThanOrEqual_DecimalneVrijednosti_RadiIspravno()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, decimal>(
                    p => p.Tezina, 2.5m);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Tezina = 2.5m }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Tezina = 2.4m }));
        }

        [TestMethod]
        public void GreaterThan_Datum_RadiIspravno()
        {
            var granica = new DateTime(2026, 1, 1);

            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, DateTime>(
                    p => p.Datum, granica);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Datum = new DateTime(2026, 1, 2)
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Datum = granica
            }));
        }

        [TestMethod]
        public void GreaterThanOrEqual_Datum_RadiIspravno()
        {
            var granica = new DateTime(2026, 1, 1);

            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, DateTime>(
                    p => p.Datum, granica);

            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Datum = granica
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Datum = new DateTime(2025, 12, 31)
            }));
        }

        [TestMethod]
        public void GreaterThan_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 101 },
                new TestniProizvod { Cijena = 200 }
            };

            var rezultat = proizvodi.AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 101, 200 }, rezultat);
        }

        [TestMethod]
        public void GreaterThanOrEqual_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 101 },
                new TestniProizvod { Cijena = 200 }
            };

            var rezultat = proizvodi.AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 100, 101, 200 }, rezultat);
        }

        [TestMethod]
        public void GreaterThan_PraznaKolekcija_VracaPrazanRezultat()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var rezultat = new List<TestniProizvod>()
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void GreaterThanOrEqual_PraznaKolekcija_VracaPrazanRezultat()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var rezultat = new List<TestniProizvod>()
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void GreaterThan_OglasUsluge_FiltriraPremaBudzetu()
        {
            var specifikacija =
                new GreaterThanSpecification<OglasUsluge, int>(
                    o => o.MaxBudzet, 500);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge { OglasID = 1, MaxBudzet = 300 },
                new OglasUsluge { OglasID = 2, MaxBudzet = 500 },
                new OglasUsluge { OglasID = 3, MaxBudzet = 800 }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 3 }, rezultat);
        }

        [TestMethod]
        public void GreaterThanOrEqual_OglasUsluge_FiltriraPremaBudzetu()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<OglasUsluge, int>(
                    o => o.MaxBudzet, 500);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge { OglasID = 1, MaxBudzet = 300 },
                new OglasUsluge { OglasID = 2, MaxBudzet = 500 },
                new OglasUsluge { OglasID = 3, MaxBudzet = 800 }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 2, 3 }, rezultat);
        }

        [TestMethod]
        public void GreaterThan_PromjenaVrijednosti_MijenjaRezultat()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = 50 };
            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsFalse(predikat(proizvod));

            proizvod.Cijena = 150;

            Assert.IsTrue(predikat(proizvod));
        }

        [TestMethod]
        public void GreaterThanOrEqual_PromjenaVrijednosti_MijenjaRezultat()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = 50 };
            var predikat = specifikacija.ToExpression().Compile();

            Assert.IsFalse(predikat(proizvod));

            proizvod.Cijena = 100;

            Assert.IsTrue(predikat(proizvod));
        }

        [TestMethod]
        public void GreaterThan_StringBezOperatora_BacaInvalidOperationException()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, string>(
                    p => p.Naziv, "M");

            Assert.ThrowsException<InvalidOperationException>(
                () => specifikacija.ToExpression());
        }

        [TestMethod]
        public void GreaterThanOrEqual_StringBezOperatora_BacaInvalidOperationException()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, string>(
                    p => p.Naziv, "M");

            Assert.ThrowsException<InvalidOperationException>(
                () => specifikacija.ToExpression());
        }

        [TestMethod]
        public void GreaterThan_OrderByVrijednostiSuNull()
        {
            var specifikacija =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }

        [TestMethod]
        public void GreaterThanOrEqual_OrderByVrijednostiSuNull()
        {
            var specifikacija =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
