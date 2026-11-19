
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
    public class OrSpecificationTests
    {
        private sealed class TestnaSpecifikacija<T> : BaseSpecification<T>
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
        }

        private static ISpecification<int> Uslov(
            Expression<Func<int, bool>> izraz)
        {
            return new TestnaSpecifikacija<int>(izraz);
        }

        [DataTestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(false, false, false)]
        public void ToExpression_SveKombinacijeOR_VracaOcekivano(
            bool lijeviRezultat,
            bool desniRezultat,
            bool ocekivano)
        {
            // Arrange
            var lijevi = Uslov(x => lijeviRezultat);
            var desni = Uslov(x => desniRezultat);

            var spec = new OrSpecification<int>(lijevi, desni);

            // Act
            var rezultat = spec.ToExpression().Compile()(10);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow(5, true)]
        [DataRow(10, false)]
        [DataRow(15, false)]
        [DataRow(20, false)]
        [DataRow(25, true)]
        public void ToExpression_BrojManjiOdDesetIliVeciOdDvadeset(
            int broj,
            bool ocekivano)
        {
            var lijevi = Uslov(x => x < 10);
            var desni = Uslov(x => x > 20);

            var spec = new OrSpecification<int>(lijevi, desni);

            Assert.AreEqual(
                ocekivano,
                spec.ToExpression().Compile()(broj));
        }

        [TestMethod]
        public void ToExpression_PrviUslovTrue_VracaTrue()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x > 10),
                Uslov(x => x < 0));

            Assert.IsTrue(spec.ToExpression().Compile()(15));
        }

        [TestMethod]
        public void ToExpression_DrugiUslovTrue_VracaTrue()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x > 10),
                Uslov(x => x < 0));

            Assert.IsTrue(spec.ToExpression().Compile()(-5));
        }

        [TestMethod]
        public void ToExpression_ObaUslovaTrue_VracaTrue()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x > 0),
                Uslov(x => x < 20));

            Assert.IsTrue(spec.ToExpression().Compile()(10));
        }

        [TestMethod]
        public void ToExpression_ObaUslovaFalse_VracaFalse()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x > 10),
                Uslov(x => x < 0));

            Assert.IsFalse(spec.ToExpression().Compile()(5));
        }

        [TestMethod]
        public void ToExpression_PrviUslovTrue_NeIzvrsavaDrugi()
        {
            // Provjera kratkospojnog OR operatora.
            var lijevi = Uslov(x => true);

            var desni = Uslov(
                x => ProvjeraKojaBacaIzuzetak(x));

            var spec = new OrSpecification<int>(lijevi, desni);

            // Drugi uslov se ne smije izvrsiti.
            var rezultat = spec.ToExpression().Compile()(10);

            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ToExpression_PrviUslovFalse_IzvrsavaDrugi()
        {
            var lijevi = Uslov(x => false);

            var desni = Uslov(
                x => ProvjeraKojaBacaIzuzetak(x));

            var spec = new OrSpecification<int>(lijevi, desni);

            var predikat = spec.ToExpression().Compile();

            Assert.ThrowsException<InvalidOperationException>(
                () => predikat(10));
        }

        private static bool ProvjeraKojaBacaIzuzetak(int broj)
        {
            throw new InvalidOperationException(
                "Drugi uslov je izvrsen.");
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x < 10),
                Uslov(x => x > 20));

            var brojevi = new List<int>
            {
                5, 10, 15, 20, 25, 30
            };

            var rezultat = brojevi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 5, 25, 30 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x < 10),
                Uslov(x => x > 20));

            var rezultat = new List<int>()
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_NijedanElementNeIspunjavaUslove()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x < 0),
                Uslov(x => x > 100));

            var brojevi = new List<int> { 10, 20, 30 };

            var rezultat = brojevi.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_SviElementiIspunjavajuBaremJedanUslov()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => x <= 10),
                Uslov(x => x > 10));

            var brojevi = new List<int> { -5, 0, 10, 11, 20 };

            var rezultat = brojevi.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            CollectionAssert.AreEqual(brojevi, rezultat);
        }

        [TestMethod]
        public void ToExpression_BooleanSvojstva_RadeIspravno()
        {
            var lijevi = new TestnaSpecifikacija<TestniProizvod>(
                p => p.Aktivan);

            var desni = new TestnaSpecifikacija<TestniProizvod>(
                p => p.Cijena < 100);

            var spec =
                new OrSpecification<TestniProizvod>(lijevi, desni);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Aktivan = true,
                Cijena = 200
            }));

            Assert.IsTrue(predikat(new TestniProizvod
            {
                Aktivan = false,
                Cijena = 50
            }));

            Assert.IsFalse(predikat(new TestniProizvod
            {
                Aktivan = false,
                Cijena = 200
            }));
        }

        [TestMethod]
        public void ToExpression_UgnijezdeniOR_RadiIspravno()
        {
            var prvi = Uslov(x => x < 0);
            var drugi = Uslov(x => x == 10);
            var treci = Uslov(x => x > 100);

            var unutrasnji = new OrSpecification<int>(
                prvi, drugi);

            var vanjski = new OrSpecification<int>(
                unutrasnji, treci);

            var predikat = vanjski.ToExpression().Compile();

            Assert.IsTrue(predikat(-5));
            Assert.IsTrue(predikat(10));
            Assert.IsTrue(predikat(150));
            Assert.IsFalse(predikat(50));
        }

        [TestMethod]
        public void ToExpression_KombinacijaSaAndSpecification()
        {
            // (x > 10 && x < 20) || x == 100
            var andSpec = new AndSpecification<int>(
                Uslov(x => x > 10),
                Uslov(x => x < 20));

            var spec = new OrSpecification<int>(
                andSpec,
                Uslov(x => x == 100));

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(15));
            Assert.IsTrue(predikat(100));
            Assert.IsFalse(predikat(5));
            Assert.IsFalse(predikat(25));
        }

        [TestMethod]
        public void ToExpression_KombinacijaSaNotSpecification()
        {
            // !(x > 10) || x == 100
            var notSpec = new NotSpecification<int>(
                Uslov(x => x > 10));

            var spec = new OrSpecification<int>(
                notSpec,
                Uslov(x => x == 100));

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(5));
            Assert.IsTrue(predikat(100));
            Assert.IsFalse(predikat(20));
        }

        [TestMethod]
        public void ToExpression_KombinacijaSaAllSpecification_UvijekTrue()
        {
            var spec = new OrSpecification<int>(
                new AllSpecification<int>(),
                Uslov(x => false));

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(-100));
            Assert.IsTrue(predikat(0));
            Assert.IsTrue(predikat(100));
        }

        [TestMethod]
        public void ToExpression_PromjenaVrijednosti_MijenjaRezultat()
        {
            var spec = new OrSpecification<TestniProizvod>(
                new TestnaSpecifikacija<TestniProizvod>(
                    p => p.Cijena < 100),
                new TestnaSpecifikacija<TestniProizvod>(
                    p => p.Aktivan));

            var proizvod = new TestniProizvod
            {
                Cijena = 200,
                Aktivan = false
            };

            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(proizvod));

            proizvod.Aktivan = true;

            Assert.IsTrue(predikat(proizvod));

            proizvod.Aktivan = false;
            proizvod.Cijena = 50;

            Assert.IsTrue(predikat(proizvod));
        }

        [TestMethod]
        public void ToExpression_MoqSpecifikacije_PozivaObaToExpression()
        {
            // Arrange
            var lijeviMock = new Mock<ISpecification<int>>();
            var desniMock = new Mock<ISpecification<int>>();

            lijeviMock
                .Setup(s => s.ToExpression())
                .Returns((Expression<Func<int, bool>>)(x => x > 10));

            desniMock
                .Setup(s => s.ToExpression())
                .Returns((Expression<Func<int, bool>>)(x => x < 0));

            var spec = new OrSpecification<int>(
                lijeviMock.Object,
                desniMock.Object);

            // Act
            var predikat = spec.ToExpression().Compile();

            // Assert
            Assert.IsTrue(predikat(15));
            Assert.IsTrue(predikat(-5));
            Assert.IsFalse(predikat(5));

            lijeviMock.Verify(
                s => s.ToExpression(), Times.Once);

            desniMock.Verify(
                s => s.ToExpression(), Times.Once);
        }

        [TestMethod]
        public void ToExpression_NullLijevaSpecifikacija_BacaNullReferenceException()
        {
            var spec = new OrSpecification<int>(
                null!,
                Uslov(x => true));

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void ToExpression_NullDesnaSpecifikacija_BacaNullReferenceException()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => true),
                null!);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression());
        }

        [TestMethod]
        public void OrSpecification_NasljedjujeBaseSpecification()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => true),
                Uslov(x => false));

            Assert.IsInstanceOfType(
                spec,
                typeof(BaseSpecification<int>));
        }

        [TestMethod]
        public void OrSpecification_ImplementiraISpecification()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => true),
                Uslov(x => false));

            Assert.IsInstanceOfType(
                spec,
                typeof(ISpecification<int>));
        }

        [TestMethod]
        public void OrSpecification_OrderByVrijednostiSuNull()
        {
            var spec = new OrSpecification<int>(
                Uslov(x => true),
                Uslov(x => false));

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
