
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Shared;

namespace PopravkaBa.Tests.Unit.Domain.Shared
{
    [TestClass]
    public class NameFormatterTests
    {
        [DataTestMethod]
        [DataRow("emir", "Emir")]
        [DataRow("adnan", "Adnan")]
        [DataRow("AMIR", "Amir")]
        [DataRow("mArKo", "Marko")]
        [DataRow("LEJLA", "Lejla")]
        public void NormalizeName_JednoIme_VracaPravilnoFormatiranoIme(
            string ulaz,
            string ocekivano)
        {
            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow("emir dzafic", "Emir Dzafic")]
        [DataRow("ADNAN TUTIC", "Adnan Tutic")]
        [DataRow("aMiR hOdZiC", "Amir Hodzic")]
        [DataRow("LEJLA HADZIC", "Lejla Hadzic")]
        public void NormalizeName_ImeIPrezime_PravilnoFormatiraObjeRijeci(
            string ulaz,
            string ocekivano)
        {
            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow("  emir  ", "Emir")]
        [DataRow(" adnan tutic ", "Adnan Tutic")]
        [DataRow("   AMIR   ", "Amir")]
        [DataRow("\temir\t", "Emir")]
        public void NormalizeName_RazmaciNaKrajevima_UklanjaRazmake(
            string ulaz,
            string ocekivano)
        {
            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [DataTestMethod]
        [DataRow("čedomir", "Čedomir")]
        [DataRow("ĆAMIL", "Ćamil")]
        [DataRow("ŽELJKO", "Željko")]
        [DataRow("ŠEJLA", "Šejla")]
        [DataRow("ĐENAN", "Đenan")]
        [DataRow("EMIR DŽAFIĆ", "Emir Džafić")]
        [DataRow("ADNAN TUTIĆ", "Adnan Tutić")]
        [DataRow("AMIR HODŽIĆ", "Amir Hodžić")]
        public void NormalizeName_BosanskaSlova_PravilnoFormatira(
            string ulaz,
            string ocekivano)
        {
            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public void NormalizeName_Null_VracaNull()
        {
            // Arrange
            string? ulaz = null;

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz!);

            // Assert
            Assert.IsNull(rezultat);
        }

        [TestMethod]
        public void NormalizeName_PrazanString_VracaPrazanString()
        {
            // Arrange
            var ulaz = string.Empty;

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(string.Empty, rezultat);
        }

        [DataTestMethod]
        [DataRow(" ")]
        [DataRow("   ")]
        [DataRow("\t")]
        [DataRow("\n")]
        [DataRow(" \t \n ")]
        public void NormalizeName_SamoRazmaci_VracaOriginalniUlaz(
            string ulaz)
        {
            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(ulaz, rezultat);
        }

        [TestMethod]
        public void NormalizeName_ViseRijeci_FormatiraSvakuRijec()
        {
            // Arrange
            const string ulaz = "EMIR ADNAN TUTIĆ";

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(
                "Emir Adnan Tutić",
                rezultat);
        }

        [TestMethod]
        public void NormalizeName_VecFormatiranoIme_OstajeIsto()
        {
            // Arrange
            const string ulaz = "Emir Džafić";

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual(
                "Emir Džafić",
                rezultat);
        }

        [TestMethod]
        public void NormalizeName_PonovljenaNormalizacija_DajeIstiRezultat()
        {
            // Arrange
            const string ulaz = "  EMIR DŽAFIĆ  ";

            // Act
            var prviRezultat = NameFormatter.NormalizeName(ulaz);

            var drugiRezultat =
                NameFormatter.NormalizeName(prviRezultat);

            // Assert
            Assert.AreEqual(prviRezultat, drugiRezultat);
            Assert.AreEqual("Emir Džafić", drugiRezultat);
        }

        [TestMethod]
        public void NormalizeName_JednoSlovo_VracaVelikoSlovo()
        {
            // Arrange
            const string ulaz = "e";

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual("E", rezultat);
        }

        [TestMethod]
        public void NormalizeName_Brojevi_OstajuNepromenjeni()
        {
            // Arrange
            const string ulaz = "12345";

            // Act
            var rezultat = NameFormatter.NormalizeName(ulaz);

            // Assert
            Assert.AreEqual("12345", rezultat);
        }
    }
}
