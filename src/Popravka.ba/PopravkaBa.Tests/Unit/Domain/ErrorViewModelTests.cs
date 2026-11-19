
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Popravka.ba.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class ErrorViewModelTests
    {
        [TestMethod]
        public void NoviObjekat_RequestIdJeNull()
        {
            // Arrange i Act
            var model = new ErrorViewModel();

            // Assert
            Assert.IsNull(model.RequestId);
        }

        [TestMethod]
        public void ShowRequestId_RequestIdPostoji_VracaTrue()
        {
            // Arrange
            var model = new ErrorViewModel
            {
                RequestId = "REQ-123"
            };

            // Act
            var rezultat = model.ShowRequestId;

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void ShowRequestId_RequestIdJeNull_VracaFalse()
        {
            // Arrange
            var model = new ErrorViewModel
            {
                RequestId = null
            };

            // Act
            var rezultat = model.ShowRequestId;

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ShowRequestId_RequestIdJePrazan_VracaFalse()
        {
            // Arrange
            var model = new ErrorViewModel
            {
                RequestId = ""
            };

            // Act
            var rezultat = model.ShowRequestId;

            // Assert
            Assert.IsFalse(rezultat);
        }

        [TestMethod]
        public void ShowRequestId_RequestIdSadrziRazmake_VracaTrue()
        {
            // Arrange
            var model = new ErrorViewModel
            {
                RequestId = "   "
            };

            // Act
            var rezultat = model.ShowRequestId;

            // Assert
            Assert.IsTrue(rezultat);
        }

        [TestMethod]
        public void RequestId_PostavljanjeVrijednosti_CuvaVrijednost()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = "REQ-456";

            // Assert
            Assert.AreEqual("REQ-456", model.RequestId);
        }

        [TestMethod]
        public void ExceptionMessage_PostavljanjeVrijednosti_CuvaPoruku()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.ExceptionMessage = "Došlo je do greške.";

            // Assert
            Assert.AreEqual(
                "Došlo je do greške.",
                model.ExceptionMessage);
        }

        [TestMethod]
        public void StackTrace_PostavljanjeVrijednosti_CuvaTekst()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.StackTrace = "at Program.Main()";

            // Assert
            Assert.AreEqual(
                "at Program.Main()",
                model.StackTrace);
        }

        [TestMethod]
        public void NoviObjekat_OpcionalnaSvojstvaSuNull()
        {
            // Arrange i Act
            var model = new ErrorViewModel();

            // Assert
            Assert.IsNull(model.ExceptionMessage);
            Assert.IsNull(model.StackTrace);
        }
    }
}
