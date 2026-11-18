using PopravkaBa.Domain.Enums;

namespace PopravkaBa.Domain.Shared
{
    // TDD (RED): provjera da li posjedovane vozačke dozvole pokrivaju traženu kategoriju.
    //
    // Specifikacija:
    //  - svaka kategorija pokriva samu sebe
    //  - direktna pokrivanja:
    //      A   -> A1
    //      B   -> B1
    //      BE  -> B
    //      C1  -> B
    //      C   -> C1
    //      C1E -> C1, BE
    //      CE  -> C, C1E
    //      D1  -> B
    //      D   -> D1
    //      DE  -> D, BE
    //  - pokrivanje je tranzitivno (CE -> C -> C1 -> B -> B1)
    //  - "Sve" pokriva svaku kategoriju; tražena "Sve" je pokrivena samo posjedovanom "Sve"
    //  - prazna lista posjedovanih ne pokriva ništa
    //  - posjedovane == null -> ArgumentNullException
    public static class HijerarhijaVozackihDozvola
    {
        public static bool Pokriva(IEnumerable<VozackaDozvola> posjedovane, VozackaDozvola trazena)
        {
            throw new NotImplementedException();
        }
    }
}
