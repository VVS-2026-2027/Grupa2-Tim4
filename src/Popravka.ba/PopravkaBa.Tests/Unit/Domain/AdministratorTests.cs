
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Domain
{
    [TestClass]
    public class AdministratorTests
    {
        [TestMethod]
        public void Aktivan_PozvanZaAdministratora_VracaAktivan()
        {
            // Arrange - priprema objekta
            var administrator = new Administrator();

            // Act - poziv metode koju testiramo
            var rezultat = administrator.Aktivan();

            // Assert - provjera rezultata
            Assert.AreEqual(Status.Aktivan, rezultat);
        }
    }
}
