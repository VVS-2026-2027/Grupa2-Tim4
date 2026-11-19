
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;
using PopravkaBa.Domain.Specifications.Subtype;

namespace PopravkaBa.Tests.Unit.Specifications
{
    [TestClass]
    public class KljucneRijeciSpecificationTests
    {
        // ==========================================
        // 1. PRETRAGA NASLOVA OGLASA
        // ==========================================

        [DataTestMethod]
        [DataRow("Popravka krova", "popravka", true)]
        [DataRow("Popravka krova", "KROVA", true)]
        [DataRow("POPRAVKA KROVA", "popravka", true)]
        [DataRow("Hitna popravka krova", "popravka", true)]
        [DataRow("Elektricarske usluge", "vodoinstalater", false)]
        [DataRow("Popravka krova", "krovovi", false)]
        [DataRow("Popravka krova", "", true)]
        [DataRow("", "popravka", false)]
        [DataRow("", "", true)]
        public void Naslov_RazliciteKljucneRijeci_VracaOcekivano(
            string naslov,
            string rijec,
            bool ocekivano)
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasUsluge>(
                    rijec);

            var oglas = new OglasUsluge
            {
                Naslov = naslov
            };

            var rezultat = spec.ToExpression().Compile()(oglas);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void Naslov_NullNaslov_BacaNullReferenceException()
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasUsluge>(
                    "test");

            var oglas = new OglasUsluge
            {
                Naslov = null!
            };

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Naslov_NullKljucnaRijec_BacaNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => new KljucneRijeciNaslovOglasaSpecification<OglasUsluge>(
                    null!));
        }

        [TestMethod]
        public void Naslov_RadnoMjesto_PretrazujeNaslov()
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasRadnoMjesto>(
                    "programer");

            var oglas = new OglasRadnoMjesto
            {
                Naslov = "Potreban PROGRAMER"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Naslov_Majstora_PretrazujeNaslov()
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasMajstora>(
                    "majstor");

            var oglas = new OglasMajstora
            {
                Naslov = "ISKUSAN MAJSTOR"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void Naslov_FiltriranjeKolekcije_VracaOdgovarajuceOglase()
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasUsluge>(
                    "popravka");

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    OglasID = 1,
                    Naslov = "Popravka krova"
                },
                new OglasUsluge
                {
                    OglasID = 2,
                    Naslov = "Krecenje zidova"
                },
                new OglasUsluge
                {
                    OglasID = 3,
                    Naslov = "POPRAVKA instalacija"
                }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1, 3 },
                rezultat);
        }

        [TestMethod]
        public void Naslov_OrderByVrijednostiSuNull()
        {
            var spec =
                new KljucneRijeciNaslovOglasaSpecification<OglasUsluge>(
                    "test");

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        // ==========================================
        // 2. PRETRAGA OPISA OGLASA
        // ==========================================

        [DataTestMethod]
        [DataRow("Potrebna popravka krova", "popravka", true)]
        [DataRow("Potrebna popravka krova", "KROVA", true)]
        [DataRow("HITNA POPRAVKA", "popravka", true)]
        [DataRow("Elektricarske instalacije", "voda", false)]
        [DataRow("Popravka krova", "", true)]
        [DataRow("", "popravka", false)]
        [DataRow("", "", true)]
        public void OpisOglasa_RazliciteKljucneRijeci_VracaOcekivano(
            string opis,
            string rijec,
            bool ocekivano)
        {
            var spec =
                new KljucneRijeciOpisOglasaSpecification<OglasUsluge>(
                    rijec);

            var oglas = new OglasUsluge
            {
                Opis = opis
            };

            var rezultat = spec.ToExpression().Compile()(oglas);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void OpisOglasa_NullOpis_BacaNullReferenceException()
        {
            var spec =
                new KljucneRijeciOpisOglasaSpecification<OglasUsluge>(
                    "test");

            var oglas = new OglasUsluge
            {
                Opis = null!
            };

            Assert.ThrowsException<NullReferenceException>(
                () => spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void OpisOglasa_NullKljucnaRijec_BacaNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => new KljucneRijeciOpisOglasaSpecification<OglasUsluge>(
                    null!));
        }

        [TestMethod]
        public void OpisOglasa_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new KljucneRijeciOpisOglasaSpecification<OglasUsluge>(
                    "vodovod");

            var oglasi = new List<OglasUsluge>
            {
                new OglasUsluge
                {
                    OglasID = 1,
                    Opis = "Popravka vodovoda"
                },
                new OglasUsluge
                {
                    OglasID = 2,
                    Opis = "Popravka krova"
                },
                new OglasUsluge
                {
                    OglasID = 3,
                    Opis = "VODOVOD instalacije"
                }
            };

            var rezultat = oglasi.AsQueryable()
                .Where(spec.ToExpression())
                .Select(o => o.OglasID)
                .ToList();

            CollectionAssert.AreEqual(
                new List<int> { 1, 3 },
                rezultat);
        }

        [TestMethod]
        public void OpisOglasa_RadnoMjesto_PretrazujeOpis()
        {
            var spec =
                new KljucneRijeciOpisOglasaSpecification<OglasRadnoMjesto>(
                    "iskustvo");

            var oglas = new OglasRadnoMjesto
            {
                Opis = "Potrebno radno ISKUSTVO"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(oglas));
        }

        [TestMethod]
        public void OpisOglasa_OrderByVrijednostiSuNull()
        {
            var spec =
                new KljucneRijeciOpisOglasaSpecification<OglasUsluge>(
                    "test");

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        // ==========================================
        // 3. PRETRAGA OPISA IZVRSIOCA
        // ==========================================

        [DataTestMethod]
        [DataRow("Iskusni elektricar", "elektricar", true)]
        [DataRow("Iskusni ELEKTRICAR", "elektricar", true)]
        [DataRow("Popravka instalacija", "POPRAVKA", true)]
        [DataRow("Vodoinstalaterske usluge", "elektricar", false)]
        [DataRow("Iskusni majstor", "", true)]
        [DataRow("", "majstor", false)]
        [DataRow("", "", true)]
        public void OpisIzvrsioca_RazliciteRijeci_VracaOcekivano(
            string opis,
            string rijec,
            bool ocekivano)
        {
            var spec =
                new KljucneRijeciOpisIzvrsiocaSpecification<Majstor>(
                    rijec);

            var majstor = new Majstor
            {
                Opis = opis
            };

            var rezultat = spec.ToExpression().Compile()(majstor);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void OpisIzvrsioca_NullOpis_VracaFalse()
        {
            var spec =
                new KljucneRijeciOpisIzvrsiocaSpecification<Majstor>(
                    "majstor");

            var majstor = new Majstor
            {
                Opis = null
            };

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void OpisIzvrsioca_NullKljucnaRijec_BacaNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => new KljucneRijeciOpisIzvrsiocaSpecification<Majstor>(
                    null!));
        }

        [TestMethod]
        public void OpisIzvrsioca_Firma_PretrazujeOpis()
        {
            var spec =
                new KljucneRijeciOpisIzvrsiocaSpecification<Firma>(
                    "gradnja");

            var firma = new Firma
            {
                Opis = "Izvodimo radove i GRADNJA objekata"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(firma));
        }

        [TestMethod]
        public void OpisIzvrsioca_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new KljucneRijeciOpisIzvrsiocaSpecification<Majstor>(
                    "elektricar");

            var majstori = new List<Majstor>
            {
                new Majstor
                {
                    Opis = "Iskusni elektricar"
                },
                new Majstor
                {
                    Opis = "Vodoinstalater"
                },
                new Majstor
                {
                    Opis = "ELEKTRICAR sa iskustvom"
                },
                new Majstor
                {
                    Opis = null
                }
            };

            var rezultat = majstori.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(2, rezultat.Count);
        }

        [TestMethod]
        public void OpisIzvrsioca_OrderByVrijednostiSuNull()
        {
            var spec =
                new KljucneRijeciOpisIzvrsiocaSpecification<Majstor>(
                    "test");

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }

        // ==========================================
        // 4. PRETRAGA IMENA I NAZIVA IZVRSIOCA
        // ==========================================

        [DataTestMethod]
        [DataRow("Marko", "Markovic", "marko", true)]
        [DataRow("Marko", "Markovic", "MARKO", true)]
        [DataRow("Marko", "Markovic", "markovic", true)]
        [DataRow("Marko", "Markovic", "marko markovic", true)]
        [DataRow("Marko", "Markovic", "marko mar", true)]
        [DataRow("Marko", "Markovic", "ivan", false)]
        [DataRow("Marko", "Markovic", "petrovic", false)]
        [DataRow("Marko", "Markovic", "", true)]
        public void DisplayName_Majstor_RazliciteRijeci_VracaOcekivano(
            string ime,
            string prezime,
            string rijec,
            bool ocekivano)
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    rijec);

            var majstor = new Majstor
            {
                Ime = ime,
                Prezime = prezime
            };

            var rezultat = spec.ToExpression().Compile()(majstor);

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void DisplayName_Majstor_SamoIme_VracaTrue()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "amar");

            var majstor = new Majstor
            {
                Ime = "Amar",
                Prezime = "Hodzic"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_Majstor_SamoPrezime_VracaTrue()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "hodzic");

            var majstor = new Majstor
            {
                Ime = "Amar",
                Prezime = "Hodzic"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_Majstor_PunoIme_VracaTrue()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "amar hodzic");

            var majstor = new Majstor
            {
                Ime = "Amar",
                Prezime = "Hodzic"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_Majstor_PogresnoIme_VracaFalse()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "ivan");

            var majstor = new Majstor
            {
                Ime = "Amar",
                Prezime = "Hodzic"
            };

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_NullIme_PretrazujePrezime()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "hodzic");

            var majstor = new Majstor
            {
                Ime = null!,
                Prezime = "Hodzic"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_NullPrezime_PretrazujeIme()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "amar");

            var majstor = new Majstor
            {
                Ime = "Amar",
                Prezime = null!
            };

            Assert.IsTrue(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_NullImeIPrezime_NepostojecaRijecVracaFalse()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "amar");

            var majstor = new Majstor
            {
                Ime = null!,
                Prezime = null!
            };

            Assert.IsFalse(spec.ToExpression().Compile()(majstor));
        }

        [TestMethod]
        public void DisplayName_Firma_NazivFirmeVracaTrue()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Firma>(
                    "gradnja");

            var firma = new Firma
            {
                Ime = "",
                Prezime = "",
                NazivFirme = "GRADNJA DOO"
            };

            Assert.IsTrue(spec.ToExpression().Compile()(firma));
        }

        [TestMethod]
        public void DisplayName_Firma_NeodgovarajuciNazivVracaFalse()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Firma>(
                    "elektro");

            var firma = new Firma
            {
                Ime = "",
                Prezime = "",
                NazivFirme = "Gradnja DOO"
            };

            Assert.IsFalse(spec.ToExpression().Compile()(firma));
        }

        [TestMethod]
        public void DisplayName_NullKljucnaRijec_BacaNullReferenceException()
        {
            Assert.ThrowsException<NullReferenceException>(
                () => new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    null!));
        }

        [TestMethod]
        public void DisplayName_FiltriranjeKolekcije_VracaOdgovarajuce()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "amar");

            var majstori = new List<Majstor>
            {
                new Majstor { Ime = "Amar", Prezime = "Hodzic" },
                new Majstor { Ime = "Marko", Prezime = "Markovic" },
                new Majstor { Ime = "AMAR", Prezime = "Kovac" }
            };

            var rezultat = majstori.AsQueryable()
                .Where(spec.ToExpression())
                .ToList();

            Assert.AreEqual(2, rezultat.Count);
        }

        [TestMethod]
        public void DisplayName_OrderByVrijednostiSuNull()
        {
            var spec =
                new KljucneRijeciDisplayNameIzvrsiocaSpecification<Majstor>(
                    "test");

            Assert.IsNull(spec.OrderBy);
            Assert.IsNull(spec.OrderByDescending);
        }
    }
}
