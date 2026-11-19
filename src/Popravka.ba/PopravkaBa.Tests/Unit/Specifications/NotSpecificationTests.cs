
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class NotSpecificationTests
    {
        private sealed class TestnaSpecifikacija<T>
            : BaseSpecification<T>
        {
            private readonly Expression<Func<T, bool>> _izraz;

            public TestnaSpecifikacija(
                Expression<Func<T, bool>> izraz)
            {
                _izraz = izraz;
            }

            public override Expression<Func<T, bool>> ToExpression()
            {
                return _izraz;
            }
        }

        private sealed class TestniProizvod
        {
            public int Cijena { get; set; }
            public bool Aktivan { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }

        [DataTestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        public void ToExpression_NegiraLogickuVrijednost(
            bool vrijednost,
            bool ocekivano)
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => vrijednost);

            var spec = new NotSpecification<int>(osnovna);

            var rezultat = spec.ToExpression().Compile()(10);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow(5, true)]
        [DataRow(10, true)]
        [DataRow(11, false)]
        [DataRow(15, false)]
        [DataRow(100, false)]
        public void ToExpression_NegacijaVeceOdDeset_VracaOcekivano(
            int broj,
            bool ocekivano)
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 10);

            var spec = new NotSpecification<int>(osnovna);

            var rezultat = spec.ToExpression().Compile()(broj);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_OsnovniUslovTrue_VracaFalse()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 0);

            var spec = new NotSpecification<int>(osnovna);

            Assert.IsFalse(spec.ToExpression().Compile()(5));
        }

        [TestMethod]
        public void ToExpression_OsnovniUslovFalse_VracaTrue()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 0);

            var spec = new NotSpecification<int>(osnovna);

            Assert.IsTrue(spec.ToExpression().Compile()(-5));
        }

        [TestMethod]
        public void ToExpression_BooleanSvojstvo_NegiraVrijednost()
        {
            var osnovna =
                new TestnaSpecifikacija<TestniProizvod>(
                    p => p.Aktivan);

            var spec =
                new NotSpecification<TestniProizvod>(osnovna);

            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Aktivan = true
            }));

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Aktivan = false
            }));
        }

        [TestMethod]
        public void ToExpression_NegacijaUslovaCijene_RadiIspravno()
        {
            var osnovna =
                new TestnaSpecifikacija<TestniProizvod>(
                    p => p.Cijena >= 100);

            var spec =
                new NotSpecification<TestniProizvod>(osnovna);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Cijena = 50
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Cijena = 100
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Cijena = 150
            }));
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaNegiraneRezultate()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 10);

            var spec = new NotSpecification<int>(osnovna);

            var brojevi = new List<int>
            {
                5, 10, 11, 15, 20
            };

            var rezultat = brojevi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 5, 10 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 10);

            var spec = new NotSpecification<int>(osnovna);

            var rezultat = new List<int>()
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_DvostrukaNegacija_VracaOriginalniRezultat()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 10);

            var prvaNegacija =
                new NotSpecification<int>(osnovna);

            var drugaNegacija =
                new NotSpecification<int>(prvaNegacija);

            var predikat = drugaNegacija
                .ToExpression()
                .Compile();

            Assert.IsFalse(predikat(5));
            Assert.IsFalse(predikat(10));
            Assert.IsTrue(predikat(11));
            Assert.IsTrue(predikat(20));
        }

        [TestMethod]
        public void ToExpression_TrostrukaNegacija_VracaNegiraniRezultat()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => x > 10);

            var spec = new NotSpecification<int>(
                new NotSpecification<int>(
                    new NotSpecification<int>(osnovna)));

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(5));
            Assert.IsFalse(predikat(15));
        }

        [TestMethod]
        public void ToExpression_KombinacijaSaAllSpecification_VracaFalse()
        {
            var osnovna = new AllSpecification<int>();

            var spec = new NotSpecification<int>(osnovna);

            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(0));
            Assert.IsFalse(predikat(10));
            Assert.IsFalse(predikat(-100));
        }

        [TestMethod]
        public void ToExpression_KombinacijaSaAndSpecification_RadiIspravno()
        {
            var lijevi = new TestnaSpecifikacija<int>(
                x => x > 10);

            var desni = new TestnaSpecifikacija<int>(
                x => x < 20);

            var andSpec = new AndSpecification<int>(
                lijevi, desni);

            var notSpec = new NotSpecification<int>(andSpec);

            var predikat = notSpec.ToExpression().Compile();

            Assert.IsTrue(predikat(5));
            Assert.IsFalse(predikat(15));
            Assert.IsTrue(predikat(25));
        }

        [TestMethod]
        public void ToExpression_PromjenaVrijednosti_MijenjaRezultat()
        {
            var osnovna =
                new TestnaSpecifikacija<TestniProizvod>(
                    p => p.Cijena > 100);

            var spec =
                new NotSpecification<TestniProizvod>(osnovna);

            var proizvod = new TestniProizvod
            {
                Cijena = 50
            };

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(proizvod));

            proizvod.Cijena = 150;

            Assert.IsFalse(predikat(proizvod));
        }

        [TestMethod]
        public void ToExpression_NullSpecifikacija_BacaNullReferenceException()
        {
            var spec = new NotSpecification<int>(null!);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void NotSpecification_NasljedjujeBaseSpecification()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => true);

            var spec = new NotSpecification<int>(osnovna);

            Assert.IsInstanceOfType(
                spec,
                typeof(BaseSpecification<int>));
        }

        [TestMethod]
        public void NotSpecification_ImplementiraISpecification()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => true);

            var spec = new NotSpecification<int>(osnovna);

            Assert.IsInstanceOfType(
                spec,
                typeof(ISpecification<int>));
        }

        [TestMethod]
        public void NotSpecification_OrderByVrijednostiSuNull()
        {
            var osnovna = new TestnaSpecifikacija<int>(
                x => true);

            var spec = new NotSpecification<int>(osnovna);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
