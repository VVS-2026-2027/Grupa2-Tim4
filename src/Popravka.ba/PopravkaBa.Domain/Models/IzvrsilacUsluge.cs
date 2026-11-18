
using Microsoft.AspNetCore.Identity;
using PopravkaBa.Domain.Enums;

namespace PopravkaBa.Domain.Models
{
    public abstract class IzvrsilacUsluge : ApplicationUser
    {
        public string Adresa { get; set; }
        public string? StambeniBroj { get; set; }
        public decimal ProsjecnaOcjena { get; set; } = 0.0M;
        public int BrojZavrsenihPoslova { get; set;  } = 0;
        public string? Opis { get; set; }
        public int? MinCijenaUsluge { get; set; }
        public int BrojRecenzija { get; set; } = 0;

        public ICollection<IzvrsilacKategorija>? Kategorije { get; set; }
        public ICollection<PortfolioSlika>? SlikePortfolija { get; set; }
        public ICollection<PonudaUsluge>? Ponude { get; set; }
        public ICollection<Recenzija>? Recenzije { get; set; }

        public void ZavrsiPosao() { }
        public void DodajKategoriju(Kategorija kategorija) { }
        public void UkloniKategoriju(int kategorijaID) { }

        // TDD (RED): nivo izvršioca na osnovu broja završenih poslova, recenzija i prosječne ocjene
        //  - TopMajstor: >= 10 završenih poslova, >= 5 recenzija, ocjena >= 4.5
        //  - Pouzdan:    >= 3 završena posla,    >= 3 recenzije, ocjena >= 4.0
        //  - Novi:       sve ostalo
        public NivoIzvrsioca DajNivo()
        {
            throw new NotImplementedException();
        }
    }
}
