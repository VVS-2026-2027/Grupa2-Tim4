
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class KlijentTests
    {
        [TestMethod]
        public void NoviKlijent_RecenzijeSuNull()
        {
            // Arrange i Act
            var klijent = new Klijent();

            // Assert
            Assert.IsNull(klijent.Recenzije);
        }

        [TestMethod]
        public void Recenzije_PostavljanjePrazneKolekcije_CuvaReferencu()
        {
            // Arrange
            var klijent = new Klijent();
            var recenzije = new List<Recenzija>();

            // Act
            klijent.Recenzije = recenzije;

            // Assert
            Assert.AreSame(recenzije, klijent.Recenzije);
            Assert.AreEqual(0, klijent.Recenzije.Count);
        }

        [TestMethod]
        public void Recenzije_DodavanjeJedneRecenzije_PovecavaBroj()
        {
            // Arrange
            var klijent = new Klijent
            {
                Recenzije = new List<Recenzija>()
            };

            var recenzija = new Recenzija();

            // Act
            klijent.Recenzije.Add(recenzija);

            // Assert
            Assert.AreEqual(1, klijent.Recenzije.Count);
            Assert.AreSame(recenzija, klijent.Recenzije.First());
        }

        [TestMethod]
        public void Recenzije_DodavanjeViseRecenzija_CuvaSveObjekte()
        {
            // Arrange
            var klijent = new Klijent
            {
                Recenzije = new List<Recenzija>()
            };

            var prva = new Recenzija();
            var druga = new Recenzija();

            // Act
            klijent.Recenzije.Add(prva);
            klijent.Recenzije.Add(druga);

            // Assert
            Assert.AreEqual(2, klijent.Recenzije.Count);
            Assert.IsTrue(klijent.Recenzije.Contains(prva));
            Assert.IsTrue(klijent.Recenzije.Contains(druga));
        }

        [TestMethod]
        public void Recenzije_ZamjenaKolekcije_KoristiNovuKolekciju()
        {
            // Arrange
            var klijent = new Klijent
            {
                Recenzije = new List<Recenzija>
                {
                    new Recenzija()
                }
            };

            var noveRecenzije = new List<Recenzija>();

            // Act
            klijent.Recenzije = noveRecenzije;

            // Assert
            Assert.AreSame(noveRecenzije, klijent.Recenzije);
            Assert.AreEqual(0, klijent.Recenzije.Count);
        }

        [DataTestMethod]
        [DataRow(true, Status.Aktivan)]
        [DataRow(false, Status.NaCekanju)]
        public void Aktivan_PotvrdaEmaila_VracaOcekivaniStatus(
            bool emailConfirmed,
            Status ocekivaniStatus)
        {
            // Arrange
            var klijent = new Klijent
            {
                EmailConfirmed = emailConfirmed
            };

            // Act
            var rezultat = klijent.Aktivan();

            // Assert
            Assert.AreEqual(ocekivaniStatus, rezultat);
        }
    }
}
