
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class OglasUslugeTests
    {
        [TestMethod]
        public void NoviOglas_MinBudzetJeNula()
        {
            // Arrange i Act
            var oglas = new OglasUsluge();

            // Assert
            Assert.AreEqual(0, oglas.MinBudzet);
        }

        [TestMethod]
        public void NoviOglas_MaxBudzetJeNula()
        {
            // Arrange i Act
            var oglas = new OglasUsluge();

            // Assert
            Assert.AreEqual(0, oglas.MaxBudzet);
        }

        [TestMethod]
        public void NoviOglas_PonudeSuNull()
        {
            // Arrange i Act
            var oglas = new OglasUsluge();

            // Assert
            Assert.IsNull(oglas.Ponude);
        }

        [TestMethod]
        public void MinBudzet_PostavljanjeVrijednosti_CuvaBudzet()
        {
            // Arrange
            var oglas = new OglasUsluge();

            // Act
            oglas.MinBudzet = 100;

            // Assert
            Assert.AreEqual(100, oglas.MinBudzet);
        }

        [TestMethod]
        public void MaxBudzet_PostavljanjeVrijednosti_CuvaBudzet()
        {
            // Arrange
            var oglas = new OglasUsluge();

            // Act
            oglas.MaxBudzet = 500;

            // Assert
            Assert.AreEqual(500, oglas.MaxBudzet);
        }

        [TestMethod]
        public void Budzet_PostavljanjeObjeVrijednosti_CuvaRaspon()
        {
            // Arrange
            var oglas = new OglasUsluge();

            // Act
            oglas.MinBudzet = 150;
            oglas.MaxBudzet = 800;

            // Assert
            Assert.AreEqual(150, oglas.MinBudzet);
            Assert.AreEqual(800, oglas.MaxBudzet);
        }

        [TestMethod]
        public void Budzet_PromjenaVrijednosti_CuvaNoviRaspon()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                MinBudzet = 100,
                MaxBudzet = 500
            };

            // Act
            oglas.MinBudzet = 200;
            oglas.MaxBudzet = 1000;

            // Assert
            Assert.AreEqual(200, oglas.MinBudzet);
            Assert.AreEqual(1000, oglas.MaxBudzet);
        }

        [TestMethod]
        public void Ponude_PostavljanjePrazneKolekcije_CuvaReferencu()
        {
            // Arrange
            var oglas = new OglasUsluge();
            var ponude = new List<PonudaUsluge>();

            // Act
            oglas.Ponude = ponude;

            // Assert
            Assert.AreSame(ponude, oglas.Ponude);
            Assert.AreEqual(0, oglas.Ponude.Count);
        }

        [TestMethod]
        public void Ponude_PostavljanjeVisePonuda_CuvaKolekciju()
        {
            // Arrange
            var oglas = new OglasUsluge();

            var ponude = new List<PonudaUsluge>
            {
                new PonudaUsluge(),
                new PonudaUsluge()
            };

            // Act
            oglas.Ponude = ponude;

            // Assert
            Assert.AreSame(ponude, oglas.Ponude);
            Assert.AreEqual(2, oglas.Ponude.Count);
        }

        [TestMethod]
        public void Ponude_ZamjenaKolekcije_KoristiNovuKolekciju()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    new PonudaUsluge()
                }
            };

            var novePonude = new List<PonudaUsluge>();

            // Act
            oglas.Ponude = novePonude;

            // Assert
            Assert.AreSame(novePonude, oglas.Ponude);
            Assert.AreEqual(0, oglas.BrojPrijava);
        }

        [TestMethod]
        public void BrojPrijava_NullKolekcija_VracaNula()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = null
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(0, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_PraznaKolekcija_VracaNula()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>()
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(0, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_JednaPonuda_VracaJedan()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    new PonudaUsluge()
                }
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(1, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_VisePonuda_VracaTacanBroj()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    new PonudaUsluge(),
                    new PonudaUsluge(),
                    new PonudaUsluge()
                }
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(3, rezultat);
        }

        [TestMethod]
        public void BrojPrijava_DodavanjePonude_PovecavaBroj()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>()
            };

            // Act
            oglas.Ponude.Add(new PonudaUsluge());

            // Assert
            Assert.AreEqual(1, oglas.BrojPrijava);
        }

        [TestMethod]
        public void BrojPrijava_UklanjanjePonude_SmanjujeBroj()
        {
            // Arrange
            var prvaPonuda = new PonudaUsluge();

            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    prvaPonuda,
                    new PonudaUsluge()
                }
            };

            // Act
            oglas.Ponude.Remove(prvaPonuda);

            // Assert
            Assert.AreEqual(1, oglas.BrojPrijava);
        }

        [TestMethod]
        public void BrojPrijava_BrisanjeSvihPonuda_VracaNula()
        {
            // Arrange
            var oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    new PonudaUsluge(),
                    new PonudaUsluge()
                }
            };

            // Act
            oglas.Ponude.Clear();

            // Assert
            Assert.AreEqual(0, oglas.BrojPrijava);
        }

        [TestMethod]
        public void OglasUsluge_NasljedjujeOglas()
        {
            // Arrange i Act
            var oglas = new OglasUsluge();

            // Assert
            Assert.IsInstanceOfType(
                oglas,
                typeof(Oglas));
        }

        [TestMethod]
        public void NoviOglas_NaslijedjeniStatusJeAktivan()
        {
            // Arrange i Act
            var oglas = new OglasUsluge();

            // Assert
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }

        [TestMethod]
        public void NoviOglas_NaslijedjeniBrojPrijavaKoristiPonude()
        {
            // Arrange
            Oglas oglas = new OglasUsluge
            {
                Ponude = new List<PonudaUsluge>
                {
                    new PonudaUsluge(),
                    new PonudaUsluge()
                }
            };

            // Act
            var rezultat = oglas.BrojPrijava;

            // Assert
            Assert.AreEqual(2, rezultat);
        }

        [TestMethod]
        public void OglasUsluge_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            var ponude = new List<PonudaUsluge>
            {
                new PonudaUsluge(),
                new PonudaUsluge()
            };

            // Act
            var oglas = new OglasUsluge
            {
                OglasID = 10,
                Naslov = "Popravka bojlera",
                Opis = "Potreban majstor za popravku bojlera.",
                MinBudzet = 100,
                MaxBudzet = 500,
                Ponude = ponude,
                StatusOglasa = Status.Aktivan
            };

            // Assert
            Assert.AreEqual(10, oglas.OglasID);
            Assert.AreEqual(
                "Popravka bojlera",
                oglas.Naslov);
            Assert.AreEqual(
                "Potreban majstor za popravku bojlera.",
                oglas.Opis);
            Assert.AreEqual(100, oglas.MinBudzet);
            Assert.AreEqual(500, oglas.MaxBudzet);
            Assert.AreSame(ponude, oglas.Ponude);
            Assert.AreEqual(2, oglas.BrojPrijava);
            Assert.AreEqual(
                Status.Aktivan,
                oglas.StatusOglasa);
        }
    }
}
