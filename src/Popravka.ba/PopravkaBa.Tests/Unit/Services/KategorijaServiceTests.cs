using PopravkaBa.Application.Services;
using PopravkaBa.Domain.Interfaces.Repositories;
using PopravkaBa.Domain.Models;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class KategorijaServiceTests
    {
        private Mock<IKategorijaRepository> _repo = null!;
        private KategorijaService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IKategorijaRepository>();
            _servis = new KategorijaService(_repo.Object);
        }

        // Data-driven: podaci iz TestData/kategorije_oglasa.csv 

        public static IEnumerable<object[]> KategorijeOglasaIzCsv =>
            CsvHelper.Ucitaj("kategorije_oglasa.csv", k => new object[]
            {
                int.Parse(k[0]),
                k[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            });

        [TestMethod]
        [DynamicData(nameof(KategorijeOglasaIzCsv))]
        public async Task DodajKategorijeOglasu_PodaciIzCsv_ProsljedjujeRepozitorijuJednom(int oglasId, List<int> kategorije)
        {
            // Act
            await _servis.DodajKategorijeOglasu(oglasId, kategorije);

            // Assert
            _repo.Verify(r => r.DodajKategorijeOglasu(oglasId, kategorije), Times.Once);
        }


        [TestMethod]
        public async Task DajSveKategorije_RepozitorijImaKategorije_VracaSveKategorije()
        {
            
            var kategorije = new List<Kategorija> { new Kategorija(), new Kategorija() };
            _repo.Setup(r => r.DajSveAsync()).ReturnsAsync(kategorije);

            
            var rezultat = await _servis.DajSveKategorije();

            CollectionAssert.AreEqual(kategorije, rezultat.ToList());
        }

        [TestMethod]
        public async Task DajSveKategorije_RepozitorijPrazan_VracaPraznuKolekciju()
        {
           
            _repo.Setup(r => r.DajSveAsync()).ReturnsAsync(new List<Kategorija>());
            var rezultat = await _servis.DajSveKategorije();
            Assert.AreEqual(0, rezultat.Count());
        }


        [TestMethod]
        public async Task UkloniSveKategorijeOglasa_OglasId_PozivaRepozitorijJednom()
        {
            await _servis.UkloniSveKategorijeOglasa(5);

            _repo.Verify(r => r.UkloniSveKategorijeOglasa(5), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajKategorijeOglasa_NoveKategorije_PozivaRepozitorijSaIstimPodacima()
        {
            var kategorije = new List<int> { 2, 4 };
            await _servis.AzurirajKategorijeOglasa(7, kategorije);
            _repo.Verify(r => r.AzurirajKategorijeOglasa(7, kategorije), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajKategorijeOglasa_NoveKategorije_NePozivaDodavanjeNiBrisanje()
        {
            await _servis.AzurirajKategorijeOglasa(7, new List<int> { 1 });
            _repo.Verify(r => r.DodajKategorijeOglasu(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
            _repo.Verify(r => r.UkloniSveKategorijeOglasa(It.IsAny<int>()), Times.Never);
        }

        // Izvršilac

        [TestMethod]
        public async Task DodajKategorijeIzvrsiocu_ListaKategorija_PozivaRepozitorijJednom()
        {
         
            var kategorije = new List<int> { 1, 3 };

            
            await _servis.DodajKategorijeIzvrsiocu("izvrsilac-1", kategorije);
            _repo.Verify(r => r.DodajKategorijeIzvrsiocu("izvrsilac-1", kategorije), Times.Once);
        }

        [TestMethod]
        public async Task AzurirajKategorijeIzvrsioca_NoveKategorije_PozivaRepozitorijSaIstimPodacima()
        {
            
            var kategorije = new List<int> { 6 };
            await _servis.AzurirajKategorijeIzvrsioca("izvrsilac-2", kategorije);

            _repo.Verify(r => r.AzurirajKategorijeIzvrsioca("izvrsilac-2", kategorije), Times.Once);
        }

        //  Top kategorije

        [TestMethod]
        public async Task DajTopKategorijePoMajstorimaAsync_TopN_VracaRezultatRepozitorija()
        {
           
            var top = new List<(Kategorija, int)> { (new Kategorija(), 12), (new Kategorija(), 8) };
            _repo.Setup(r => r.DajTopKategorijePoMajstorimaAsync(2)).ReturnsAsync(top);

            
            var rezultat = await _servis.DajTopKategorijePoMajstorimaAsync(2);

           
            CollectionAssert.AreEqual(top, rezultat.ToList());
        }

        [TestMethod]
        public async Task DajTopKategorijePoMajstorimaAsync_TopN_PozivaRepozitorijSaIstimBrojem()
        {
           
            await _servis.DajTopKategorijePoMajstorimaAsync(3);
            _repo.Verify(r => r.DajTopKategorijePoMajstorimaAsync(3), Times.Once);
        }
    }
}