using System.ComponentModel.DataAnnotations;

namespace PopravkaBa.Tests.Helpers
{
    // Zajednicka provjera data annotation pravila i IValidatableObject pravila na DTO objektima.
    public static class DtoValidacija
    {
        public static IReadOnlyList<ValidationResult> Validiraj(object dto)
        {
            var rezultati = new List<ValidationResult>();
            Validator.TryValidateObject(
                dto,
                new ValidationContext(dto),
                rezultati,
                validateAllProperties: true);

            return rezultati;
        }

        public static bool ImaGreskuZa(IReadOnlyList<ValidationResult> rezultati, string nazivPolja)
            => rezultati.Any(r => r.MemberNames.Contains(nazivPolja));
    }
}
