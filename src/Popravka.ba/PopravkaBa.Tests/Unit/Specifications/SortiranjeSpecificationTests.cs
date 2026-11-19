
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class SortiranjeSpecificationTests
    {
        // Public je neophodan jer Moq kreira proxy objekat.
        public sealed class TestniProizvod
        {
            public int Cijena { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }

        private static SortiranjeSpecification<TestniProizvod>
            KreirajSpecifikaciju(bool descending = false)
        {
            return new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                p => p.Cijena,
                descending);
        }

        [TestMethod]
        public void Konstruktor_BezDescendingParametra_KoristiUzlaznoSortiranje()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                p => p.Cijena);

            Assert.IsNotNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        [TestMethod]
        public void OrderBy_Ascending_VracaIzrazZaSortiranje()
        {
            var spec = KreirajSpecifikaciju(false);

            Assert.IsNotNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);

            var proizvod = new TestniProizvod { Cijena = 150 };

            Assert.AreEqual(
                150,
                spec.OrderBy!.Compile()(proizvod));
        }

        [TestMethod]
        public void OrderBy_Descending_VracaNull()
        {
            var spec = KreirajSpecifikaciju(true);

            Assert.IsNull(spec.OrderBy);
        }

        [TestMethod]
        public void OrderByDescending_Descending_VracaIzraz()
        {
            var spec = KreirajSpecifikaciju(true);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNotNull(spec.OrderByDescending);

            var proizvod = new TestniProizvod { Cijena = 250 };

            Assert.AreEqual(
                250,
                spec.OrderByDescending!.Compile()(proizvod));
        }

        [TestMethod]
        public void OrderByDescending_Ascending_VracaNull()
        {
            var spec = KreirajSpecifikaciju(false);

            Assert.IsNull(spec.OrderByDescending);
        }

        [TestMethod]
        public void OrderBy_UzlaznoSortiranje_VracaIspravanRedoslijed()
        {
            var spec = KreirajSpecifikaciju(false);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 300 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 200 },
                new TestniProizvod { Cijena = 50 }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .OrderBy(spec.OrderBy!)
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 50, 100, 200, 300 },
                rezultat);
        }

        [TestMethod]
        public void OrderByDescending_SilaznoSortiranje_VracaIspravanRedoslijed()
        {
            var spec = KreirajSpecifikaciju(true);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 300 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 200 },
                new TestniProizvod { Cijena = 50 }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .OrderByDescending(spec.OrderByDescending!)
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 300, 200, 100, 50 },
                rezultat);
        }

        [TestMethod]
        public void OrderBy_SortiranjePoNazivu_RadiIspravno()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                p => p.Naziv);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Naziv = "Zid" },
                new TestniProizvod { Naziv = "Alat" },
                new TestniProizvod { Naziv = "Krov" }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .OrderBy(spec.OrderBy!)
                .Select(p => p.Naziv)
                .ToList();

            CollectionAssert.AreEqual(
                new List<string> { "Alat", "Krov", "Zid" },
                rezultat);
        }

        [TestMethod]
        public void OrderByDescending_SortiranjePoNazivu_RadiIspravno()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                p => p.Naziv,
                true);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Naziv = "Zid" },
                new TestniProizvod { Naziv = "Alat" },
                new TestniProizvod { Naziv = "Krov" }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .OrderByDescending(spec.OrderByDescending!)
                .Select(p => p.Naziv)
                .ToList();

            CollectionAssert.AreEqual(
                new List<string> { "Zid", "Krov", "Alat" },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_AllSpecification_PropustaSveObjekte()
        {
            var spec = KreirajSpecifikaciju();

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = 10 }));

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = 1000 }));
        }

        [TestMethod]
        public void ToExpression_InnerFilter_PropustaSamoOdgovarajuceObjekte()
        {
            var inner =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var spec = new SortiranjeSpecification<TestniProizvod>(
                inner,
                p => p.Cijena);

            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = 50 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = 100 }));

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = 150 }));
        }

        [TestMethod]
        public void ToExpression_FilterISortiranje_RadeZajedno()
        {
            var inner =
                new GreaterThanOrEqualSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var spec = new SortiranjeSpecification<TestniProizvod>(
                inner,
                p => p.Cijena);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 300 },
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 200 },
                new TestniProizvod { Cijena = 100 }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .Where(spec.ToExpression())
                .OrderBy(spec.OrderBy!)
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 100, 200, 300 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_FilterISilaznoSortiranje_RadeZajedno()
        {
            var inner =
                new GreaterThanSpecification<TestniProizvod, int>(
                    p => p.Cijena, 100);

            var spec = new SortiranjeSpecification<TestniProizvod>(
                inner,
                p => p.Cijena,
                true);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 300 },
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 200 },
                new TestniProizvod { Cijena = 100 }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .Where(spec.ToExpression())
                .OrderByDescending(spec.OrderByDescending!)
                .Select(p => p.Cijena)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 300, 200 },
                rezultat);
        }

        [TestMethod]
        public void OrderBy_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec = KreirajSpecifikaciju();

            var rezultat = new List<TestniProizvod>()
                .AsQueryable()
                .OrderBy(spec.OrderBy!)
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void OrderBy_JednakaVrijednost_ZadrzavaSveObjekte()
        {
            var spec = KreirajSpecifikaciju();

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 100 }
            };

            var rezultat = proizvodi
                .AsQueryable()
                .OrderBy(spec.OrderBy!)
                .ToList();

            Assert.AreEqual(3, rezultat.Count);
            Assert.IsTrue(rezultat.All(p => p.Cijena == 100));
        }

        [TestMethod]
        public void ToExpression_Moq_PozivaInnerToExpression()
        {
            var mock = new Mock<ISpecification<TestniProizvod>>();

            mock.Setup(s => s.ToExpression())
                .Returns(
                    (Expression<Func<TestniProizvod, bool>>)
                    (p => p.Cijena > 100));

            var spec = new SortiranjeSpecification<TestniProizvod>(
                mock.Object,
                p => p.Cijena);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(
                new TestniProizvod { Cijena = 200 }));

            Assert.IsFalse(predikat(
                new TestniProizvod { Cijena = 50 }));

            mock.Verify(
                s => s.ToExpression(),
                Times.Once());
        }

        [TestMethod]
        public void ToExpression_NullInner_BacaNullReferenceException()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                null!,
                p => p.Cijena);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void OrderBy_NullIzraz_VracaNull()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                null!,
                false);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        [TestMethod]
        public void OrderByDescending_NullIzraz_VracaNull()
        {
            var spec = new SortiranjeSpecification<TestniProizvod>(
                new AllSpecification<TestniProizvod>(),
                null!,
                true);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        [TestMethod]
        public void SortiranjeSpecification_ImplementiraISpecification()
        {
            var spec = KreirajSpecifikaciju();

            Assert.IsInstanceOfType(
                spec,
                typeof(ISpecification<TestniProizvod>));
        }
    }
}
