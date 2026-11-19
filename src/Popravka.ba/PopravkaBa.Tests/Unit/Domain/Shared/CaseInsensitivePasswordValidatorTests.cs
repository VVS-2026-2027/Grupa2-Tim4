
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain.Shared
{
    [TestClass]
    public class CaseInsensitivePasswordValidatorTests
    {
        private CaseInsensitivePasswordValidator<ApplicationUser>
            _validator = null!;

        private ApplicationUser _user = null!;

        [TestInitialize]
        public void Setup()
        {
            _validator =
                new CaseInsensitivePasswordValidator<ApplicationUser>();

            _user = new Klijent();
        }

        [DataTestMethod]
        [DataRow("password")]
        [DataRow("PASSWORD")]
        [DataRow("Password123")]
        [DataRow("abc")]
        [DataRow("A")]
        [DataRow("a")]
        [DataRow("123a456")]
        [DataRow("!@#A123")]
        public async Task ValidateAsync_LozinkaSadrziSlovo_VracaSuccess(
            string password)
        {
            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
            Assert.AreEqual(0, rezultat.Errors.Count());
        }

        [DataTestMethod]
        [DataRow("123456")]
        [DataRow("!@#$%")]
        [DataRow("123!@#")]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("123 456")]
        public async Task ValidateAsync_LozinkaBezSlova_VracaFailed(
            string password)
        {
            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsFalse(rezultat.Succeeded);

            var greska = rezultat.Errors.Single();

            Assert.AreEqual(
                "PasswordRequiresLetter",
                greska.Code);

            Assert.AreEqual(
                "Lozinka mora sadržavati barem jedno slovo.",
                greska.Description);
        }

        [TestMethod]
        public async Task ValidateAsync_NullLozinka_VracaFailed()
        {
            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                null);

            // Assert
            Assert.IsFalse(rezultat.Succeeded);

            var greska = rezultat.Errors.Single();

            Assert.AreEqual(
                "PasswordRequiresLetter",
                greska.Code);

            Assert.AreEqual(
                "Lozinka mora sadržavati barem jedno slovo.",
                greska.Description);
        }

        [DataTestMethod]
        [DataRow("č")]
        [DataRow("ć")]
        [DataRow("ž")]
        [DataRow("š")]
        [DataRow("đ")]
        [DataRow("Ž123")]
        public async Task ValidateAsync_BosanskaSlova_VracaSuccess(
            string password)
        {
            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
            Assert.AreEqual(0, rezultat.Errors.Count());
        }

        [TestMethod]
        public async Task ValidateAsync_SlovoNaKrajuLozinke_VracaSuccess()
        {
            // Arrange
            const string password = "123456a";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
        }

        [TestMethod]
        public async Task ValidateAsync_SlovoNaPocetkuLozinke_VracaSuccess()
        {
            // Arrange
            const string password = "a123456";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
        }

        [TestMethod]
        public async Task ValidateAsync_SlovoUSrediniLozinke_VracaSuccess()
        {
            // Arrange
            const string password = "123a456";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
        }

        [TestMethod]
        public async Task ValidateAsync_UnicodeSlovo_VracaSuccess()
        {
            // Arrange
            const string password = "123Ω456";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
        }

        [TestMethod]
        public async Task ValidateAsync_NullUserILozinkaSaSlovom_VracaSuccess()
        {
            // Arrange
            const string password = "Password123";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                null!,
                password);

            // Assert
            Assert.IsTrue(rezultat.Succeeded);
        }

        [TestMethod]
        public async Task ValidateAsync_LozinkaSamoBrojevi_VracaTacnoJednuGresku()
        {
            // Arrange
            const string password = "123456789";

            // Act
            var rezultat = await _validator.ValidateAsync(
                null!,
                _user,
                password);

            // Assert
            Assert.IsFalse(rezultat.Succeeded);
            Assert.AreEqual(1, rezultat.Errors.Count());
        }
    }
}
