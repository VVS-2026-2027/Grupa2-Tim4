using PopravkaBa.Domain.Enums;

namespace PopravkaBa.Domain.Shared
{
    public static class HijerarhijaVozackihDozvola
    {
        public static bool Pokriva(IEnumerable<VozackaDozvola> posjedovane, VozackaDozvola trazena)
        {
            if (posjedovane == null) throw new ArgumentNullException();

            foreach (var d in posjedovane)
            {
                if (d == trazena) return true;
                if (d == VozackaDozvola.Sve) return true;
                if (d == VozackaDozvola.A && trazena == VozackaDozvola.A1) return true;
                if (d == VozackaDozvola.B && trazena == VozackaDozvola.B1) return true;
                if (d == VozackaDozvola.BE && (trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.C1 && (trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.C && (trazena == VozackaDozvola.C1 || trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.C1E && (trazena == VozackaDozvola.C1 || trazena == VozackaDozvola.BE || trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.CE && (trazena == VozackaDozvola.C || trazena == VozackaDozvola.C1 || trazena == VozackaDozvola.C1E || trazena == VozackaDozvola.BE || trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.D1 && (trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.D && (trazena == VozackaDozvola.D1 || trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
                if (d == VozackaDozvola.DE && (trazena == VozackaDozvola.D || trazena == VozackaDozvola.D1 || trazena == VozackaDozvola.BE || trazena == VozackaDozvola.B || trazena == VozackaDozvola.B1)) return true;
            }
            return false;
        }
    }
}
