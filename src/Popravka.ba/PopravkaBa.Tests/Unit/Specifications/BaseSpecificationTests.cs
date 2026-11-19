
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Specifications.Interface;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class BaseSpecificationTests
    {
        // Pomoćni model za testiranje specifikacije.
        private sealed class TestniProizvod
        {
            public int Cijena { get; set; }
            public string Naziv { get; set; } = string.Empty;
        }

        // Konkretna implementacija apstraktne klase.
        private sealed class TestnaSpecifikacija
            : BaseSpecification<TestniProizvod>
        {
            public override Expression<Func<TestniProizvod, bool>>
                ToExpression()
            {
                return proizvod => proizvod.Cijena >= 100;
            }
        }

        [TestMethod]
        public void NovaSpecifikacija_OrderByJeNull()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            // Act
            var rezultat = specifikacija.OrderBy;

            // Assert
            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public void NovaSpecifikacija_OrderByDescendingJeNull()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            // Act
            var rezultat = specifikacija.OrderByDescending;

            // Assert
            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public void ToExpression_ProizvodIspunjavaUslov_VracaTrue()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            var proizvod = new TestniProizvod
            {
                Cijena = 150,
                Naziv = "Proizvod A"
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_ProizvodNeIspunjavaUslov_VracaFalse()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            var proizvod = new TestniProizvod
            {
                Cijena = 50,
                Naziv = "Proizvod B"
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_GranicnaVrijednost_VracaTrue()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            var proizvod = new TestniProizvod
            {
                Cijena = 100
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaOdgovarajuceProizvode()
        {
            // Arrange
            var specifikacija = new TestnaSpecifikacija();

            var proizvodi = new[]
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 150 },
                new TestniProizvod { Cijena = 200 }
            };

            // Act
            var rezultat = proizvodi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(3, rezultat.Count);
            Assert.IsTrue(rezultat.All(p => p.Cijena >= 100));
        }

        [TestMethod]
        public void BaseSpecification_PristupKrozBazniTip_RadiIspravno()
        {
            // Arrange
            BaseSpecification<TestniProizvod> specifikacija =
                new TestnaSpecifikacija();

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(new TestniProizvod
                {
                    Cijena = 120
                });

            // Assert
            Assert.IsTrue(rezultat);
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
