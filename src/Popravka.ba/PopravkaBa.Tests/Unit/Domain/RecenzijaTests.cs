
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
    public class RecenzijaTests
    {
        private static bool ValidirajOcjenu(int ocjena)
        {
            var recenzija = new Recenzija
            {
                Ocjena = ocjena
            };

            var svojstvo = typeof(Recenzija)
                .GetProperty(nameof(Recenzija.Ocjena))!;

            var atribut = svojstvo
                .GetCustomAttributes(
                    typeof(RangeAttribute), true)
                .Cast<RangeAttribute>()
                .Single();

            return atribut.IsValid(recenzija.Ocjena);
        }

        [TestMethod]
        public void NovaRecenzija_PocetneVrijednostiSuIspravne()
        {
            // Arrange i Act
            var recenzija = new Recenzija();

            // Assert
            Assert.AreEqual(0, recenzija.RecenzijaID);
            Assert.AreEqual(0, recenzija.Ocjena);
            Assert.IsFalse(recenzija.Prijavljena);
            Assert.IsNull(recenzija.RazlogPrijave);
            Assert.IsNull(recenzija.DatumPrijave);
            Assert.IsNull(recenzija.StatusPrijave);
        }

        [TestMethod]
        public void NovaRecenzija_PovezaniObjektiSuNull()
        {
            // Arrange i Act
            var recenzija = new Recenzija();

            // Assert
            Assert.IsNull(recenzija.KlijentID);
            Assert.IsNull(recenzija.Klijent);
            Assert.IsNull(recenzija.IzvrsilacID);
            Assert.IsNull(recenzija.Izvrsilac);
            Assert.IsNull(recenzija.Komentar);
        }

        [TestMethod]
        public void DatumRecenzije_NovaRecenzija_PostavljaUtcVrijeme()
        {
            // Arrange
            var prije = DateTime.UtcNow;

            // Act
            var recenzija = new Recenzija();
            var poslije = DateTime.UtcNow;

            // Assert
            Assert.AreEqual(
                DateTimeKind.Utc,
                recenzija.DatumRecenzije.Kind);

            Assert.IsTrue(
                recenzija.DatumRecenzije >= prije);

            Assert.IsTrue(
                recenzija.DatumRecenzije <= poslije);
        }

        [TestMethod]
        public void RecenzijaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.RecenzijaID = 15;

            // Assert
            Assert.AreEqual(15, recenzija.RecenzijaID);
        }

        [TestMethod]
        public void KlijentID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.KlijentID = "klijent-123";

            // Assert
            Assert.AreEqual(
                "klijent-123",
                recenzija.KlijentID);
        }

        [TestMethod]
        public void Klijent_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var recenzija = new Recenzija();

            var klijent = new Klijent
            {
                Id = "klijent-123"
            };

            // Act
            recenzija.Klijent = klijent;

            // Assert
            Assert.AreSame(klijent, recenzija.Klijent);
        }

        [TestMethod]
        public void IzvrsilacID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.IzvrsilacID = "majstor-123";

            // Assert
            Assert.AreEqual(
                "majstor-123",
                recenzija.IzvrsilacID);
        }

        [TestMethod]
        public void Izvrsilac_Majstor_CuvaReferencu()
        {
            // Arrange
            var recenzija = new Recenzija();

            IzvrsilacUsluge majstor = new Majstor
            {
                Id = "majstor-123"
            };

            // Act
            recenzija.Izvrsilac = majstor;

            // Assert
            Assert.AreSame(majstor, recenzija.Izvrsilac);
        }

        [TestMethod]
        public void Izvrsilac_Firma_CuvaReferencu()
        {
            // Arrange
            var recenzija = new Recenzija();

            IzvrsilacUsluge firma = new Firma
            {
                Id = "firma-001"
            };

            // Act
            recenzija.Izvrsilac = firma;

            // Assert
            Assert.AreSame(firma, recenzija.Izvrsilac);
        }

        [TestMethod]
        public void Ocjena_PostavljanjeVrijednosti_CuvaOcjenu()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.Ocjena = 5;

            // Assert
            Assert.AreEqual(5, recenzija.Ocjena);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void Ocjena_ValidneVrijednosti_ProlazeRangeValidaciju(
            int ocjena)
        {
            // Act
            bool rezultat = ValidirajOcjenu(ocjena);

            // Assert
            Assert.IsTrue(rezultat);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(6)]
        [DataRow(10)]
        public void Ocjena_NevalidneVrijednosti_NeProlazeRangeValidaciju(
            int ocjena)
        {
            // Act
            bool rezultat = ValidirajOcjenu(ocjena);

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void Komentar_PostavljanjeVrijednosti_CuvaKomentar()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.Komentar =
                "Majstor je kvalitetno obavio posao.";

            // Assert
            Assert.AreEqual(
                "Majstor je kvalitetno obavio posao.",
                recenzija.Komentar);
        }

        [TestMethod]
        public void DatumRecenzije_RucnoPostavljanje_CuvaDatum()
        {
            // Arrange
            var recenzija = new Recenzija();

            var datum = new DateTime(
                2026, 11, 19, 12, 0, 0,
                DateTimeKind.Utc);

            // Act
            recenzija.DatumRecenzije = datum;

            // Assert
            Assert.AreEqual(
                datum,
                recenzija.DatumRecenzije);
        }

        [TestMethod]
        public void Prijavljena_PostavljanjeVrijednosti_CuvaVrijednost()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.Prijavljena = true;

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);
        }

        [TestMethod]
        public void RazlogPrijave_PostavljanjeVrijednosti_CuvaTekst()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.RazlogPrijave =
                "Neprimjeren sadržaj";

            // Assert
            Assert.AreEqual(
                "Neprimjeren sadržaj",
                recenzija.RazlogPrijave);
        }

        [TestMethod]
        public void RazlogPrijave_NullVrijednost_CuvaNull()
        {
            // Arrange
            var recenzija = new Recenzija
            {
                RazlogPrijave = "Prethodni razlog"
            };

            // Act
            recenzija.RazlogPrijave = null;

            // Assert
            Assert.IsNull(recenzija.RazlogPrijave);
        }

        [TestMethod]
        public void DatumPrijave_PostavljanjeVrijednosti_CuvaDatum()
        {
            // Arrange
            var recenzija = new Recenzija();

            var datum = new DateTime(
                2026, 11, 19, 13, 0, 0,
                DateTimeKind.Utc);

            // Act
            recenzija.DatumPrijave = datum;

            // Assert
            Assert.AreEqual(
                datum,
                recenzija.DatumPrijave);
        }

        [TestMethod]
        public void StatusPrijave_PostavljanjeVrijednosti_CuvaStatus()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.StatusPrijave = Status.NaCekanju;

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);
        }

        [TestMethod]
        public void StatusPrijave_NullVrijednost_CuvaNull()
        {
            // Arrange
            var recenzija = new Recenzija
            {
                StatusPrijave = Status.NaCekanju
            };

            // Act
            recenzija.StatusPrijave = null;

            // Assert
            Assert.IsNull(recenzija.StatusPrijave);
        }

        [TestMethod]
        public void PrijaviRecenziju_ValidanRazlog_OznacavaRecenzijuKaoPrijavljenu()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.PrijaviRecenziju(
                "Neprimjeren sadržaj");

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);
        }

        [TestMethod]
        public void PrijaviRecenziju_ValidanRazlog_CuvaRazlogPrijave()
        {
            // Arrange
            var recenzija = new Recenzija();
            const string razlog = "Uvredljiv komentar";

            // Act
            recenzija.PrijaviRecenziju(razlog);

            // Assert
            Assert.AreEqual(
                razlog,
                recenzija.RazlogPrijave);
        }

        [TestMethod]
        public void PrijaviRecenziju_ValidanRazlog_PostavljaUtcDatumPrijave()
        {
            // Arrange
            var recenzija = new Recenzija();
            var prije = DateTime.UtcNow;

            // Act
            recenzija.PrijaviRecenziju(
                "Lažne informacije");

            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(recenzija.DatumPrijave);

            Assert.AreEqual(
                DateTimeKind.Utc,
                recenzija.DatumPrijave.Value.Kind);

            Assert.IsTrue(
                recenzija.DatumPrijave.Value >= prije);

            Assert.IsTrue(
                recenzija.DatumPrijave.Value <= poslije);
        }

        [TestMethod]
        public void PrijaviRecenziju_ValidanRazlog_PostavljaStatusNaCekanju()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.PrijaviRecenziju(
                "Neprimjeren sadržaj");

            // Assert
            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);
        }

        [TestMethod]
        public void PrijaviRecenziju_SviPodaci_AzurirajuSeIspravno()
        {
            // Arrange
            var recenzija = new Recenzija
            {
                RecenzijaID = 10,
                Ocjena = 1,
                Komentar = "Loše iskustvo"
            };

            var prije = DateTime.UtcNow;

            // Act
            recenzija.PrijaviRecenziju(
                "Uvredljiv sadržaj");

            var poslije = DateTime.UtcNow;

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);

            Assert.AreEqual(
                "Uvredljiv sadržaj",
                recenzija.RazlogPrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);

            Assert.IsNotNull(recenzija.DatumPrijave);

            Assert.IsTrue(
                recenzija.DatumPrijave.Value >= prije);

            Assert.IsTrue(
                recenzija.DatumPrijave.Value <= poslije);

            Assert.AreEqual(10, recenzija.RecenzijaID);
            Assert.AreEqual(1, recenzija.Ocjena);
            Assert.AreEqual(
                "Loše iskustvo",
                recenzija.Komentar);
        }

        [TestMethod]
        public void PrijaviRecenziju_PonovnaPrijava_AzuriraRazlogIStatus()
        {
            // Arrange
            var recenzija = new Recenzija();

            recenzija.PrijaviRecenziju(
                "Prvi razlog");

            recenzija.StatusPrijave = Status.Aktivan;

            // Act
            recenzija.PrijaviRecenziju(
                "Novi razlog");

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);

            Assert.AreEqual(
                "Novi razlog",
                recenzija.RazlogPrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);

            Assert.IsNotNull(recenzija.DatumPrijave);
        }

        [TestMethod]
        public void PrijaviRecenziju_PrazanRazlog_CuvaPrazanString()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.PrijaviRecenziju(string.Empty);

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);

            Assert.AreEqual(
                string.Empty,
                recenzija.RazlogPrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);
        }

        [TestMethod]
        public void PrijaviRecenziju_NullRazlog_TrenutnaImplementacijaPrihvataNull()
        {
            // Arrange
            var recenzija = new Recenzija();

            // Act
            recenzija.PrijaviRecenziju(null!);

            // Assert
            Assert.IsTrue(recenzija.Prijavljena);
            Assert.IsNull(recenzija.RazlogPrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);
        }

        [TestMethod]
        public void Recenzija_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var klijent = new Klijent
            {
                Id = "klijent-001"
            };

            IzvrsilacUsluge izvrsilac = new Majstor
            {
                Id = "majstor-001"
            };

            var datum = new DateTime(
                2026, 11, 19, 10, 0, 0,
                DateTimeKind.Utc);

            var datumPrijave = new DateTime(
                2026, 11, 19, 11, 0, 0,
                DateTimeKind.Utc);

            // Act
            var recenzija = new Recenzija
            {
                RecenzijaID = 1,
                KlijentID = "klijent-001",
                Klijent = klijent,
                IzvrsilacID = "majstor-001",
                Izvrsilac = izvrsilac,
                Ocjena = 5,
                Komentar = "Odlično obavljen posao",
                DatumRecenzije = datum,
                Prijavljena = true,
                RazlogPrijave = "Testni razlog",
                DatumPrijave = datumPrijave,
                StatusPrijave = Status.NaCekanju
            };

            // Assert
            Assert.AreEqual(1, recenzija.RecenzijaID);
            Assert.AreEqual(
                "klijent-001",
                recenzija.KlijentID);

            Assert.AreSame(klijent, recenzija.Klijent);

            Assert.AreEqual(
                "majstor-001",
                recenzija.IzvrsilacID);

            Assert.AreSame(
                izvrsilac,
                recenzija.Izvrsilac);

            Assert.AreEqual(5, recenzija.Ocjena);

            Assert.AreEqual(
                "Odlično obavljen posao",
                recenzija.Komentar);

            Assert.AreEqual(datum, recenzija.DatumRecenzije);
            Assert.IsTrue(recenzija.Prijavljena);

            Assert.AreEqual(
                "Testni razlog",
                recenzija.RazlogPrijave);

            Assert.AreEqual(
                datumPrijave,
                recenzija.DatumPrijave);

            Assert.AreEqual(
                Status.NaCekanju,
                recenzija.StatusPrijave);
        }
    }
}
