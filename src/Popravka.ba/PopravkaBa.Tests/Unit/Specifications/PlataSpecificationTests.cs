
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class PlataSpecificationTests
    {
        [DataTestMethod]
        [DataRow(1500, 2500, 2000, 3000, true)]
        [DataRow(1500, 2500, 1000, 2000, true)]
        [DataRow(1500, 2500, 1500, 2500, true)]
        [DataRow(1500, 2500, 1800, 2200, true)]
        [DataRow(1500, 2500, 1000, 3000, true)]
        [DataRow(1500, 2500, 2500, 3000, true)]
        [DataRow(1500, 2500, 1000, 1500, true)]
        [DataRow(1500, 2500, 2501, 3000, false)]
        [DataRow(1500, 2500, 1000, 1499, false)]
        public void ToExpression_RazlicitiIntervali_VracaOcekivano(
            int minPrihod,
            int maxPrihod,
            int filterOd,
            int filterDo,
            bool ocekivano)
        {
            var spec = new PlataSpecification(filterOd, filterDo);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = minPrihod,
                MaxPrihod = maxPrihod
            };

            var rezultat = spec.ToExpression().Compile()(oglas);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow(-100, -50, true)]
        [DataRow(-100, 0, false)]
        [DataRow(0, -100, false)]
        [DataRow(0, 0, false)]
        [DataRow(100, 200, false)]
        public void ToExpression_ObrnuteGranice_ProvjeravaNegativnePrihode(
            int minPrihod,
            int maxPrihod,
            bool ocekivano)
        {
            var spec = new PlataSpecification(500, 100);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = minPrihod,
                MaxPrihod = maxPrihod
            };

            var rezultat = spec.ToExpression().Compile()(oglas);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void ToExpression_JednakeGranice_PoklapanjeVracaTrue()
        {
            var spec = new PlataSpecification(2000, 2000);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = 1500,
                MaxPrihod = 2500
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_JednakeGranice_BezPoklapanjaVracaFalse()
        {
            var spec = new PlataSpecification(3000, 3000);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = 1500,
                MaxPrihod = 2500
            };

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_NultiPrihodi_VracaTrue()
        {
            var spec = new PlataSpecification(0, 0);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = 0,
                MaxPrihod = 0
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_ObrnuteGranice_PozitivniPrihodiVracaFalse()
        {
            var spec = new PlataSpecification(3000, 1000);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = 1500,
                MaxPrihod = 2500
            };

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_ObrnuteGranice_NegativniPrihodiVracaTrue()
        {
            var spec = new PlataSpecification(3000, 1000);

            var oglas = new OglasRadnoMjesto
            {
                MinPrihod = -200,
                MaxPrihod = -100
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaOdgovarajuceOglase()
        {
            var spec = new PlataSpecification(2000, 3000);

            var oglasi = new List<OglasRadnoMjesto>
            {
                new OglasRadnoMjesto
                {
                    OglasID = 1,
                    MinPrihod = 1000,
                    MaxPrihod = 1500
                },
                new OglasRadnoMjesto
                {
                    OglasID = 2,
                    MinPrihod = 1500,
                    MaxPrihod = 2500
                },
                new OglasRadnoMjesto
                {
                    OglasID = 3,
                    MinPrihod = 2500,
                    MaxPrihod = 3500
                },
                new OglasRadnoMjesto
                {
                    OglasID = 4,
                    MinPrihod = 3500,
                    MaxPrihod = 4500
                }
            };

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 2, 3 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_ObrnuteGranice_FiltriraNegativnePrihode()
        {
            var spec = new PlataSpecification(3000, 1000);

            var oglasi = new List<OglasRadnoMjesto>
            {
                new OglasRadnoMjesto
                {
                    OglasID = 1,
                    MinPrihod = -500,
                    MaxPrihod = -100
                },
                new OglasRadnoMjesto
                {
                    OglasID = 2,
                    MinPrihod = 1000,
                    MaxPrihod = 2000
                },
                new OglasRadnoMjesto
                {
                    OglasID = 3,
                    MinPrihod = -200,
                    MaxPrihod = 0
                }
            };

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec = new PlataSpecification(1000, 3000);

            var oglasi = new List<OglasRadnoMjesto>();

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void PlataSpecification_OrderByVrijednostiSuNull()
        {
            var spec = new PlataSpecification(1000, 3000);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
