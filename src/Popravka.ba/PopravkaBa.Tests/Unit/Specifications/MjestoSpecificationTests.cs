
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class MjestoSpecificationTests
    {
        private static OglasUsluge KreirajOglas(
            int id, int mjestoId)
        {
            return new OglasUsluge
            {
                OglasID = id,
                MjestoID = mjestoId
            };
        }

        private static Majstor KreirajMajstora(
            params int[] mjesta)
        {
            return new Majstor
            {
                Mjesta = mjesta.Select(id => new KorisnikMjesto
                {
                    MjestoID = id
                }).ToList()
            };
        }

        // ==========================================
        // 1. MJESTO OGLASA
        // ==========================================

        [DataTestMethod]
        [DataRow(1, true)]
        [DataRow(3, true)]
        [DataRow(5, true)]
        [DataRow(2, false)]
        [DataRow(4, false)]
        [DataRow(10, false)]
        public void Oglas_RazlicitaMjesta_VracaOcekivano(
            int mjestoId, bool ocekivano)
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 1, 3, 5 });

            var oglas = KreirajOglas(1, mjestoId);

            var rezultat = spec.ToExpression().Compile()(oglas);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void Oglas_JednoOdgovarajuceMjesto_VracaTrue()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 2 });

            Assert.IsTrue(spec.ToExpression().Compile()(
                KreirajOglas(1, 2)));
        }

        [TestMethod]
        public void Oglas_MjestoNijeUFilteru_VracaFalse()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 2, 3 });

            Assert.IsFalse(spec.ToExpression().Compile()(
                KreirajOglas(1, 5)));
        }

        [TestMethod]
        public void Oglas_PrazanFilter_VracaFalse()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int>());

            Assert.IsFalse(spec.ToExpression().Compile()(
                KreirajOglas(1, 2)));
        }

        [TestMethod]
        public void Oglas_NullFilter_BacaNullReferenceException()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                null!);

            var oglas = KreirajOglas(1, 2);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_RadnoMjesto_FiltriraMjesto()
        {
            var spec = new MjestoOglasSpecification<OglasRadnoMjesto>(
                new List<int> { 3 });

            var oglas = new OglasRadnoMjesto
            {
                MjestoID = 3
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_Majstora_FiltriraMjesto()
        {
            var spec = new MjestoOglasSpecification<OglasMajstora>(
                new List<int> { 4 });

            var oglas = new OglasMajstora
            {
                MjestoID = 4
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Oglas_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
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
        public void Oglas_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 1 });

            var oglasi = new List<OglasUsluge>();

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void Oglas_PromjenaMjesta_MijenjaRezultat()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 5 });

            var oglas = KreirajOglas(1, 2);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(oglas));

            oglas.MjestoID = 5;

            Assert.IsTrue(predikat(oglas));
        }

        [TestMethod]
        public void Oglas_DodavanjeMjestaUFilter_MijenjaRezultat()
        {
            var filter = new List<int> { 1 };

            var spec = new MjestoOglasSpecification<OglasUsluge>(
                filter);

            var oglas = KreirajOglas(1, 3);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(oglas));

            filter.Add(3);

            Assert.IsTrue(predikat(oglas));
        }

        [TestMethod]
        public void Oglas_OrderByVrijednostiSuNull()
        {
            var spec = new MjestoOglasSpecification<OglasUsluge>(
                new List<int> { 1 });

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        // ==========================================
        // 2. MJESTO IZVRSIOCA
        // ==========================================

        [TestMethod]
        public void Izvrsilac_JednoOdgovarajuceMjesto_VracaTrue()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 2 });

            var majstor = KreirajMajstora(2);

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_MjestoNeOdgovara_VracaFalse()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 5 });

            var majstor = KreirajMajstora(2);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_ViseMjesta_JednoOdgovara_VracaTrue()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 4, 5 });

            var majstor = KreirajMajstora(1, 2, 5);

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_ViseMjesta_NijednoNeOdgovara_VracaFalse()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 7, 8 });

            var majstor = KreirajMajstora(1, 2, 3);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_PraznaListaMjesta_VracaFalse()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 1 });

            var majstor = KreirajMajstora();

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_PrazanFilter_VracaFalse()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int>());

            var majstor = KreirajMajstora(1, 2);

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_NullMjesta_BacaArgumentNullException()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 1 });

            var majstor = new Majstor
            {
                Mjesta = null!
            };

            Assert.ThrowsException<ArgumentNullException>(
                () => spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_NullFilter_BacaNullReferenceException()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                null!);

            var majstor = KreirajMajstora(1);

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void Izvrsilac_Firma_FiltriraMjesta()
        {
            var spec = new MjestoIzvrsilacSpecification<Firma>(
                new List<int> { 3 });

            var firma = new Firma
            {
                Mjesta = new List<KorisnikMjesto>
                {
                    new KorisnikMjesto { MjestoID = 3 }
                }
            };

            Assert.IsTrue(spec.ToExpression().Compile()(firma));
        }

        [TestMethod]
        public void Izvrsilac_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 2, 4 });

            var majstori = new List<Majstor>
            {
                KreirajMajstora(1),
                KreirajMajstora(2),
                KreirajMajstora(3),
                KreirajMajstora(4)
            };

            var rezultat = majstori.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(2, rezultat.Count);
            Assert.AreSame(majstori[1], rezultat[0]);
            Assert.AreSame(majstori[3], rezultat[1]);
        }

        [TestMethod]
        public void Izvrsilac_PraznaKolekcija_VracaPrazanRezultat()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 1 });

            var majstori = new List<Majstor>();

            var rezultat = majstori.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(0, rezultat.Count);
        }

        [TestMethod]
        public void Izvrsilac_DodavanjeMjesta_MijenjaRezultat()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 5 });

            var majstor = KreirajMajstora(1);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(majstor));

            majstor.Mjesta.Add(
                new KorisnikMjesto { MjestoID = 5 });

            Assert.IsTrue(predikat(majstor));
        }

        [TestMethod]
        public void Izvrsilac_DodavanjeMjestaUFilter_MijenjaRezultat()
        {
            var filter = new List<int> { 1 };

            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                filter);

            var majstor = KreirajMajstora(3);
            var predikat = spec.ToExpression().Compile();

            Assert.IsFalse(predikat(majstor));

            filter.Add(3);

            Assert.IsTrue(predikat(majstor));
        }

        [TestMethod]
        public void Izvrsilac_OrderByVrijednostiSuNull()
        {
            var spec = new MjestoIzvrsilacSpecification<Majstor>(
                new List<int> { 1 });

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
