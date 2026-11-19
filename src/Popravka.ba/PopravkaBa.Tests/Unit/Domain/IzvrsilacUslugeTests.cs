
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class IzvrsilacUslugeTests
    {
        private static IzvrsilacUsluge KreirajIzvrsioca()
        {
            return new Firma
            {
                NazivFirme = "Test Firma"
            };
        }

        [TestMethod]
        public void NoviIzvrsilac_PodrazumijevaneVrijednostiSuIspravne()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Assert
            Assert.AreEqual(0.0M, izvrsilac.ProsjecnaOcjena);
            Assert.AreEqual(0, izvrsilac.BrojZavrsenihPoslova);
            Assert.AreEqual(0, izvrsilac.BrojRecenzija);
        }

        [TestMethod]
        public void NoviIzvrsilac_OpcionalnaSvojstvaSuNull()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Assert
            Assert.IsNull(izvrsilac.StambeniBroj);
            Assert.IsNull(izvrsilac.Opis);
            Assert.IsNull(izvrsilac.MinCijenaUsluge);
            Assert.IsNull(izvrsilac.Kategorije);
            Assert.IsNull(izvrsilac.SlikePortfolija);
            Assert.IsNull(izvrsilac.Ponude);
            Assert.IsNull(izvrsilac.Recenzije);
        }

        [TestMethod]
        public void Adresa_PostavljanjeVrijednosti_CuvaAdresu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.Adresa = "Zmaja od Bosne 10";

            // Assert
            Assert.AreEqual(
                "Zmaja od Bosne 10",
                izvrsilac.Adresa);
        }

        [TestMethod]
        public void StambeniBroj_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.StambeniBroj = "12A";

            // Assert
            Assert.AreEqual(
                "12A",
                izvrsilac.StambeniBroj);
        }

        [TestMethod]
        public void ProsjecnaOcjena_PostavljanjeVrijednosti_CuvaOcjenu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.ProsjecnaOcjena = 4.75M;

            // Assert
            Assert.AreEqual(
                4.75M,
                izvrsilac.ProsjecnaOcjena);
        }

        [TestMethod]
        public void BrojZavrsenihPoslova_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.BrojZavrsenihPoslova = 15;

            // Assert
            Assert.AreEqual(
                15,
                izvrsilac.BrojZavrsenihPoslova);
        }

        [TestMethod]
        public void Opis_PostavljanjeVrijednosti_CuvaOpis()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.Opis =
                "Profesionalne vodoinstalaterske usluge";

            // Assert
            Assert.AreEqual(
                "Profesionalne vodoinstalaterske usluge",
                izvrsilac.Opis);
        }

        [TestMethod]
        public void MinCijenaUsluge_PostavljanjeVrijednosti_CuvaCijenu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.MinCijenaUsluge = 50;

            // Assert
            Assert.AreEqual(
                50,
                izvrsilac.MinCijenaUsluge);
        }

        [TestMethod]
        public void BrojRecenzija_PostavljanjeVrijednosti_CuvaBroj()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.BrojRecenzija = 8;

            // Assert
            Assert.AreEqual(
                8,
                izvrsilac.BrojRecenzija);
        }

        [TestMethod]
        public void Kategorije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            var kategorije = new List<IzvrsilacKategorija>();

            // Act
            izvrsilac.Kategorije = kategorije;

            // Assert
            Assert.AreSame(
                kategorije,
                izvrsilac.Kategorije);
        }

        [TestMethod]
        public void SlikePortfolija_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            var slike = new List<PortfolioSlika>();

            // Act
            izvrsilac.SlikePortfolija = slike;

            // Assert
            Assert.AreSame(
                slike,
                izvrsilac.SlikePortfolija);
        }

        [TestMethod]
        public void Ponude_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            var ponude = new List<PonudaUsluge>();

            // Act
            izvrsilac.Ponude = ponude;

            // Assert
            Assert.AreSame(
                ponude,
                izvrsilac.Ponude);
        }

        [TestMethod]
        public void Recenzije_PostavljanjeKolekcije_CuvaReferencu()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            var recenzije = new List<Recenzija>();

            // Act
            izvrsilac.Recenzije = recenzije;

            // Assert
            Assert.AreSame(
                recenzije,
                izvrsilac.Recenzije);
        }

        [TestMethod]
        public void SvojstvaIzvrsioca_PostavljanjeSvihVrijednosti_CuvaPodatke()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            // Act
            izvrsilac.Adresa = "Mostarska 15";
            izvrsilac.StambeniBroj = "3B";
            izvrsilac.ProsjecnaOcjena = 4.9M;
            izvrsilac.BrojZavrsenihPoslova = 25;
            izvrsilac.Opis = "Iskusan izvršilac";
            izvrsilac.MinCijenaUsluge = 100;
            izvrsilac.BrojRecenzija = 12;

            // Assert
            Assert.AreEqual("Mostarska 15", izvrsilac.Adresa);
            Assert.AreEqual("3B", izvrsilac.StambeniBroj);
            Assert.AreEqual(4.9M, izvrsilac.ProsjecnaOcjena);
            Assert.AreEqual(25, izvrsilac.BrojZavrsenihPoslova);
            Assert.AreEqual("Iskusan izvršilac", izvrsilac.Opis);
            Assert.AreEqual(100, izvrsilac.MinCijenaUsluge);
            Assert.AreEqual(12, izvrsilac.BrojRecenzija);
        }

        [TestMethod]
        public void ZavrsiPosao_TrenutnaImplementacija_NeMijenjaBrojPoslova()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            izvrsilac.BrojZavrsenihPoslova = 5;

            // Act
            izvrsilac.ZavrsiPosao();

            // Assert
            Assert.AreEqual(
                5,
                izvrsilac.BrojZavrsenihPoslova);
        }

        [TestMethod]
        public void DodajKategoriju_TrenutnaImplementacija_NeMijenjaKolekciju()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();
            izvrsilac.Kategorije =
                new List<IzvrsilacKategorija>();

            var kategorija = new Kategorija();

            // Act
            izvrsilac.DodajKategoriju(kategorija);

            // Assert
            Assert.IsNotNull(izvrsilac.Kategorije);
            Assert.AreEqual(
                0,
                izvrsilac.Kategorije.Count);
        }

        [TestMethod]
        public void UkloniKategoriju_TrenutnaImplementacija_NeMijenjaKolekciju()
        {
            // Arrange
            var izvrsilac = KreirajIzvrsioca();

            izvrsilac.Kategorije =
                new List<IzvrsilacKategorija>
                {
                    new IzvrsilacKategorija
                    {
                        KategorijaID = 3
                    }
                };

            // Act
            izvrsilac.UkloniKategoriju(3);

            // Assert
            Assert.IsNotNull(izvrsilac.Kategorije);
            Assert.AreEqual(
                1,
                izvrsilac.Kategorije.Count);

            Assert.AreEqual(
                3,
                izvrsilac.Kategorije.First().KategorijaID);
        }
    }
}
