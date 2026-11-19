
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class PortfolioSlikaTests
    {
        [TestMethod]
        public void NovaPortfolioSlika_IDJeNula()
        {
            // Arrange i Act
            var slika = new PortfolioSlika();

            // Assert
            Assert.AreEqual(0, slika.PortfolioSlikaID);
        }

        [TestMethod]
        public void NovaPortfolioSlika_OpcionalniOpisJeNull()
        {
            // Arrange i Act
            var slika = new PortfolioSlika();

            // Assert
            Assert.IsNull(slika.Opis);
        }

        [TestMethod]
        public void NovaPortfolioSlika_ReferenceSuNull()
        {
            // Arrange i Act
            var slika = new PortfolioSlika();

            // Assert
            Assert.IsNull(slika.IzvrsilacID);
            Assert.IsNull(slika.Izvrsilac);
            Assert.IsNull(slika.URL);
        }

        [TestMethod]
        public void PortfolioSlikaID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var slika = new PortfolioSlika();

            // Act
            slika.PortfolioSlikaID = 10;

            // Assert
            Assert.AreEqual(10, slika.PortfolioSlikaID);
        }

        [TestMethod]
        public void IzvrsilacID_PostavljanjeVrijednosti_CuvaID()
        {
            // Arrange
            var slika = new PortfolioSlika();

            // Act
            slika.IzvrsilacID = "majstor-123";

            // Assert
            Assert.AreEqual(
                "majstor-123",
                slika.IzvrsilacID);
        }

        [TestMethod]
        public void Izvrsilac_PostavljanjeObjekta_CuvaReferencu()
        {
            // Arrange
            var slika = new PortfolioSlika();

            IzvrsilacUsluge izvrsilac = new Majstor
            {
                Id = "majstor-123"
            };

            // Act
            slika.Izvrsilac = izvrsilac;

            // Assert
            Assert.AreSame(
                izvrsilac,
                slika.Izvrsilac);
        }

        [TestMethod]
        public void Izvrsilac_FirmaKaoIzvrsilac_CuvaReferencu()
        {
            // Arrange
            var slika = new PortfolioSlika();

            IzvrsilacUsluge firma = new Firma
            {
                Id = "firma-001",
                NazivFirme = "Popravke Sarajevo"
            };

            // Act
            slika.Izvrsilac = firma;

            // Assert
            Assert.AreSame(firma, slika.Izvrsilac);
        }

        [TestMethod]
        public void Izvrsilac_ZamjenaReference_KoristiNovogIzvrsioca()
        {
            // Arrange
            var slika = new PortfolioSlika();

            var prviIzvrsilac = new Majstor
            {
                Id = "majstor-1"
            };

            var drugiIzvrsilac = new Majstor
            {
                Id = "majstor-2"
            };

            slika.Izvrsilac = prviIzvrsilac;

            // Act
            slika.Izvrsilac = drugiIzvrsilac;

            // Assert
            Assert.AreSame(
                drugiIzvrsilac,
                slika.Izvrsilac);
        }

        [TestMethod]
        public void URL_PostavljanjeVrijednosti_CuvaURL()
        {
            // Arrange
            var slika = new PortfolioSlika();

            // Act
            slika.URL =
                "https://popravka.ba/images/rad1.jpg";

            // Assert
            Assert.AreEqual(
                "https://popravka.ba/images/rad1.jpg",
                slika.URL);
        }

        [TestMethod]
        public void URL_PromjenaVrijednosti_CuvaNoviURL()
        {
            // Arrange
            var slika = new PortfolioSlika
            {
                URL = "https://popravka.ba/images/stara.jpg"
            };

            // Act
            slika.URL =
                "https://popravka.ba/images/nova.jpg";

            // Assert
            Assert.AreEqual(
                "https://popravka.ba/images/nova.jpg",
                slika.URL);
        }

        [TestMethod]
        public void Opis_PostavljanjeVrijednosti_CuvaOpis()
        {
            // Arrange
            var slika = new PortfolioSlika();

            // Act
            slika.Opis =
                "Završena renovacija kupatila.";

            // Assert
            Assert.AreEqual(
                "Završena renovacija kupatila.",
                slika.Opis);
        }

        [TestMethod]
        public void Opis_PrazanString_CuvaPraznuVrijednost()
        {
            // Arrange
            var slika = new PortfolioSlika();

            // Act
            slika.Opis = string.Empty;

            // Assert
            Assert.AreEqual(string.Empty, slika.Opis);
        }

        [TestMethod]
        public void Opis_NullVrijednost_CuvaNull()
        {
            // Arrange
            var slika = new PortfolioSlika
            {
                Opis = "Prethodni opis"
            };

            // Act
            slika.Opis = null;

            // Assert
            Assert.IsNull(slika.Opis);
        }

        [TestMethod]
        public void PortfolioSlika_SviPodaciPostavljeni_VracaOcekivaneVrijednosti()
        {
            // Arrange
            IzvrsilacUsluge izvrsilac = new Majstor
            {
                Id = "majstor-001"
            };

            // Act
            var slika = new PortfolioSlika
            {
                PortfolioSlikaID = 1,
                IzvrsilacID = "majstor-001",
                Izvrsilac = izvrsilac,
                URL = "https://popravka.ba/images/portfolio1.jpg",
                Opis = "Postavljanje keramičkih pločica"
            };

            // Assert
            Assert.AreEqual(1, slika.PortfolioSlikaID);

            Assert.AreEqual(
                "majstor-001",
                slika.IzvrsilacID);

            Assert.AreSame(
                izvrsilac,
                slika.Izvrsilac);

            Assert.AreEqual(
                "https://popravka.ba/images/portfolio1.jpg",
                slika.URL);

            Assert.AreEqual(
                "Postavljanje keramičkih pločica",
                slika.Opis);
        }

        [TestMethod]
        public void PortfolioSlika_PromjenaIzvrsiocaID_CuvaNovuVrijednost()
        {
            // Arrange
            var slika = new PortfolioSlika
            {
                IzvrsilacID = "majstor-001"
            };

            // Act
            slika.IzvrsilacID = "majstor-002";

            // Assert
            Assert.AreEqual(
                "majstor-002",
                slika.IzvrsilacID);
        }
    }
}
