
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Web.Infrastructure;

namespace PopravkaBa.Tests.Unit.Domain.Shared
{
    [TestClass]
    public class BosanskiValidationMetadataProviderTests
    {
        private BosanskiValidationMetadataProvider _provider = null!;

        private class TestniModel
        {
            public string Ime { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Prezime { get; set; } = string.Empty;
        }

        [TestInitialize]
        public void Setup()
        {
            _provider = new BosanskiValidationMetadataProvider();
        }

        private static ValidationMetadataProviderContext KreirajKontekst(
            string nazivSvojstva,
            params object[] atributi)
        {
            // Pronalazimo stvarno svojstvo testnog modela.
            var property = typeof(TestniModel)
                .GetProperty(nazivSvojstva);

            Assert.IsNotNull(property);

            // Koristimo podrzani overload umjesto zastarjelog.
            var identity = ModelMetadataIdentity.ForProperty(
                property,
                property.PropertyType,
                typeof(TestniModel));

            // ModelAttributes se kreira preko javne factory metode.
            var attributes = ModelAttributes.GetAttributesForProperty(
                typeof(TestniModel),
                property);

            // Konstruktor prima samo dva argumenta.
            var context = new ValidationMetadataProviderContext(
                identity,
                attributes);

            // Dodajemo validacijske atribute za konkretan test.
            foreach (var atribut in atributi)
            {
                context.ValidationMetadata
                    .ValidatorMetadata.Add(atribut);
            }

            return context;
        }

        [TestMethod]
        public void RequiredBezPoruke_PostavljaBosanskuPoruku()
        {
            // Arrange
            var required = new RequiredAttribute();

            var context = KreirajKontekst(
                "Ime",
                required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                "Polje Ime je obavezno.",
                required.ErrorMessage);
        }

        [TestMethod]
        public void RequiredSaPostojecomPorukom_NeMijenjaPoruku()
        {
            // Arrange
            var required = new RequiredAttribute
            {
                ErrorMessage = "Unesite ime."
            };

            var context = KreirajKontekst(
                "Ime",
                required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                "Unesite ime.",
                required.ErrorMessage);
        }

        [TestMethod]
        public void RequiredSaResourcePorukom_NeMijenjaPoruku()
        {
            // Arrange
            var required = new RequiredAttribute
            {
                ErrorMessageResourceName = "RequiredMessage"
            };

            var context = KreirajKontekst(
                "Ime",
                required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.IsNull(required.ErrorMessage);

            Assert.AreEqual(
                "RequiredMessage",
                required.ErrorMessageResourceName);
        }

        [TestMethod]
        public void DrugiValidacijskiAtribut_NeMijenjaAtribut()
        {
            // Arrange
            var minLength = new MinLengthAttribute(3);

            var context = KreirajKontekst(
                "Ime",
                minLength);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(3, minLength.Length);
            Assert.IsNull(minLength.ErrorMessage);
        }

        [TestMethod]
        public void ViseAtributa_MijenjaSamoRequiredBezPoruke()
        {
            // Arrange
            var required = new RequiredAttribute();

            var minLength = new MinLengthAttribute(3);

            var drugiRequired = new RequiredAttribute
            {
                ErrorMessage = "Vec definisana poruka."
            };

            var context = KreirajKontekst(
                "Email",
                required,
                minLength,
                drugiRequired);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                "Polje Email je obavezno.",
                required.ErrorMessage);

            Assert.AreEqual(
                "Vec definisana poruka.",
                drugiRequired.ErrorMessage);

            Assert.IsNull(minLength.ErrorMessage);
        }

        [TestMethod]
        public void PraznaListaAtributa_NeBacaIzuzetak()
        {
            // Arrange
            var context = KreirajKontekst("Ime");

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                0,
                context.ValidationMetadata.ValidatorMetadata.Count);
        }

        [DataTestMethod]
        [DataRow("Ime", "Polje Ime je obavezno.")]
        [DataRow("Email", "Polje Email je obavezno.")]
        [DataRow("Prezime", "Polje Prezime je obavezno.")]
        public void Required_RazlicitaImena_PravilanTekst(
            string naziv,
            string ocekivanaPoruka)
        {
            // Arrange
            var required = new RequiredAttribute();

            var context = KreirajKontekst(
                naziv,
                required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                ocekivanaPoruka,
                required.ErrorMessage);
        }

        [TestMethod]
        public void RequiredBezNazivaSvojstva_KoristiNazivPolje()
        {
            // Arrange
            var required = new RequiredAttribute();

            var identity = ModelMetadataIdentity.ForType(
                typeof(string));

            var attributes = ModelAttributes.GetAttributesForType(
                typeof(string));

            var context = new ValidationMetadataProviderContext(
                identity,
                attributes);

            context.ValidationMetadata
                .ValidatorMetadata.Add(required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                "Polje Polje je obavezno.",
                required.ErrorMessage);
        }

        [TestMethod]
        public void RequiredSaResourceINepostavljenomPorukom_NeMijenjaResurs()
        {
            // Arrange
            var required = new RequiredAttribute
            {
                ErrorMessageResourceName = "ObaveznoPolje"
            };

            var context = KreirajKontekst(
                "Email",
                required);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.IsNull(required.ErrorMessage);

            Assert.AreEqual(
                "ObaveznoPolje",
                required.ErrorMessageResourceName);
        }

        [TestMethod]
        public void ViseRequiredAtributaBezPoruke_SvakiDobijaBosanskuPoruku()
        {
            // Arrange
            var prviRequired = new RequiredAttribute();
            var drugiRequired = new RequiredAttribute();

            var context = KreirajKontekst(
                "Prezime",
                prviRequired,
                drugiRequired);

            // Act
            _provider.CreateValidationMetadata(context);

            // Assert
            Assert.AreEqual(
                "Polje Prezime je obavezno.",
                prviRequired.ErrorMessage);

            Assert.AreEqual(
                "Polje Prezime je obavezno.",
                drugiRequired.ErrorMessage);
        }
    }
}
