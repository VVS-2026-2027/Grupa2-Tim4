
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopravkaBa.Domain.Shared;

namespace PopravkaBa.Tests.Unit.Domain.Shared
{
    [TestClass]
    public class BosanskiIdentityErrorDescriberTests
    {
        private BosanskiIdentityErrorDescriber describer = null!;

        [TestInitialize]
        public void Setup()
        {
            describer = new BosanskiIdentityErrorDescriber();
        }

        private static void ProvjeriGresku(
            IdentityError greska,
            string ocekivaniKod,
            string ocekivanaPoruka)
        {
            Assert.IsNotNull(greska);
            Assert.AreEqual(ocekivaniKod, greska.Code);
            Assert.AreEqual(ocekivanaPoruka, greska.Description);
        }

        [TestMethod]
        public void DefaultError_VracaPrevedenuPoruku()
        {
            var greska = describer.DefaultError();

            ProvjeriGresku(
                greska,
                "DefaultError",
                "Došlo je do nepoznate greške.");
        }

        [TestMethod]
        public void ConcurrencyFailure_VracaPrevedenuPoruku()
        {
            var greska = describer.ConcurrencyFailure();

            ProvjeriGresku(
                greska,
                "ConcurrencyFailure",
                "Optimistička greška u istovremenosti, objekat je u međuvremenu izmijenjen.");
        }

        [TestMethod]
        public void PasswordMismatch_VracaPrevedenuPoruku()
        {
            var greska = describer.PasswordMismatch();

            ProvjeriGresku(
                greska,
                "PasswordMismatch",
                "Pogrešna lozinka.");
        }

        [TestMethod]
        public void InvalidToken_VracaPrevedenuPoruku()
        {
            var greska = describer.InvalidToken();

            ProvjeriGresku(
                greska,
                "InvalidToken",
                "Token nije validan.");
        }

        [TestMethod]
        public void RecoveryCodeRedemptionFailed_VracaPrevedenuPoruku()
        {
            var greska = describer.RecoveryCodeRedemptionFailed();

            ProvjeriGresku(
                greska,
                "RecoveryCodeRedemptionFailed",
                "Iskorištavanje koda za oporavak nije uspjelo.");
        }

        [TestMethod]
        public void LoginAlreadyAssociated_VracaPrevedenuPoruku()
        {
            var greska = describer.LoginAlreadyAssociated();

            ProvjeriGresku(
                greska,
                "LoginAlreadyAssociated",
                "Korisnik s ovom prijavom već postoji.");
        }

        [DataTestMethod]
        [DataRow("amir123")]
        [DataRow("korisnik!")]
        [DataRow("")]
        public void InvalidUserName_RazlicitaImena_UkljucujeIme(
            string korisnickoIme)
        {
            var greska = describer.InvalidUserName(korisnickoIme);

            ProvjeriGresku(
                greska,
                "InvalidUserName",
                $"Korisničko ime '{korisnickoIme}' nije validno, može sadržavati samo slova ili cifre.");
        }

        [DataTestMethod]
        [DataRow("neispravan-email")]
        [DataRow("korisnik@")]
        [DataRow("")]
        public void InvalidEmail_RazliciteAdrese_UkljucujeEmail(
            string email)
        {
            var greska = describer.InvalidEmail(email);

            ProvjeriGresku(
                greska,
                "InvalidEmail",
                $"Email '{email}' nije validan.");
        }

        [TestMethod]
        public void DuplicateUserName_VracaPrevedenuPoruku()
        {
            var greska = describer.DuplicateUserName("amir");

            ProvjeriGresku(
                greska,
                "DuplicateUserName",
                "Korisničko ime je već zauzeto.");
        }

        [TestMethod]
        public void DuplicateEmail_VracaPrevedenuPoruku()
        {
            var greska = describer.DuplicateEmail("amir@example.com");

            ProvjeriGresku(
                greska,
                "DuplicateEmail",
                "Email adresa je već registrovana.");
        }

        [DataTestMethod]
        [DataRow("Administrator!")]
        [DataRow("Nepoznata uloga")]
        [DataRow("")]
        public void InvalidRoleName_RazliciteUloge_UkljucujeNaziv(
            string uloga)
        {
            var greska = describer.InvalidRoleName(uloga);

            ProvjeriGresku(
                greska,
                "InvalidRoleName",
                $"Naziv uloge '{uloga}' nije validan.");
        }

        [DataTestMethod]
        [DataRow("Administrator")]
        [DataRow("Majstor")]
        public void DuplicateRoleName_RazliciteUloge_UkljucujeNaziv(
            string uloga)
        {
            var greska = describer.DuplicateRoleName(uloga);

            ProvjeriGresku(
                greska,
                "DuplicateRoleName",
                $"Naziv uloge '{uloga}' je već zauzet.");
        }

        [TestMethod]
        public void UserAlreadyHasPassword_VracaPrevedenuPoruku()
        {
            var greska = describer.UserAlreadyHasPassword();

            ProvjeriGresku(
                greska,
                "UserAlreadyHasPassword",
                "Korisnik već ima postavljenu lozinku.");
        }

        [TestMethod]
        public void UserLockoutNotEnabled_VracaPrevedenuPoruku()
        {
            var greska = describer.UserLockoutNotEnabled();

            ProvjeriGresku(
                greska,
                "UserLockoutNotEnabled",
                "Zaključavanje naloga nije omogućeno za ovog korisnika.");
        }

        [DataTestMethod]
        [DataRow("Administrator")]
        [DataRow("Majstor")]
        [DataRow("Klijent")]
        public void UserAlreadyInRole_RazliciteUloge_UkljucujeNaziv(
            string uloga)
        {
            var greska = describer.UserAlreadyInRole(uloga);

            ProvjeriGresku(
                greska,
                "UserAlreadyInRole",
                $"Korisnik je već u ulozi '{uloga}'.");
        }

        [DataTestMethod]
        [DataRow("Administrator")]
        [DataRow("Majstor")]
        public void UserNotInRole_RazliciteUloge_UkljucujeNaziv(
            string uloga)
        {
            var greska = describer.UserNotInRole(uloga);

            ProvjeriGresku(
                greska,
                "UserNotInRole",
                $"Korisnik nije u ulozi '{uloga}'.");
        }

        [DataTestMethod]
        [DataRow(6)]
        [DataRow(8)]
        [DataRow(12)]
        public void PasswordTooShort_RazliciteDuzine_UkljucujeBroj(
            int duzina)
        {
            var greska = describer.PasswordTooShort(duzina);

            ProvjeriGresku(
                greska,
                "PasswordTooShort",
                $"Lozinka mora imati najmanje {duzina} znakova.");
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(3)]
        [DataRow(5)]
        public void PasswordRequiresUniqueChars_RazlicitiBrojevi_UkljucujeBroj(
            int broj)
        {
            var greska = describer.PasswordRequiresUniqueChars(broj);

            ProvjeriGresku(
                greska,
                "PasswordRequiresUniqueChars",
                $"Lozinka mora sadržavati najmanje {broj} različitih znakova.");
        }

        [TestMethod]
        public void PasswordRequiresNonAlphanumeric_VracaPrevedenuPoruku()
        {
            var greska = describer.PasswordRequiresNonAlphanumeric();

            ProvjeriGresku(
                greska,
                "PasswordRequiresNonAlphanumeric",
                "Lozinka mora sadržavati barem jedan specijalni znak ili znak interpunkcije.");
        }

        [TestMethod]
        public void PasswordRequiresDigit_VracaPrevedenuPoruku()
        {
            var greska = describer.PasswordRequiresDigit();

            ProvjeriGresku(
                greska,
                "PasswordRequiresDigit",
                "Lozinka mora sadržavati barem jednu cifru.");
        }

        [TestMethod]
        public void PasswordRequiresUpper_VracaPrevedenuPoruku()
        {
            var greska = describer.PasswordRequiresUpper();

            ProvjeriGresku(
                greska,
                "PasswordRequiresUpper",
                "Lozinka mora sadržavati barem jedno veliko slovo.");
        }

        [TestMethod]
        public void PasswordRequiresLower_VracaPrevedenuPoruku()
        {
            var greska = describer.PasswordRequiresLower();

            ProvjeriGresku(
                greska,
                "PasswordRequiresLower",
                "Lozinka mora sadržavati barem jedno malo slovo.");
        }
    }
}
