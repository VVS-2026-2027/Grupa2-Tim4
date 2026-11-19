
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class LessSpecificationTests
    {
        private sealed class TestniProizvod
        {
            public int Cijena { get; set; }
            public decimal Tezina { get; set; }
            public DateTime Datum { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }

        [DataTestMethod]
        [DataRow(-100, true)]
        [DataRow(0, true)]
        [DataRow(50, true)]
        [DataRow(99, true)]
        [DataRow(100, false)]
        [DataRow(101, false)]
        [DataRow(500, false)]
        public void LessThan_RazliciteVrijednosti_VracaOcekivano(
            int cijena, bool ocekivano)
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = cijena };

            var rezultat = spec.ToExpression().Compile()(proizvod);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow(-100, true)]
        [DataRow(0, true)]
        [DataRow(50, true)]
        [DataRow(99, true)]
        [DataRow(100, true)]
        [DataRow(101, false)]
        [DataRow(500, false)]
        public void LessThanOrEqual_RazliciteVrijednosti_VracaOcekivano(
            int cijena, bool ocekivano)
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = cijena };

            var rezultat = spec.ToExpression().Compile()(proizvod);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void LessThan_JednakaGranica_VracaFalse()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsFalse(spec.ToExpression().Compile()(
                new TestniProizvod { Cijena = 100 }));
        }

        [TestMethod]
        public void LessThanOrEqual_JednakaGranica_VracaTrue()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsTrue(spec.ToExpression().Compile()(
                new TestniProizvod { Cijena = 100 }));
        }

        [TestMethod]
        public void LessThan_NegativneVrijednosti_RadiIspravno()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, -50);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = -51 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = -50 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = 0 }));
        }

        [TestMethod]
        public void LessThanOrEqual_NegativneVrijednosti_RadiIspravno()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, -50);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = -51 }));

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = -50 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = -49 }));
        }

        [TestMethod]
        public void LessThan_DecimalneVrijednosti_RadeIspravno()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, decimal>(
                    p => p.Tezina, 2.5m);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Tezina = 2.4m }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Tezina = 2.5m }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Tezina = 2.6m }));
        }

        [TestMethod]
        public void LessThanOrEqual_DecimalneVrijednosti_RadeIspravno()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, decimal>(
                    p => p.Tezina, 2.5m);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Tezina = 2.4m }));

            Assert.IsTrue(predikat(
                new TestniProizvod { Tezina = 2.5m }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Tezina = 2.6m }));
        }

        [TestMethod]
        public void LessThan_Datum_RadiIspravno()
        {
            var granica = new DateTime(2026, 1, 1);

            var spec =
                new LessThanSpecification<TestniProizvod, DateTime>(
                    p => p.Datum, granica);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Datum = new DateTime(2025, 12, 31)
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Datum = granica
            }));
        }

        [TestMethod]
        public void LessThanOrEqual_Datum_RadiIspravno()
        {
            var granica = new DateTime(2026, 1, 1);

            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, DateTime>(
                    p => p.Datum, granica);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Datum = new DateTime(2025, 12, 31)
            }));

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Datum = granica
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Datum = new DateTime(2026, 1, 2)
            }));
        }

        [TestMethod]
        public void LessThan_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 99 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 101 },
                new TestniProizvod { Cijena = 200 }
            };

            var rezultat = proizvodi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 50, 99 }, rezultat);
        }

        [TestMethod]
        public void LessThanOrEqual_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 99 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 101 },
                new TestniProizvod { Cijena = 200 }
            };

            var rezultat = proizvodi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 50, 99, 100 }, rezultat);
        }

        [TestMethod]
        public void LessThan_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var rezultat = new List<TestniProizvod>()
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void LessThanOrEqual_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var rezultat = new List<TestniProizvod>()
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void LessThan_OglasUsluge_FiltriraPremaBudzetu()
        {
            var spec =
                new LessThanSpecification<OglasUsluge, int>(
                    o => o.MaxBudzet, 500);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge { OglasID = 1, MaxBudzet = 300 },
                new OglasUsluge { OglasID = 2, MaxBudzet = 500 },
                new OglasUsluge { OglasID = 3, MaxBudzet = 800 }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1 }, rezultat);
        }

        [TestMethod]
        public void LessThanOrEqual_OglasUsluge_FiltriraPremaBudzetu()
        {
            var spec =
                new LessThanOrEqualSpecification<OglasUsluge, int>(
                    o => o.MaxBudzet, 500);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge { OglasID = 1, MaxBudzet = 300 },
                new OglasUsluge { OglasID = 2, MaxBudzet = 500 },
                new OglasUsluge { OglasID = 3, MaxBudzet = 800 }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1, 2 }, rezultat);
        }

        [TestMethod]
        public void LessThan_PromjenaVrijednosti_MijenjaRezultat()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = 150 };
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(proizvod));

            proizvod.Cijena = 50;

            Assert.IsTrue(predikat(proizvod));
        }

        [TestMethod]
        public void LessThanOrEqual_PromjenaVrijednosti_MijenjaRezultat()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var proizvod = new TestniProizvod { Cijena = 150 };
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(proizvod));

            proizvod.Cijena = 100;

            Assert.IsTrue(predikat(proizvod));
        }

        [TestMethod]
        public void LessThan_StringBezOperatora_BacaInvalidOperationException()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, string>(
                    p => p.Naziv, "M");

            Assert.ThrowsException<InvalidOperationException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void LessThanOrEqual_StringBezOperatora_BacaInvalidOperationException()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, string>(
                    p => p.Naziv, "M");

            Assert.ThrowsException<InvalidOperationException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void LessThan_OrderByVrijednostiSuNull()
        {
            var spec =
                new LessThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        [TestMethod]
        public void LessThanOrEqual_OrderByVrijednostiSuNull()
        {
            var spec =
                new LessThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
