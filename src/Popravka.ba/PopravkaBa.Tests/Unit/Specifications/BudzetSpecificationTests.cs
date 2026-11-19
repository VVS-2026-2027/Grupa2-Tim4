
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class BudzetSpecificationTests
    {
        [DataTestMethod]
        [DataRow(200, 500, 400, 800, true)]
        [DataRow(200, 500, 100, 300, true)]
        [DataRow(200, 500, 200, 500, true)]
        [DataRow(200, 500, 250, 450, true)]
        [DataRow(200, 500, 100, 800, true)]
        [DataRow(200, 500, 500, 800, true)]
        [DataRow(200, 500, 100, 200, true)]
        [DataRow(200, 500, 501, 800, false)]
        [DataRow(200, 500, 100, 199, false)]
        public void ToExpression_RazlicitiIntervali_VracaOcekivaniRezultat(
            int minBudzet,
            int maxBudzet,
            int filterOd,
            int filterDo,
            bool ocekivano)
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(filterOd, filterDo);

            var oglas = new OglasUsluge
            {
                MinBudzet = minBudzet,
                MaxBudzet = maxBudzet
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_ObrnuteGraniceFiltera_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(800, 200);

            var oglas = new OglasUsluge
            {
                MinBudzet = 100,
                MaxBudzet = 1000
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_ObrnuteGraniceFiltera_UvijekVracaFalse()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(500, 100);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    MinBudzet = 100,
                    MaxBudzet = 500
                },
                new OglasUsluge
                {
                    MinBudzet = 0,
                    MaxBudzet = 1000
                }
            };

            // Act
            var rezultat = oglasi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_JednakeGraniceFiltera_PoklapanjeVracaTrue()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(300, 300);

            var oglas = new OglasUsluge
            {
                MinBudzet = 100,
                MaxBudzet = 500
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_JednakeGraniceFiltera_BezPoklapanjaVracaFalse()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(600, 600);

            var oglas = new OglasUsluge
            {
                MinBudzet = 100,
                MaxBudzet = 500
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_NultiBudzet_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(0, 0);

            var oglas = new OglasUsluge
            {
                MinBudzet = 0,
                MaxBudzet = 0
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_NegativneVrijednosti_PorediIntervale()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(-100, 100);

            var oglas = new OglasUsluge
            {
                MinBudzet = -50,
                MaxBudzet = 50
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaSamoOdgovarajuceOglase()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(300, 600);

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    OglasID = 1,
                    MinBudzet = 100,
                    MaxBudzet = 200
                },
                new OglasUsluge
                {
                    OglasID = 2,
                    MinBudzet = 200,
                    MaxBudzet = 400
                },
                new OglasUsluge
                {
                    OglasID = 3,
                    MinBudzet = 500,
                    MaxBudzet = 800
                },
                new OglasUsluge
                {
                    OglasID = 4,
                    MinBudzet = 700,
                    MaxBudzet = 900
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
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(100, 500);

            var oglasi = new List<OglasUsluge>();

            // Act
            var rezultat = oglasi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void BudzetSpecification_NasljedjujeBaseSpecification()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(100, 500);

            // Assert
            Assert.IsInstanceOfType(
                specifikacija,
                typeof(BaseSpecification<OglasUsluge>));
        }

        [TestMethod]
        public void BudzetSpecification_OrderByVrijednostiSuNull()
        {
            // Arrange
            var specifikacija =
                new BudzеtSpecification(100, 500);

            // Assert
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
