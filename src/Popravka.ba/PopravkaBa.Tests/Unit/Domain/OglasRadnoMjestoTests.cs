
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasRadnoMjestoTests
    {
        [TestMethod]
        public void NoviOglas_BrojIzvrsilacaJeJedan()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.AreEqual(1, oglas.BrojIzvrsilaca);
        }

        [TestMethod]
        public void NoviOglas_PrihodiIIskustvoSuNula()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.AreEqual(0, oglas.MinPrihod);
            Assert.AreEqual(0, oglas.MaxPrihod);
            Assert.AreEqual(0, oglas.MinIskustvo);
        }

        [TestMethod]
        public void NoviOglas_KolekcijeSuNull()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.IsNull(oglas.Prijave);
            Assert.IsNull(oglas.Uvjeti);
            Assert.IsNull(oglas.VozackeDozvole);
        }

        [TestMethod]
        public void NoviOglas_EnumVrijednostiSuPodrazumijevane()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.AreEqual(
                default(VrstaZaposlenja),
                oglas.VrstaZaposlenja);

            Assert.AreEqual(
                default(TipIsplate),
                oglas.TipIsplate);
        }

        [TestMethod]
        public void BrojIzvrsilaca_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();

            // Act
            oglas.BrojIzvrsilaca = 5;

            // Assert
            Assert.AreEqual(5, oglas.BrojIzvrsilaca);
        }

        [TestMethod]
        public void BrojIzvrsilaca_PromjenaVrijednosti_CuvaNoviBroj()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                BrojIzvrsilaca = 3
            };

            // Act
            oglas.BrojIzvrsilaca = 10;

            // Assert
            Assert.AreEqual(10, oglas.BrojIzvrsilaca);
        }

        [TestMethod]
        public void VozackeDozvole_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();
            var dozvole = new List<OglasVozackaDozvola>();

            // Act
            oglas.VozackeDozvole = dozvole;

            // Assert
            Assert.AreSame(dozvole, oglas.VozackeDozvole);
        }

        [TestMethod]
        public void Uvjeti_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();
            var uvjeti = new List<UvjetOglasa>();

            // Act
            oglas.Uvjeti = uvjeti;

            // Assert
            Assert.AreSame(uvjeti, oglas.Uvjeti);
        }

        [TestMethod]
        public void Prijave_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();
            var prijave = new List<PrijavaRadnoMjesto>();

            // Act
            oglas.Prijave = prijave;

            // Assert
            Assert.AreSame(prijave, oglas.Prijave);
        }

        [TestMethod]
        public void BrojPrijava_NullKolekcija_VracaNula()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(0, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_PraznaKolekcija_VracaNula()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                Prijave = new List<PrijavaRadnoMjesto>()
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(0, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_JednaPrijava_VracaJedan()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                Prijave = new List<PrijavaRadnoMjesto>
                {
                    new PrijavaRadnoMjesto()
                }
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(1, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_VisePrijava_VracaTacanBroj()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                Prijave = new List<PrijavaRadnoMjesto>
                {
                    new PrijavaRadnoMjesto(),
                    new PrijavaRadnoMjesto(),
                    new PrijavaRadnoMjesto()
                }
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(3, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_DodavanjePrijave_AzuriraBroj()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto
            {
                Prijave = new List<PrijavaRadnoMjesto>()
            };

            // Act
            oglas.Prijave.Add(new PrijavaRadnoMjesto());

            // Assert
            Assert.AreEqual(1, oglas.BrojPrijava);
        }

        [TestMethod]
        public void BrojPrijava_UklanjanjePrijave_AzuriraBroj()
        {
            // Arrange
            var prijava = new PrijavaRadnoMjesto();

            var oglas = new OglasRadnoMjesto
            {
                Prijave = new List<PrijavaRadnoMjesto>
                {
                    prijava,
                    new PrijavaRadnoMjesto()
                }
            };

            // Act
            oglas.Prijave.Remove(prijava);

            // Assert
            Assert.AreEqual(1, oglas.BrojPrijava);
        }

        [TestMethod]
        public void VrstaZaposlenja_PostavljanjeVrijednosti_CuvaEnum()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();
            var vrsta = (VrstaZaposlenja)1;

            // Act
            oglas.VrstaZaposlenja = vrsta;

            // Assert
            Assert.AreEqual(vrsta, oglas.VrstaZaposlenja);
        }

        [TestMethod]
        public void MinPrihod_PostavljanjeVrijednosti_CuvaIznos()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();

            // Act
            oglas.MinPrihod = 1500;

            // Assert
            Assert.AreEqual(1500, oglas.MinPrihod);
        }

        [TestMethod]
        public void MaxPrihod_PostavljanjeVrijednosti_CuvaIznos()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();

            // Act
            oglas.MaxPrihod = 3000;

            // Assert
            Assert.AreEqual(3000, oglas.MaxPrihod);
        }

        [TestMethod]
        public void TipIsplate_PostavljanjeVrijednosti_CuvaEnum()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();
            var tip = (TipIsplate)1;

            // Act
            oglas.TipIsplate = tip;

            // Assert
            Assert.AreEqual(tip, oglas.TipIsplate);
        }

        [TestMethod]
        public void MinIskustvo_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var oglas = new OglasRadnoMjesto();

            // Act
            oglas.MinIskustvo = 3;

            // Assert
            Assert.AreEqual(3, oglas.MinIskustvo);
        }

        [TestMethod]
        public void OglasRadnoMjesto_NasljedjujeOglas()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.IsInstanceOfType(oglas, typeof(Oglas));
        }

        [TestMethod]
        public void NoviOglas_NaslijedjeniStatusJeAktivan()
        {
            // Arrange i Act
            var oglas = new OglasRadnoMjesto();

            // Assert
            Assert.AreEqual(Status.Aktivan, oglas.StatusOglasa);
        }

        [TestMethod]
        public void SvaSvojstva_PostavljanjeVrijednosti_CuvaPodatke()
        {
            // Arrange
            var prijave = new List<PrijavaRadnoMjesto>
            {
                new PrijavaRadnoMjesto(),
                new PrijavaRadnoMjesto()
            };

            var dozvole = new List<OglasVozackaDozvola>();
            var uvjeti = new List<UvjetOglasa>();

            // Act
            var oglas = new OglasRadnoMjesto
            {
                OglasID = 15,
                Naslov = "Potreban električar",
                Opis = "Tražimo iskusnog električara.",
                BrojIzvrsilaca = 2,
                VozackeDozvole = dozvole,
                Uvjeti = uvjeti,
                Prijave = prijave,
                VrstaZaposlenja = (VrstaZaposlenja)1,
                MinPrihod = 1500,
                MaxPrihod = 2500,
                TipIsplate = (TipIsplate)1,
                MinIskustvo = 5
            };

            // Assert
            Assert.AreEqual(15, oglas.OglasID);
            Assert.AreEqual("Potreban električar", oglas.Naslov);
            Assert.AreEqual(
                "Tražimo iskusnog električara.",
                oglas.Opis);
            Assert.AreEqual(2, oglas.BrojIzvrsilaca);
            Assert.AreSame(dozvole, oglas.VozackeDozvole);
            Assert.AreSame(uvjeti, oglas.Uvjeti);
            Assert.AreSame(prijave, oglas.Prijave);
            Assert.AreEqual(
                (VrstaZaposlenja)1,
                oglas.VrstaZaposlenja);
            Assert.AreEqual(1500, oglas.MinPrihod);
            Assert.AreEqual(2500, oglas.MaxPrihod);
            Assert.AreEqual((TipIsplate)1, oglas.TipIsplate);
            Assert.AreEqual(5, oglas.MinIskustvo);
            Assert.AreEqual(2, oglas.BrojPrijava);
        }
    }
}
