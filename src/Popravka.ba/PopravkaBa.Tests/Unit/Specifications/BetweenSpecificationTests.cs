
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class BetweenSpecificationTests
    {
        // Pomoćni model za testiranje različitih tipova.
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
        [DataRow(100, true)]
        [DataRow(101, true)]
        [DataRow(250, true)]
        [DataRow(499, true)]
        [DataRow(500, true)]
        [DataRow(501, false)]
        [DataRow(600, false)]
        public void ToExpression_RazliciteCijene_VracaOcekivaniRezultat(
            int cijena,
            bool ocekivano)
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvod = new TestniProizvod
            {
                Cijena = cijena
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_DonjaGranicaJeUkljucena_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

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
        public void ToExpression_GornjaGranicaJeUkljucena_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvod = new TestniProizvod
            {
                Cijena = 500
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [DataTestMethod]
        [DataRow(-100, true)]
        [DataRow(-50, true)]
        [DataRow(0, true)]
        [DataRow(1, false)]
        [DataRow(-101, false)]
        public void ToExpression_NegativneGranice_VracaOcekivaniRezultat(
            int cijena,
            bool ocekivano)
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    -100,
                    0);

            var proizvod = new TestniProizvod
            {
                Cijena = cijena
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_JednakeGranice_VrijednostNaGraniciVracaTrue()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    200,
                    200);

            var proizvod = new TestniProizvod
            {
                Cijena = 200
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_JednakeGranice_DrugaVrijednostVracaFalse()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    200,
                    200);

            var proizvod = new TestniProizvod
            {
                Cijena = 201
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_ObrnuteGranice_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    500,
                    100);

            var proizvod = new TestniProizvod
            {
                Cijena = 250
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_DecimalneVrijednosti_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, decimal>(
                    p => p.Tezina,
                    1.5m,
                    5.5m);

            var proizvod = new TestniProizvod
            {
                Tezina = 3.25m
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_DecimalnaVrijednostIzvanRaspona_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, decimal>(
                    p => p.Tezina,
                    1.5m,
                    5.5m);

            var proizvod = new TestniProizvod
            {
                Tezina = 5.6m
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_DatumUnutarRaspona_VracaTrue()
        {
            // Arrange
            var od = new DateTime(2026, 1, 1);
            var doDatum = new DateTime(2026, 12, 31);

            var specifikacija =
                new BetweenSpecification<TestniProizvod, DateTime>(
                    p => p.Datum,
                    od,
                    doDatum);

            var proizvod = new TestniProizvod
            {
                Datum = new DateTime(2026, 6, 15)
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_DatumIzvanRaspona_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, DateTime>(
                    p => p.Datum,
                    new DateTime(2026, 1, 1),
                    new DateTime(2026, 12, 31));

            var proizvod = new TestniProizvod
            {
                Datum = new DateTime(2027, 1, 1)
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(proizvod);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaSamoVrijednostiURasponu()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 100 },
                new TestniProizvod { Cijena = 250 },
                new TestniProizvod { Cijena = 500 },
                new TestniProizvod { Cijena = 600 }
            };

            // Act
            var rezultat = proizvodi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(p => p.Cijena)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(
                new List<int> { 100, 250, 500 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvodi = new List<TestniProizvod>();

            // Act
            var rezultat = proizvodi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_SviObjektiIzvanRaspona_VracaPraznuKolekciju()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvodi = new List<TestniProizvod>
            {
                new TestniProizvod { Cijena = 10 },
                new TestniProizvod { Cijena = 50 },
                new TestniProizvod { Cijena = 600 }
            };

            // Act
            var rezultat = proizvodi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_OglasUsluge_FiltriraPremaMinBudzetu()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<OglasUsluge, int>(
                    o => o.MinBudzet,
                    100,
                    500);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    OglasID = 1,
                    MinBudzet = 50
                },
                new OglasUsluge
                {
                    OglasID = 2,
                    MinBudzet = 100
                },
                new OglasUsluge
                {
                    OglasID = 3,
                    MinBudzet = 300
                },
                new OglasUsluge
                {
                    OglasID = 4,
                    MinBudzet = 700
                }
            };

            // Act
            var rezultat = oglasi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            // Assert
            CollectionAssert.AreEqual(
                new List<int> { 2, 3 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PromjenaVrijednosti_AzuriraRezultat()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            var proizvod = new TestniProizvod
            {
                Cijena = 50
            };

            var predikat = specifikacija
                .ToExpression()
                .Compile();

            // Act
            var prije = predikat(proizvod);

            proizvod.Cijena = 250;

            var poslije = predikat(proizvod);

            // Assert
            Assert.IsFalse(prije);
            Assert.IsTrue(poslije);
        }

        [TestMethod]
        public void ToExpression_StringBezOperatoraPoredjenja_BacaInvalidOperationException()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, string>(
                    p => p.Naziv,
                    "A",
                    "Z");

            // Act i Assert
            Assert.ThrowsException<InvalidOperationException>(
                () => specifikacija.ToExpression());
        }

        [TestMethod]
        public void BetweenSpecification_OrderByVrijednostiSuNull()
        {
            // Arrange
            var specifikacija =
                new BetweenSpecification<TestniProizvod, int>(
                    p => p.Cijena,
                    100,
                    500);

            // Assert
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
