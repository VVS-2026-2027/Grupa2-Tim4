
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class MajstorTests
    {
        [TestMethod]
        public void NoviMajstor_PrijaveZaRadnoMjestoSuNull()
        {
            // Arrange i Act
            var majstor = new Majstor();

            // Assert
            Assert.IsNull(majstor.PrijaveZaRadnoMjesto);
        }

        [TestMethod]
        public void PrijaveZaRadnoMjesto_PostavljanjePrazneKolekcije_CuvaReferencu()
        {
            // Arrange
            var majstor = new Majstor();
            var prijave = new List<PrijavaRadnoMjesto>();

            // Act
            majstor.PrijaveZaRadnoMjesto = prijave;

            // Assert
            Assert.AreSame(
                prijave,
                majstor.PrijaveZaRadnoMjesto);

            Assert.AreEqual(
                0,
                majstor.PrijaveZaRadnoMjesto.Count);
        }

        [TestMethod]
        public void PrijaveZaRadnoMjesto_DodavanjeJednePrijave_CuvaPrijavu()
        {
            // Arrange
            var majstor = new Majstor
            {
                PrijaveZaRadnoMjesto = new List<PrijavaRadnoMjesto>()
            };

            var prijava = new PrijavaRadnoMjesto();

            // Act
            majstor.PrijaveZaRadnoMjesto.Add(prijava);

            // Assert
            Assert.AreEqual(
                1,
                majstor.PrijaveZaRadnoMjesto.Count);

            Assert.AreSame(
                prijava,
                majstor.PrijaveZaRadnoMjesto.First());
        }

        [TestMethod]
        public void PrijaveZaRadnoMjesto_DodavanjeVisePrijava_CuvaSvePrijave()
        {
            // Arrange
            var majstor = new Majstor
            {
                PrijaveZaRadnoMjesto = new List<PrijavaRadnoMjesto>()
            };

            var prvaPrijava = new PrijavaRadnoMjesto();
            var drugaPrijava = new PrijavaRadnoMjesto();

            // Act
            majstor.PrijaveZaRadnoMjesto.Add(prvaPrijava);
            majstor.PrijaveZaRadnoMjesto.Add(drugaPrijava);

            // Assert
            Assert.AreEqual(
                2,
                majstor.PrijaveZaRadnoMjesto.Count);

            Assert.IsTrue(
                majstor.PrijaveZaRadnoMjesto.Contains(prvaPrijava));

            Assert.IsTrue(
                majstor.PrijaveZaRadnoMjesto.Contains(drugaPrijava));
        }

        [TestMethod]
        public void PrijaveZaRadnoMjesto_ZamjenaKolekcije_KoristiNovuKolekciju()
        {
            // Arrange
            var staraKolekcija = new List<PrijavaRadnoMjesto>
            {
                new PrijavaRadnoMjesto()
            };

            var majstor = new Majstor
            {
                PrijaveZaRadnoMjesto = staraKolekcija
            };

            var novaKolekcija = new List<PrijavaRadnoMjesto>();

            // Act
            majstor.PrijaveZaRadnoMjesto = novaKolekcija;

            // Assert
            Assert.AreSame(
                novaKolekcija,
                majstor.PrijaveZaRadnoMjesto);

            Assert.AreEqual(
                0,
                majstor.PrijaveZaRadnoMjesto.Count);
        }

        [TestMethod]
        public void Majstor_NasljedjujeIzvrsilacUsluge()
        {
            // Arrange i Act
            var majstor = new Majstor();

            // Assert
            Assert.IsInstanceOfType(
                majstor,
                typeof(IzvrsilacUsluge));
        }

        [TestMethod]
        public void NoviMajstor_NaslijedjenaSvojstvaImajuPocetneVrijednosti()
        {
            // Arrange i Act
            var majstor = new Majstor();

            // Assert
            Assert.AreEqual(
                0,
                majstor.BrojZavrsenihPoslova);

            Assert.AreEqual(
                0.0M,
                majstor.ProsjecnaOcjena);

            Assert.AreEqual(
                0,
                majstor.BrojRecenzija);
        }
    }
}
