
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class VerifikacijskiTokenTests
    {
        private static List<ValidationResult> ValidirajTokenHash(
            string? hash)
        {
            // Testiramo samo atribute svojstva TokenHash.
            var svojstvo = typeof(VerifikacijskiToken)
                .GetProperty(nameof(VerifikacijskiToken.TokenHash))!;

            var atributi = svojstvo
                .GetCustomAttributes(
                    typeof(ValidationAttribute), true)
                .Cast<ValidationAttribute>();

            var rezultati = new List<ValidationResult>();

            foreach (var atribut in atributi)
            {
                if (!atribut.IsValid(hash))
                {
                    rezultati.Add(
                        new ValidationResult(
                            atribut.FormatErrorMessage("TokenHash")));
                }
            }

            return rezultati;
        }

        [TestMethod]
        public void NoviToken_PodrazumijevaneVrijednostiSuIspravne()
        {
            // Arrange i Act
            var token = new VerifikacijskiToken();

            // Assert
            Assert.AreEqual(0, token.ID);
            Assert.IsNull(token.KorisnikID);
            Assert.IsNull(token.Korisnik);
            Assert.IsNull(token.TokenHash);
            Assert.AreEqual(default(TipTokena), token.Tip);
            Assert.AreEqual(
                default(DateTime),
                token.VrijemeGenerisanja);
            Assert.AreEqual(
                default(DateTime),
                token.VrijemeIsteka);
            Assert.IsNull(token.VrijemeKoristenja);
        }

        [TestMethod]
        public void ID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            // Act
            token.ID = 15;

            // Assert
            Assert.AreEqual(15, token.ID);
        }

        [TestMethod]
        public void KorisnikID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            // Act
            token.KorisnikID = "korisnik-123";

            // Assert
            Assert.AreEqual(
                "korisnik-123",
                token.KorisnikID);
        }

        [TestMethod]
        public void Korisnik_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            ApplicationUser korisnik = new Klijent
            {
                Id = "korisnik-123"
            };

            // Act
            token.Korisnik = korisnik;

            // Assert
            Assert.AreSame(
                korisnik,
                token.Korisnik);
        }

        [TestMethod]
        public void Korisnik_ZamjenaReference_CuvaNovogKorisnika()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            var prviKorisnik = new Klijent
            {
                Id = "korisnik-1"
            };

            var drugiKorisnik = new Klijent
            {
                Id = "korisnik-2"
            };

            token.Korisnik = prviKorisnik;

            // Act
            token.Korisnik = drugiKorisnik;

            // Assert
            Assert.AreSame(
                drugiKorisnik,
                token.Korisnik);
        }

        [TestMethod]
        public void TokenHash_PostavljanjeVrijednosti_CuvaHash()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            var hash = new string('a', 64);

            // Act
            token.TokenHash = hash;

            // Assert
            Assert.AreEqual(hash, token.TokenHash);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(32)]
        [DataRow(63)]
        [DataRow(64)]
        public void TokenHash_DozvoljeneDuzine_ProlazeValidaciju(
            int duzina)
        {
            // Arrange
            var hash = new string('a', duzina);

            // Act
            var greske = ValidirajTokenHash(hash);

            // Assert
            Assert.AreEqual(0, greske.Count);
        }

        [DataTestMethod]
        [DataRow(65)]
        [DataRow(100)]
        [DataRow(128)]
        public void TokenHash_PredugacakHash_NeProlaziValidaciju(
            int duzina)
        {
            // Arrange
            var hash = new string('a', duzina);

            // Act
            var greske = ValidirajTokenHash(hash);

            // Assert
            Assert.IsTrue(greske.Count > 0);
        }

        [TestMethod]
        public void TokenHash_NullVrijednost_NeProlaziValidaciju()
        {
            // Arrange
            string? hash = null;

            // Act
            var greske = ValidirajTokenHash(hash);

            // Assert
            Assert.IsTrue(greske.Count > 0);
        }

        [TestMethod]
        public void TokenHash_PrazanString_NeProlaziValidaciju()
        {
            // Arrange
            var hash = string.Empty;

            // Act
            var greske = ValidirajTokenHash(hash);

            // Assert
            Assert.IsTrue(greske.Count > 0);
        }

        [TestMethod]
        public void TokenHash_SamoRazmaci_NeProlaziValidaciju()
        {
            // Arrange
            var hash = "   ";

            // Act
            var greske = ValidirajTokenHash(hash);

            // Assert
            Assert.IsTrue(greske.Count > 0);
        }

        [TestMethod]
        public void Tip_PostavljanjeVrijednosti_CuvaTipTokena()
        {
            // Arrange
            var token = new VerifikacijskiToken();
            var tip = (TipTokena)1;

            // Act
            token.Tip = tip;

            // Assert
            Assert.AreEqual(tip, token.Tip);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void Tip_RazliciteVrijednosti_CuvaEnum(
            int vrijednost)
        {
            // Arrange
            var token = new VerifikacijskiToken();
            var tip = (TipTokena)vrijednost;

            // Act
            token.Tip = tip;

            // Assert
            Assert.AreEqual(tip, token.Tip);
        }

        [TestMethod]
        public void VrijemeGenerisanja_PostavljanjeDatuma_CuvaVrijednost()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            var datum = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            // Act
            token.VrijemeGenerisanja = datum;

            // Assert
            Assert.AreEqual(
                datum,
                token.VrijemeGenerisanja);
        }

        [TestMethod]
        public void VrijemeIsteka_PostavljanjeDatuma_CuvaVrijednost()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            var datum = new DateTime(
                2026, 11, 20, 10, 0, 0,
                DateTimeKind.Utc);

            // Act
            token.VrijemeIsteka = datum;

            // Assert
            Assert.AreEqual(
                datum,
                token.VrijemeIsteka);
        }

        [TestMethod]
        public void VrijemeKoristenja_NovaInstanca_JeNull()
        {
            // Arrange i Act
            var token = new VerifikacijskiToken();

            // Assert
            Assert.IsNull(token.VrijemeKoristenja);
        }

        [TestMethod]
        public void VrijemeKoristenja_PostavljanjeDatuma_CuvaVrijednost()
        {
            // Arrange
            var token = new VerifikacijskiToken();

            var datum = new DateTime(
                2026, 11, 19, 12, 0, 0,
                DateTimeKind.Utc);

            // Act
            token.VrijemeKoristenja = datum;

            // Assert
            Assert.IsNotNull(token.VrijemeKoristenja);
            Assert.AreEqual(
                datum,
                token.VrijemeKoristenja.Value);
        }

        [TestMethod]
        public void VrijemeKoristenja_ResetovanjeNaNull_CuvaNull()
        {
            // Arrange
            var token = new VerifikacijskiToken
            {
                VrijemeKoristenja = DateTime.UtcNow
            };

            // Act
            token.VrijemeKoristenja = null;

            // Assert
            Assert.IsNull(token.VrijemeKoristenja);
        }

        [TestMethod]
        public void VerifikacijskiToken_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var korisnik = new Klijent
            {
                Id = "korisnik-001"
            };

            var hash = new string('b', 64);

            var generisanje = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            var istek = generisanje.AddHours(24);
            var koristenje = generisanje.AddHours(2);

            var tip = (TipTokena)1;

            // Act
            var token = new VerifikacijskiToken
            {
                ID = 1,
                KorisnikID = "korisnik-001",
                Korisnik = korisnik,
                TokenHash = hash,
                Tip = tip,
                VrijemeGenerisanja = generisanje,
                VrijemeIsteka = istek,
                VrijemeKoristenja = koristenje
            };

            // Assert
            Assert.AreEqual(1, token.ID);
            Assert.AreEqual(
                "korisnik-001",
                token.KorisnikID);
            Assert.AreSame(korisnik, token.Korisnik);
            Assert.AreEqual(hash, token.TokenHash);
            Assert.AreEqual(tip, token.Tip);
            Assert.AreEqual(
                generisanje,
                token.VrijemeGenerisanja);
            Assert.AreEqual(
                istek,
                token.VrijemeIsteka);
            Assert.AreEqual(
                koristenje,
                token.VrijemeKoristenja);
        }
    }
}
