
using System;
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
    public class VrstaZaposlenjaSpecificationTests
    {
        private static OglasRadnoMjesto KreirajOglas(
            VrstaZaposlenja vrsta)
        {
            return new OglasRadnoMjesto
            {
                VrstaZaposlenja = vrsta
            };
        }

        [TestMethod]
        public void ToExpression_IstaVrstaZaposlenja_VracaTrue()
        {
            var vrsta = (VrstaZaposlenja)1;

            var spec = new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                vrsta);

            var oglas = KreirajOglas(vrsta);

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_RazlicitaVrstaZaposlenja_VracaFalse()
        {
            var spec = new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                (VrstaZaposlenja)1);

            var oglas = KreirajOglas((VrstaZaposlenja)2);

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_PodrazumijevanaVrijednost_VracaTrue()
        {
            var vrsta = default(VrstaZaposlenja);

            var spec = new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                vrsta);

            var oglas = KreirajOglas(vrsta);

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void ToExpression_SveDefinisaneVrijednostiEnum_RadeIspravno()
        {
            var sveVrste = Enum.GetValues<VrstaZaposlenja>();

            foreach (var vrsta in sveVrste)
            {
                var spec =
                    new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                        vrsta);

                var oglas = KreirajOglas(vrsta);

                Assert.IsTrue(
                    spec.ToExpression().Compile()(oglas),
                    $"Specifikacija nije prihvatila vrstu {vrsta}.");
            }
        }

        [TestMethod]
        public void ToExpression_SveRazliciteVrijednostiEnum_VracajuFalse()
        {
            var sveVrste = Enum.GetValues<VrstaZaposlenja>();

            foreach (var trazenaVrsta in sveVrste)
            {
                var spec =
                    new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                        trazenaVrsta);

                var predikat = spec.ToExpression().Compile();

                foreach (var stvarnaVrsta in sveVrste)
                {
                    var oglas = KreirajOglas(stvarnaVrsta);

                    Assert.AreEqual(
                        trazenaVrsta == stvarnaVrsta,
                        predikat(oglas));
                }
            }
        }

        [TestMethod]
        public void ToExpression_FiltriranjeKolekcije_VracaSamoOdgovarajuce()
        {
            var trazenaVrsta = (VrstaZaposlenja)1;
            var drugaVrsta = (VrstaZaposlenja)2;

            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    trazenaVrsta);

            var oglasi = new List<OglasRadnoMjesto>
            {
                new OglasRadnoMjesto
                {
                    OglasID = 1,
                    VrstaZaposlenja = trazenaVrsta
                },
                new OglasRadnoMjesto
                {
                    OglasID = 2,
                    VrstaZaposlenja = drugaVrsta
                },
                new OglasRadnoMjesto
                {
                    OglasID = 3,
                    VrstaZaposlenja = trazenaVrsta
                },
                new OglasRadnoMjesto
                {
                    OglasID = 4,
                    VrstaZaposlenja = drugaVrsta
                }
            };

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1, 3 },
                rezultat);
        }

        [TestMethod]
        public void ToExpression_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    (VrstaZaposlenja)1);

            var oglasi = new List<OglasRadnoMjesto>();

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_NijedanOglasNeOdgovara_VracaPraznuKolekciju()
        {
            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    (VrstaZaposlenja)1);

            var oglasi = new List<OglasRadnoMjesto>
            {
                KreirajOglas((VrstaZaposlenja)2),
                KreirajOglas((VrstaZaposlenja)3)
            };

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_SviOglasiOdgovaraju_VracaSve()
        {
            var vrsta = (VrstaZaposlenja)1;

            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    vrsta);

            var oglasi = new List<OglasRadnoMjesto>
            {
                KreirajOglas(vrsta),
                KreirajOglas(vrsta),
                KreirajOglas(vrsta)
            };

            var rezultat = oglasi
                .AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(3, rezultat.Count);
        }

        [TestMethod]
        public void ToExpression_PromjenaVrsteZaposlenja_MijenjaRezultat()
        {
            var trazenaVrsta = (VrstaZaposlenja)1;

            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    trazenaVrsta);

            var oglas = KreirajOglas((VrstaZaposlenja)2);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(oglas));

            oglas.VrstaZaposlenja = trazenaVrsta;

            Assert.IsTrue(predikat(oglas));
        }

        [TestMethod]
        public void ToExpression_NedefinisanaEnumVrijednost_RadiPoredjenje()
        {
            var nedefinisana = (VrstaZaposlenja)999;

            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    nedefinisana);

            var predikat = spec.ToExpression().Compile();

            Assert.IsTrue(predikat(KreirajOglas(nedefinisana)));

            Assert.IsFalse(predikat(
                KreirajOglas((VrstaZaposlenja)1)));
        }

        [TestMethod]
        public void VrstaZaposlenjaSpecification_NasljedjujeBaseSpecification()
        {
            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    (VrstaZaposlenja)1);

            Assert.IsInstanceOfType(
                spec,
                typeof(BaseSpecification<OglasRadnoMjesto>));
        }

        [TestMethod]
        public void VrstaZaposlenjaSpecification_ImplementiraISpecification()
        {
            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    (VrstaZaposlenja)1);

            Assert.IsInstanceOfType(
                spec,
                typeof(ISpecification<OglasRadnoMjesto>));
        }

        [TestMethod]
        public void VrstaZaposlenjaSpecification_OrderByVrijednostiSuNull()
        {
            var spec =
                new VrstaZaposlenjaSpecification<OglasRadnoMjesto>(
                    (VrstaZaposlenja)1);

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
