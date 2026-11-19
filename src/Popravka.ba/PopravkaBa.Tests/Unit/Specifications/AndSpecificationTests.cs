
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Interface;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class AndSpecificationTests
    {
        // Pomoćna specifikacija za testiranje.
        // Omogućava definisanje različitih uslova.
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

        private static ISpecification<int> Uslov(
            Expression<Func<int, bool>> izraz)
        {
            return new TestnaSpecifikacija<int>(izraz);
        }

        [DataTestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(false, false, false)]
        public void ToExpression_KombinacijeUslova_VracaOcekivaniRezultat(
            bool lijeviRezultat,
            bool desniRezultat,
            bool ocekivano)
        {
            // Arrange
            var lijevi = Uslov(x => lijeviRezultat);
            var desni = Uslov(x => desniRezultat);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(10);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_BrojIspunjavaObaUslova_VracaTrue()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(15);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_BrojNeIspunjavaPrviUslov_VracaFalse()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(5);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_BrojNeIspunjavaDrugiUslov_VracaFalse()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(25);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_DonjaGranicaNijeUkljucena_VracaFalse()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(10);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_GornjaGranicaNijeUkljucena_VracaFalse()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(20);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaSamoOdgovarajuceBrojeve()
        {
            // Arrange
            var lijevi = Uslov(x => x > 10);
            var desni = Uslov(x => x < 20);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            var brojevi = new List<int>
            {
                5, 10, 11, 15, 19, 20, 25
            };

            // Act
            var rezultat = brojevi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            CollectionAssert.AreEqual(
                new List<int> { 11, 15, 19 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_NijedanObjekatNeIspunjavaUslove_VracaPraznuKolekciju()
        {
            // Arrange
            var lijevi = Uslov(x => x > 100);
            var desni = Uslov(x => x < 200);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            var brojevi = new List<int> { 1, 2, 3, 4 };

            // Act
            var rezultat = brojevi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            // Arrange
            var lijevi = Uslov(x => x > 0);
            var desni = Uslov(x => x < 100);

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            var brojevi = new List<int>();

            // Act
            var rezultat = brojevi
                .AsQueryable()
                .Where(specifikacija.ToExpression())
                .ToList();

            // Assert
            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_PrviUslovFalse_NeIzvrsavaDrugiUslov()
        {
            // Arrange
            var lijevi = Uslov(x => false);

            // Drugi izraz baca izuzetak ako se izvrsi.
            var desni = Uslov(
                x => ProvjeraKojaBacaIzuzetak(x));

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(10);

            // Assert
            Assert.IsFalse(rezultat);
        }

        private static bool ProvjeraKojaBacaIzuzetak(int broj)
        {
            throw new InvalidOperationException(
                "Drugi uslov ne smije biti izvrsen.");
        }

        [TestMethod]
        public void ToExpression_PrviUslovTrue_IzvrsavaDrugiUslov()
        {
            // Arrange
            var lijevi = Uslov(x => true);

            var desni = Uslov(
                x => ProvjeraKojaBacaIzuzetak(x));

            var specifikacija = new AndSpecification<int>(
                lijevi,
                desni);

            var predikat = specifikacija
                .ToExpression()
                .Compile();

            // Act i Assert
            Assert.ThrowsException<InvalidOperationException>(
                () => predikat(10));
        }

        [TestMethod]
        public void ToExpression_KombinovanjeAktivnogStatusaIBudzeta_VracaTrue()
        {
            // Arrange
            var lijevi =
                new AktivanSpecification<OglasUsluge>();

            var desni = new TestnaSpecifikacija<OglasUsluge>(
                oglas => oglas.MinBudzet >= 100);

            var specifikacija =
                new AndSpecification<OglasUsluge>(
                    lijevi,
                    desni);

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Aktivan,
                MinBudzet = 150
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_AktivanOglasAliNedovoljanBudzet_VracaFalse()
        {
            // Arrange
            var lijevi =
                new AktivanSpecification<OglasUsluge>();

            var desni = new TestnaSpecifikacija<OglasUsluge>(
                oglas => oglas.MinBudzet >= 100);

            var specifikacija =
                new AndSpecification<OglasUsluge>(
                    lijevi,
                    desni);

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Aktivan,
                MinBudzet = 50
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_NeaktivanOglasSaDovoljnimBudzetom_VracaFalse()
        {
            // Arrange
            var lijevi =
                new AktivanSpecification<OglasUsluge>();

            var desni = new TestnaSpecifikacija<OglasUsluge>(
                oglas => oglas.MinBudzet >= 100);

            var specifikacija =
                new AndSpecification<OglasUsluge>(
                    lijevi,
                    desni);

            var oglas = new OglasUsluge
            {
                StatusOglasa = Status.Neaktivan,
                MinBudzet = 150
            };

            // Act
            var rezultat = specifikacija
                .ToExpression()
                .Compile()(oglas);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ToExpression_UgnijezdeneSpecifikacije_RadeIspravno()
        {
            // Arrange
            var veceOdDeset = Uslov(x => x > 10);
            var manjeOdSto = Uslov(x => x < 100);
            var paranBroj = Uslov(x => x % 2 == 0);

            var prviAnd = new AndSpecification<int>(
                veceOdDeset,
                manjeOdSto);

            var drugiAnd = new AndSpecification<int>(
                prviAnd,
                paranBroj);

            var predikat = drugiAnd
                .ToExpression()
                .Compile();

            // Act i Assert
            Assert.IsTrue(predikat(20));
            Assert.IsFalse(predikat(21));
            Assert.IsFalse(predikat(8));
            Assert.IsFalse(predikat(120));
        }

        [TestMethod]
        public void AndSpecification_NasljedjujeBaseSpecification()
        {
            // Arrange
            var specifikacija = new AndSpecification<int>(
                Uslov(x => true),
                Uslov(x => true));

            // Assert
            Assert.IsInstanceOfType(
                specifikacija,
                typeof(BaseSpecification<int>));
        }

        [TestMethod]
        public void AndSpecification_OrderByJeNull()
        {
            // Arrange
            var specifikacija = new AndSpecification<int>(
                Uslov(x => true),
                Uslov(x => true));

            // Assert
            Assert.IsNull(specifikacija.OrderBy);
            Assert.IsNull(specifikacija.OrderByDescending);
        }
    }
}
