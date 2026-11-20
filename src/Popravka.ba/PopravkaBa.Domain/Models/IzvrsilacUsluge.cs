
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

        // Pragovi za nivo izvršioca (minimalni broj poslova, recenzija i prosječna ocjena)
        private const int MinPoslovaTopMajstor = 10;
        private const int MinRecenzijaTopMajstor = 5;
        private const decimal MinOcjenaTopMajstor = 4.5m;

        private const int MinPoslovaPouzdan = 3;
        private const int MinRecenzijaPouzdan = 3;
        private const decimal MinOcjenaPouzdan = 4.0m;

        public NivoIzvrsioca DajNivo()
        {
            if (Ispunjava(MinPoslovaTopMajstor, MinRecenzijaTopMajstor, MinOcjenaTopMajstor))
                return NivoIzvrsioca.TopMajstor;

            if (Ispunjava(MinPoslovaPouzdan, MinRecenzijaPouzdan, MinOcjenaPouzdan))
                return NivoIzvrsioca.Pouzdan;

            return NivoIzvrsioca.Novi;
        }

        private bool Ispunjava(int minPoslova, int minRecenzija, decimal minOcjena) =>
            BrojZavrsenihPoslova >= minPoslova
            && BrojRecenzija >= minRecenzija
            && ProsjecnaOcjena >= minOcjena;
    }
}
