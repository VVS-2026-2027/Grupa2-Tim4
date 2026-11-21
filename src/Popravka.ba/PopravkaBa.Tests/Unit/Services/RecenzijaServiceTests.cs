using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Interfaces.Repositories;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class RecenzijaServiceTests
    {
        private Mock<IRecenzijaRepository> _recenzijaRepo = null!;
        private Mock<IPonudaUslugeRepository> _ponudaRepo = null!;
        private Mock<IIzvrsilacUslugeRepository> _izvrsilacRepo = null!;
        private RecenzijaService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _recenzijaRepo = new Mock<IRecenzijaRepository>();
            _ponudaRepo = new Mock<IPonudaUslugeRepository>();
            _izvrsilacRepo = new Mock<IIzvrsilacUslugeRepository>();
            _servis = new RecenzijaService(_recenzijaRepo.Object, _ponudaRepo.Object, _izvrsilacRepo.Object);
        }

        private void PostaviPravoNaRecenziju(bool zavrsenPosao, bool vecPostoji)
        {
            _ponudaRepo.Setup(r => r.PostojiZavrseniPosaoAsync("klijent-1", "izvrsilac-1")).ReturnsAsync(zavrsenPosao);
            _recenzijaRepo.Setup(r => r.PostojiAsync("klijent-1", "izvrsilac-1")).ReturnsAsync(vecPostoji);
        }

        private void PostaviOcjeneIzvrsioca(params int[] ocjene)
        {
            var recenzije = ocjene.Select(o => new Recenzija { Ocjena = o }).ToList();
            _recenzijaRepo.Setup(r => r.DajRecenzijeIzvrsiocaAsync("izvrsilac-1")).ReturnsAsync(recenzije);
        }


        [TestMethod]
        public async Task DajRecenzijePoId_IzvrsilacId_VracaRecenzijeIzvrsioca()
        {
            var recenzije = new List<Recenzija>();
            _recenzijaRepo.Setup(r => r.DajRecenzijeIzvrsiocaAsync("izvrsilac-1")).ReturnsAsync(recenzije);

            var rezultat = await _servis.DajRecenzijePoId("izvrsilac-1");

            Assert.AreSame(recenzije, rezultat);
        }

        [TestMethod]
        public async Task DajRecenzijuPoId_PostojeciId_VracaRecenziju()
        {
            var recenzija = new Recenzija();
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(recenzija);

            var rezultat = await _servis.DajRecenzijuPoId(1);

            Assert.AreSame(recenzija, rezultat);
        }

        [TestMethod]
        public async Task DajRecenzijuPoId_NepostojeciId_VracaNull()
        {
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(999)).ReturnsAsync((Recenzija?)null);

            var rezultat = await _servis.DajRecenzijuPoId(999);

            Assert.IsNull(rezultat);
        }

        [TestMethod]
        [DataRow(true, false, true)]
        [DataRow(false, false, false)]
        [DataRow(true, true, false)]
        [DataRow(false, true, false)]
        public async Task MozeOstavitiRecenziju_KombinacijaUslova_VracaOcekivano(bool zavrsenPosao, bool vecPostoji, bool ocekivano)
        {
            PostaviPravoNaRecenziju(zavrsenPosao, vecPostoji);

            var rezultat = await _servis.MozeOstavitiRecenziju("klijent-1", "izvrsilac-1");

            Assert.AreEqual(ocekivano, rezultat);
        }

        [TestMethod]
        public async Task MozeOstavitiRecenziju_NemaZavrsenogPosla_NeProvjeravaPostojecuRecenziju()
        {
            PostaviPravoNaRecenziju(zavrsenPosao: false, vecPostoji: false);

            await _servis.MozeOstavitiRecenziju("klijent-1", "izvrsilac-1");

            _recenzijaRepo.Verify(r => r.PostojiAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task ObjaviRecenziju_NemaPravoNaRecenziju_BacaInvalidOperationException()
        {
            PostaviPravoNaRecenziju(zavrsenPosao: false, vecPostoji: false);
            var dto = new KreirajRecenzijuDto { IzvrsilacID = "izvrsilac-1", Ocjena = 5, Komentar = "Super" };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _servis.ObjaviRecenziju(dto, "klijent-1"));
        }

        [TestMethod]
        public async Task ObjaviRecenziju_NemaPravoNaRecenziju_NeSpremaRecenziju()
        {
            PostaviPravoNaRecenziju(zavrsenPosao: true, vecPostoji: true);
            var dto = new KreirajRecenzijuDto { IzvrsilacID = "izvrsilac-1", Ocjena = 5, Komentar = "Super" };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _servis.ObjaviRecenziju(dto, "klijent-1"));

            _recenzijaRepo.Verify(r => r.DodajAsync(It.IsAny<Recenzija>()), Times.Never);
        }

        [TestMethod]
        public async Task ObjaviRecenziju_ImaPravo_SpremaRecenzijuSaTrimovanimKomentarom()
        {
            PostaviPravoNaRecenziju(zavrsenPosao: true, vecPostoji: false);
            var dto = new KreirajRecenzijuDto { IzvrsilacID = "izvrsilac-1", Ocjena = 5, Komentar = "   Odličan majstor   " };

            await _servis.ObjaviRecenziju(dto, "klijent-1");

            _recenzijaRepo.Verify(r => r.DodajAsync(It.Is<Recenzija>(x =>
                x.KlijentID == "klijent-1" &&
                x.IzvrsilacID == "izvrsilac-1" &&
                x.Ocjena == 5 &&
                x.Komentar == "Odličan majstor")), Times.Once);
        }

        [TestMethod]
        public async Task ObjaviRecenziju_ImaPravo_OsvjezavaOcjenuIzvrsioca()
        {
            PostaviPravoNaRecenziju(zavrsenPosao: true, vecPostoji: false);
            PostaviOcjeneIzvrsioca(5, 4);
            var dto = new KreirajRecenzijuDto { IzvrsilacID = "izvrsilac-1", Ocjena = 4, Komentar = "Dobro" };

            await _servis.ObjaviRecenziju(dto, "klijent-1");

            _izvrsilacRepo.Verify(r => r.OsvjeziOcjeneAsync("izvrsilac-1", 4.5m, 2), Times.Once);
        }

        [TestMethod]
        public async Task ObrisiRecenziju_NepostojecaRecenzija_BacaKeyNotFoundException()
        {
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((Recenzija?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _servis.ObrisiRecenziju(99));
        }

        [TestMethod]
        public async Task ObrisiRecenziju_PostojecaRecenzija_BriseIzRepozitorija()
        {
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(3)).ReturnsAsync(new Recenzija { IzvrsilacID = "izvrsilac-1" });

            await _servis.ObrisiRecenziju(3);

            _recenzijaRepo.Verify(r => r.ObrisiAsync(3), Times.Once);
        }

        [TestMethod]
        [DataRow(new[] { 5, 4 }, 4.5)]
        [DataRow(new[] { 5, 4, 4 }, 4.33)]
        [DataRow(new[] { 3 }, 3.0)]
        [DataRow(new int[0], 0.0)]
        public async Task ObrisiRecenziju_PreostaleOcjene_PreracunavaZaokruzenProsjek(int[] preostaleOcjene, double prosjek)
        {
            var ocekivaniProsjek = (decimal)prosjek;
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(3)).ReturnsAsync(new Recenzija { IzvrsilacID = "izvrsilac-1" });
            PostaviOcjeneIzvrsioca(preostaleOcjene);

            await _servis.ObrisiRecenziju(3);

            _izvrsilacRepo.Verify(r => r.OsvjeziOcjeneAsync("izvrsilac-1", ocekivaniProsjek, preostaleOcjene.Length), Times.Once);
        }

        [TestMethod]
        public async Task PrijaviRecenziju_NepostojecaRecenzija_BacaKeyNotFoundException()
        {
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(99)).ReturnsAsync((Recenzija?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _servis.PrijaviRecenziju(new PrijaviRecenzijuDto { RecenzijaID = 99, RazlogPrijave = "Uvredljivo" }));
        }

        [TestMethod]
        public async Task PrijaviRecenziju_VecPrijavljenaNaCekanju_BacaInvalidOperationException()
        {
            var recenzija = new Recenzija { Prijavljena = true, StatusPrijave = Status.NaCekanju };
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(recenzija);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _servis.PrijaviRecenziju(new PrijaviRecenzijuDto { RecenzijaID = 1, RazlogPrijave = "Uvredljivo" }));
        }

        [TestMethod]
        public async Task PrijaviRecenziju_NijePrijavljena_SpremaPrijavljenuRecenziju()
        {
            var recenzija = new Recenzija { Prijavljena = false };
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(recenzija);

            await _servis.PrijaviRecenziju(new PrijaviRecenzijuDto { RecenzijaID = 1, RazlogPrijave = "  Uvredljivo  " });

            _recenzijaRepo.Verify(r => r.SacuvajAsync(recenzija), Times.Once);
        }

        [TestMethod]
        public async Task OdbaciPrijavu_PrijavljenaRecenzija_PonistavaPrijavuIPostavljaOdbijeno()
        {
            var recenzija = new Recenzija { Prijavljena = true, StatusPrijave = Status.NaCekanju };
            _recenzijaRepo.Setup(r => r.DajPoIdAsync(1)).ReturnsAsync(recenzija);

            await _servis.OdbaciPrijavu(1);

            Assert.IsFalse(recenzija.Prijavljena);
            Assert.AreEqual(Status.Odbijeno, recenzija.StatusPrijave);
            _recenzijaRepo.Verify(r => r.SacuvajAsync(recenzija), Times.Once);
        }
    }
}