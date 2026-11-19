
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class KategorijaSpecificationTests
    {
        private static OglasUsluge KreirajOglas(
            int id, params int[] kategorije)
        {
            return new OglasUsluge
            {
                OglasID = id,
                Kategorije = kategorije.Select(k => new OglasKategorija
                {
                    KategorijaID = k
                }).ToList()
            };
        }

        private static Majstor KreirajMajstora(
            string id, params int[] kategorije)
        {
            return new Majstor
            {
                Id = id,
                Kategorije = kategorije.Select(k => new IzvrsilacKategorija
                {
                    KategorijaID = k
                }).ToList()
            };
        }

        // ============================================
        // KATEGORIJA OGLASA
        // ============================================

        [TestMethod]
        public void Oglas_JednaOdgovarajucaKategorija_VracaTrue()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 2 });

            var oglas = KreirajOglas(1, 2);

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_KategorijaNeOdgovara_VracaFalse()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 5 });

            var oglas = KreirajOglas(1, 2);

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_ViseKategorija_JednaOdgovara_VracaTrue()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 4, 5 });

            var oglas = KreirajOglas(1, 1, 2, 5);

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_ViseKategorija_NijednaNeOdgovara_VracaFalse()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 7, 8 });

            var oglas = KreirajOglas(1, 1, 2, 3);

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_PraznaListaKategorija_VracaFalse()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 1 });

            var oglas = KreirajOglas(1);

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_PrazanFilter_VracaFalse()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int>());

            var oglas = KreirajOglas(1, 1, 2);

            Assert.IsFalse(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_NullKategorije_BacaArgumentNullException()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 1 });

            var oglas = new OglasUsluge
            {
                Kategorije = null
            };

            Assert.ThrowsException<ArgumentNullException>(
                () => spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_NullFilter_BacaNullReferenceException()
        {
            // Trenutna implementacija ne provjerava null filter.
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                null!);

            var oglas = KreirajOglas(1, 1);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_RadnoMjesto_FiltriraKategorije()
        {
            var spec =
                new KategorijaOglasSpecification<OglasRadnoMjesto>(
                    new List<int> { 3 });

            var oglas = new OglasRadnoMjesto
            {
                Kategorije = new List<OglasKategorija>
                {
                    new OglasKategorija { KategorijaID = 3 }
                }
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_Majstora_FiltriraKategorije()
        {
            var spec =
                new KategorijaOglasSpecification<OglasMajstora>(
                    new List<int> { 2 });

            var oglas = new OglasMajstora
            {
                Kategorije = new List<OglasKategorija>
                {
                    new OglasKategorija { KategorijaID = 2 }
                }
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 2, 4 });

            var oglasi = new List<OglasUsluge>
            {
                KreirajOglas(1, 1),
                KreirajOglas(2, 2),
                KreirajOglas(3, 3),
                KreirajOglas(4, 4),
                KreirajOglas(5, 5)
            };

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 2, 4 }, rezultat);
        }

        [TestMethod]
        public void Oglas_DodavanjeKategorije_MijenjaRezultat()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 5 });

            var oglas = KreirajOglas(1, 1);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(oglas));

            ((List<OglasKategorija>)oglas.Kategorije!)
                .Add(new OglasKategorija { KategorijaID = 5 });

            Assert.IsTrue(predikat(oglas));
        }

        [TestMethod]
        public void Oglas_OrderByVrijednostiSuNull()
        {
            var spec = new KategorijaOglasSpecification<OglasUsluge>(
                new List<int> { 1 });

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        // ============================================
        // KATEGORIJA IZVRSIOCA
        // ============================================

        [TestMethod]
        public void Izvrsilac_JednaOdgovarajucaKategorija_VracaTrue()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 2 });

            var majstor = KreirajMajstora("1", 2);

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_KategorijaNeOdgovara_VracaFalse()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 5 });

            var majstor = KreirajMajstora("1", 2);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_ViseKategorija_JednaOdgovara_VracaTrue()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 3, 5 });

            var majstor = KreirajMajstora("1", 1, 2, 5);

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_ViseKategorija_NijednaNeOdgovara_VracaFalse()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 7, 8 });

            var majstor = KreirajMajstora("1", 1, 2, 3);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_PrazneKategorije_VracaFalse()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 1 });

            var majstor = KreirajMajstora("1");

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_PrazanFilter_VracaFalse()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int>());

            var majstor = KreirajMajstora("1", 1, 2);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_NullKategorije_BacaArgumentNullException()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 1 });

            var majstor = new Majstor
            {
                Kategorije = null!
            };

            Assert.ThrowsException<ArgumentNullException>(
                () => spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_NullFilter_BacaNullReferenceException()
        {
            // Trenutna implementacija ne provjerava null filter.
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    null!);

            var majstor = KreirajMajstora("1", 1);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_Firma_FiltriraKategorije()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Firma>(
                    new List<int> { 3 });

            var firma = new Firma
            {
                Kategorije = new List<IzvrsilacKategorija>
                {
                    new IzvrsilacKategorija { KategorijaID = 3 }
                }
            };

            Assert.IsTrue(spec.ToExpression().Compile()(firma));
        }

        [TestMethod]
        public void Izvrsilac_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 2, 4 });

            var majstori = new List<Majstor>
            {
                KreirajMajstora("1", 1),
                KreirajMajstora("2", 2),
                KreirajMajstora("3", 3),
                KreirajMajstora("4", 4)
            };

            var rezultat = majstori.AsQueryable()
                .Where(spec.ToExpression())
                .Select(m => m.Id)
                .ToList();

            CollectionAssert.AreEqual(
                new List<string> { "2", "4" }, rezultat);
        }

        [TestMethod]
        public void Izvrsilac_DodavanjeKategorije_MijenjaRezultat()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 5 });

            var majstor = KreirajMajstora("1", 1);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(majstor));

            majstor.Kategorije.Add(
                new IzvrsilacKategorija { KategorijaID = 5 });

            Assert.IsTrue(predikat(majstor));
        }

        [TestMethod]
        public void Izvrsilac_OrderByVrijednostiSuNull()
        {
            var spec =
                new KategorijaIzvrsilacSpecification<Majstor>(
                    new List<int> { 1 });

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
