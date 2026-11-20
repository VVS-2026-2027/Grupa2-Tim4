using PopravkaBa.Domain.Enums;

namespace PopravkaBa.Domain.Shared
{
    public static class HijerarhijaVozackihDozvola
    {
        private static readonly IReadOnlyDictionary<VozackaDozvola, VozackaDozvola[]> DirektnoPokriva =
            new Dictionary<VozackaDozvola, VozackaDozvola[]>
            {
                [VozackaDozvola.A]   = [VozackaDozvola.A1],
                [VozackaDozvola.B]   = [VozackaDozvola.B1],
                [VozackaDozvola.BE]  = [VozackaDozvola.B],
                [VozackaDozvola.C1]  = [VozackaDozvola.B],
                [VozackaDozvola.C]   = [VozackaDozvola.C1],
                [VozackaDozvola.C1E] = [VozackaDozvola.C1, VozackaDozvola.BE],
                [VozackaDozvola.CE]  = [VozackaDozvola.C, VozackaDozvola.C1E],
                [VozackaDozvola.D1]  = [VozackaDozvola.B],
                [VozackaDozvola.D]   = [VozackaDozvola.D1],
                [VozackaDozvola.DE]  = [VozackaDozvola.D, VozackaDozvola.BE],
            };

        public static bool Pokriva(IEnumerable<VozackaDozvola> posjedovane, VozackaDozvola trazena)
        {
            ArgumentNullException.ThrowIfNull(posjedovane);

            return posjedovane.Any(posjedovana => PokrivaKategoriju(posjedovana, trazena));
        }
        private static bool PokrivaKategoriju(VozackaDozvola posjedovana, VozackaDozvola trazena)
        {
            if (posjedovana == VozackaDozvola.Sve || posjedovana == trazena)
                return true;

            return DirektnoPokriva.TryGetValue(posjedovana, out var pokrivene)
                && pokrivene.Any(pokrivena => PokrivaKategoriju(pokrivena, trazena));
        }
    }
}
