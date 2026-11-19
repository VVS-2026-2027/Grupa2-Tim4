
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class AllSpecificationTests
    {
        [TestMethod]
        public void ToExpression_JedanObjekat_VracaTrue()
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(10);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(1000000)]
        public void ToExpression_RazlicitiBrojevi_UvijekVracaTrue(
            int broj)
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(broj);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_AktivanOglas_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AllSpecification<OglasUsluge>();

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Aktivan
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_NeaktivanOglas_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AllSpecification<OglasUsluge>();

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Neaktivan
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_NullObjekat_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AllSpecification<string>();

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(null!);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            var podaci = new List<int>();

            // Act
            var rezultat = podaci
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_ViseObjekata_VracaSveObjekte()
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            var podaci = new List<int>
            {
                10, 20, 30, 40, 50
            };

            // Act
            var rezultat = podaci
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            CollectionAssert.AreEqual(podaci, rezultat);
        }

        [TestMethod]
        public void ToExpression_RazlicitiStatusiOglasa_VracaSveOglase()
        {
            // Arrange
            var specifikacija =
                new AllSpecification<OglasUsluge>();

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    OglasID = 1,
                    StatusOglasa = Status.Aktivan
                },
                new OglasUsluge
                {
                    OglasID = 2,
                    StatusOglasa = Status.Neaktivan
                },
                new OglasUsluge
                {
                    OglasID = 3,
                    StatusOglasa = Status.Aktivan
                }
            };

            // Act
            var rezultat = oglasi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(3, rezultat.Count);

            CollectionAssert.AreEqual(
                new List<int> { 1, 2, 3 },
                rezultat.Select(o => o.OglasID).ToList());
        }

        [TestMethod]
        public void AllSpecification_NasljedjujeBaseSpecification()
        {
            // Arrange i Act
            var specifikacija = new AllSpecification<int>();

            // Assert
            Assert.IsInstanceOfType(
                specifikacija,
                typeof(BaseSpecification<int>));
        }

        [TestMethod]
        public void AllSpecification_ImplementiraISpecification()
        {
            // Arrange i Act
            var specifikacija = new AllSpecification<int>();

            // Assert
            Assert.IsInstanceOfType(
                specifikacija,
                typeof(ISpecification<int>));
        }

        [TestMethod]
        public void AllSpecification_OrderByJeNull()
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            // Assert
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }

        [TestMethod]
        public void ToExpression_PonovljeniPozivi_UvijekVracaTrue()
        {
            // Arrange
            var specifikacija = new AllSpecification<int>();

            var predikat = specifikacija
                .ToExpression()
                .Compile();

            // Act
            var prviRezultat = predikat(10);
            var drugiRezultat = predikat(-50);
            var treciRezultat = predikat(0);

            // Assert
            Assert.IsTrue(prviRezultat);
            Assert.IsTrue(drugiRezultat);
            Assert.IsTrue(treciRezultat);
        }
    }
}
