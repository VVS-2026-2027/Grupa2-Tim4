using PopravkaBa.Application.DTOs;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class RezultatPretrageDtoTests
    {
        [TestMethod]
        public void NoviDto_ImaZadaneKolekcije()
        {
            // Akcija
            var dto = new RezultatPretrageDto();

            // Provjera
            Assert.AreEqual(string.Empty, dto.AktivniTab);
            Assert.IsNotNull(dto.DostupniTabovi);
            Assert.AreEqual(0, dto.DostupniTabovi.Count());
            Assert.IsNotNull(dto.Izvrsioci);
            Assert.AreEqual(0, dto.Izvrsioci.Count());
            Assert.IsNotNull(dto.OglasiUsluga);
            Assert.AreEqual(0, dto.OglasiUsluga.Count());
            Assert.IsNotNull(dto.OglasiRadnihMjesta);
            Assert.AreEqual(0, dto.OglasiRadnihMjesta.Count());
            Assert.AreEqual(0, dto.TrenutnaStrana);
            Assert.AreEqual(0, dto.StavkiPoStranici);
            Assert.AreEqual(0, dto.UkupnoStavki);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var tabovi = new List<string> { "izvrsioci", "usluge", "poslovi" };
            var izvrsioci = new List<IzvrsilacListDto> { new() { Id = "majstor-1" } };
            var usluge = new List<OglasUslugeListDto> { new() { Id = 1 } };
            var poslovi = new List<OglasRadnoMjestoListDto> { new() { Id = 2 } };

            var dto = new RezultatPretrageDto
            {
                AktivniTab = "izvrsioci",
                DostupniTabovi = tabovi,
                Izvrsioci = izvrsioci,
                OglasiUsluga = usluge,
                OglasiRadnihMjesta = poslovi,
                TrenutnaStrana = 2,
                StavkiPoStranici = 20,
                UkupnoStavki = 35
            };

            // Akcija i provjera
            Assert.AreEqual("izvrsioci", dto.AktivniTab);
            CollectionAssert.AreEqual(tabovi, dto.DostupniTabovi.ToList());
            CollectionAssert.AreEqual(izvrsioci, dto.Izvrsioci.ToList());
            CollectionAssert.AreEqual(usluge, dto.OglasiUsluga.ToList());
            CollectionAssert.AreEqual(poslovi, dto.OglasiRadnihMjesta.ToList());
            Assert.AreEqual(2, dto.TrenutnaStrana);
            Assert.AreEqual(20, dto.StavkiPoStranici);
            Assert.AreEqual(35, dto.UkupnoStavki);
            Assert.AreEqual(2, dto.UkupnoStranica);
            Assert.AreEqual(21, dto.PrikazOd);
            Assert.AreEqual(35, dto.PrikazDo);
        }

        [DataTestMethod]
        [DataRow(0, 1, 20, 0, 1, 0)]
        [DataRow(1, 1, 20, 1, 1, 1)]
        [DataRow(20, 1, 20, 1, 1, 20)]
        [DataRow(21, 1, 20, 2, 1, 20)]
        [DataRow(21, 2, 20, 2, 21, 21)]
        [DataRow(76, 1, 20, 4, 1, 20)]
        [DataRow(76, 2, 20, 4, 21, 40)]
        [DataRow(76, 4, 20, 4, 61, 76)]
        [DataRow(5, 5, 1, 5, 5, 5)]
        public void Stranicenje_RacunaStraniceIPrikaz(
            int ukupnoStavki,
            int trenutnaStrana,
            int stavkiPoStranici,
            int ocekivanoStranica,
            int ocekivanoOd,
            int ocekivanoDo)
        {
            // Priprema
            var dto = new RezultatPretrageDto
            {
                UkupnoStavki = ukupnoStavki,
                TrenutnaStrana = trenutnaStrana,
                StavkiPoStranici = stavkiPoStranici
            };

            // Akcija i provjera
            Assert.AreEqual(ocekivanoStranica, dto.UkupnoStranica);
            Assert.AreEqual(ocekivanoOd, dto.PrikazOd);
            Assert.AreEqual(ocekivanoDo, dto.PrikazDo);
        }
    }

    [TestClass]
    public class IzvrsilacListDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = new IzvrsilacListDto
            {
                Id = "majstor-1",
                Username = "majstor1",
                DisplayName = "Milan Jovanovic",
                Slika = "https://cdn.example.com/majstor.jpg",
                ProsjecnaOcjena = 4.6m,
                Opis = "Vodoinstalater sa iskustvom",
                MinCijenaUsluge = 30,
                BrojZavrsenihPoslova = 40,
                BrojRecenzija = 18,
                Lokacija = "Sarajevo",
                PrvaKategorija = "Vodovod"
            };

            // Akcija i provjera
            Assert.AreEqual("majstor-1", dto.Id);
            Assert.AreEqual("majstor1", dto.Username);
            Assert.AreEqual("Milan Jovanovic", dto.DisplayName);
            Assert.AreEqual("https://cdn.example.com/majstor.jpg", dto.Slika);
            Assert.AreEqual(4.6m, dto.ProsjecnaOcjena);
            Assert.AreEqual("Vodoinstalater sa iskustvom", dto.Opis);
            Assert.AreEqual(30, dto.MinCijenaUsluge);
            Assert.AreEqual(40, dto.BrojZavrsenihPoslova);
            Assert.AreEqual(18, dto.BrojRecenzija);
            Assert.AreEqual("Sarajevo", dto.Lokacija);
            Assert.AreEqual("Vodovod", dto.PrvaKategorija);
        }
    }

    [TestClass]
    public class OglasUslugeListDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var datum = new DateTime(2026, 11, 10);
            var dto = new OglasUslugeListDto
            {
                Id = 3,
                Naslov = "Popravka bojlera",
                Opis = "Bojler ne grije vodu",
                DatumObjave = datum,
                BrojPonuda = 4,
                MinBudzet = 40,
                MaxBudzet = 120,
                VlasnikSkracenoIme = "A. H.",
                VlasnikSlika = "https://cdn.example.com/vlasnik.jpg",
                Lokacija = "Tuzla"
            };

            // Akcija i provjera
            Assert.AreEqual(3, dto.Id);
            Assert.AreEqual("Popravka bojlera", dto.Naslov);
            Assert.AreEqual("Bojler ne grije vodu", dto.Opis);
            Assert.AreEqual(datum, dto.DatumObjave);
            Assert.AreEqual(4, dto.BrojPonuda);
            Assert.AreEqual(40, dto.MinBudzet);
            Assert.AreEqual(120, dto.MaxBudzet);
            Assert.AreEqual("A. H.", dto.VlasnikSkracenoIme);
            Assert.AreEqual("https://cdn.example.com/vlasnik.jpg", dto.VlasnikSlika);
            Assert.AreEqual("Tuzla", dto.Lokacija);
        }
    }

    [TestClass]
    public class OglasRadnoMjestoListDtoTests
    {
        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var datum = new DateTime(2026, 11, 12);
            var dto = new OglasRadnoMjestoListDto
            {
                Id = 6,
                Naslov = "Stolar",
                Opis = "Potreban stolar za plakare",
                DatumObjave = datum,
                BrojPrijava = 2,
                MinPrihod = 800,
                MaxPrihod = 1200,
                VlasnikDisplayName = "Stolarija d.o.o.",
                VlasnikSlika = "https://cdn.example.com/firma.jpg",
                Lokacija = "Mostar"
            };

            // Akcija i provjera
            Assert.AreEqual(6, dto.Id);
            Assert.AreEqual("Stolar", dto.Naslov);
            Assert.AreEqual("Potreban stolar za plakare", dto.Opis);
            Assert.AreEqual(datum, dto.DatumObjave);
            Assert.AreEqual(2, dto.BrojPrijava);
            Assert.AreEqual(800, dto.MinPrihod);
            Assert.AreEqual(1200, dto.MaxPrihod);
            Assert.AreEqual("Stolarija d.o.o.", dto.VlasnikDisplayName);
            Assert.AreEqual("https://cdn.example.com/firma.jpg", dto.VlasnikSlika);
            Assert.AreEqual("Mostar", dto.Lokacija);
        }
    }
}
