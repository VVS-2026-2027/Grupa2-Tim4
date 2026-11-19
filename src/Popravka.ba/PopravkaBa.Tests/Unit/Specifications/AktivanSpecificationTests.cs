
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;
using PopravkaBa.Domain.Specifications.Interface;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class AktivanSpecificationTests
    {
        [TestMethod]
        public void ToExpression_AktivanOglasUsluge_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

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
        public void ToExpression_NeaktivanOglasUsluge_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Neaktivan
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_NoviOglas_PodrazumijevanoJeAktivan()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            var oglas = new OglasUsluge();

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_AktivanOglasRadnoMjesto_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasRadnoMjesto>();

            var oglas = new OglasRadnoMjesto
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
        public void ToExpression_NeaktivanOglasRadnoMjesto_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasRadnoMjesto>();

            var oglas = new OglasRadnoMjesto
            {
                StatusOglasa = Status.Neaktivan
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_AktivanOglasMajstora_VracaTrue()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasMajstora>();

            var oglas = new OglasMajstora
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
        public void ToExpression_NeaktivanOglasMajstora_VracaFalse()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasMajstora>();

            var oglas = new OglasMajstora
            {
                StatusOglasa = Status.Neaktivan
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_PromjenaStatusa_MijenjaRezultat()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Aktivan
            };

            var predikat = specifikacija
                .ToExpression()
                .Compile();

            // Act
            var prijePromjene = predikat(oglas);

            oglas.StatusOglasa = Status.Neaktivan;

            var poslijePromjene = predikat(oglas);

            // Assert
            Assert.IsTrue(prijePromjene);
            Assert.IsFalse(poslijePromjene);
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaSamoAktivne()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

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
                },
                new OglasUsluge
                {
                    OglasID = 4,
                    StatusOglasa = Status.Neaktivan
                }
            };

            // Act
            var rezultat = oglasi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(2, rezultat.Count);

            CollectionAssert.AreEquivalent(
                new List<int> { 1, 3 },
                rezultat.Select(o => o.OglasID).ToList());

            Assert.IsTrue(
                rezultat.All(
                    o => o.StatusOglasa == Status.Aktivan));
        }

        [TestMethod]
        public void ToExpression_SviOglasiNeaktivni_VracaPraznuKolekciju()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    StatusOglasa = Status.Neaktivan
                },
                new OglasUsluge
                {
                    StatusOglasa = Status.Neaktivan
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
        public void ToExpression_SviOglasiAktivni_VracaSveOglase()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

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
                    StatusOglasa = Status.Aktivan
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
        }

        [TestMethod]
        public void AktivanSpecification_NasljedjujeBaseSpecification()
        {
            // Arrange i Act
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            // Assert
            Assert.IsInstanceOfType(
                specifikacija,
                typeof(BaseSpecification<OglasUsluge>));
        }

        [TestMethod]
        public void AktivanSpecification_OrderByJeNull()
        {
            // Arrange
            var specifikacija =
                new AktivanSpecification<OglasUsluge>();

            // Assert
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
