using PopravkaBa.Application.DTOs;
using PopravkaBa.Application.Services.Implementation;
using PopravkaBa.Domain.Interfaces;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Unit.Services
{
    [TestClass]
    public class StatistikaServiceTests
    {
        private Mock<IMjesecnaStatistikaRepository> _repo = null!;
        private StatistikaService _servis = null!;

        [TestInitialize]
        public void Postavi()
        {
            _repo = new Mock<IMjesecnaStatistikaRepository>();
            _servis = new StatistikaService(_repo.Object);
        }

        private void PostaviRedove(StatistikaFilterDTO filter, List<MjesecnaStatistikaKompozicija> redovi)
        {
            _repo.Setup(r => r.DohvatiFiltrirano(
                    It.IsAny<int>(), It.IsAny<int>(),
                    filter.KategorijaId, filter.MjestoId, filter.AktivnaKolona, filter.Smjer,
                    It.IsAny<CancellationToken>()))
                 .ReturnsAsync(redovi);
        }


        [TestMethod]
        [DataRow(2025, 3)]
        [DataRow(2024, 1)]
        [DataRow(2026, 12)]
        public async Task DohvatiRangListu_ZadanPeriod_PozivaRepozitorijSaTimPeriodom(int godina, int mjesec)
        {
            var filter = new StatistikaFilterDTO { Godina = godina, Mjesec = mjesec };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>());

            await _servis.DohvatiRangListu(filter);

            _repo.Verify(r => r.DohvatiFiltrirano(
                godina, mjesec,
                filter.KategorijaId, filter.MjestoId, filter.AktivnaKolona, filter.Smjer,
                It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.DohvatiMeta(godina, mjesec, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DohvatiRangListu_ZadanPeriod_PeriodOstajeUFilteru()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>());

            await _servis.DohvatiRangListu(filter);

            Assert.AreEqual(2025, filter.Godina);
            Assert.AreEqual(6, filter.Mjesec);
        }

        [TestMethod]
        public async Task DohvatiRangListu_BiloKojiFilter_VracaIstiFilterURezultatu()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>());

            var rezultat = await _servis.DohvatiRangListu(filter);

            Assert.AreSame(filter, rezultat.Filter);
        }

        [TestMethod]
        public async Task DohvatiRangListu_RepozitorijVracaRedove_VracaIstiBrojRedova()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>
            {
                new MjesecnaStatistikaKompozicija { DisplayName = "Majstor A" },
                new MjesecnaStatistikaKompozicija { DisplayName = "Majstor B" },
                new MjesecnaStatistikaKompozicija { DisplayName = "Majstor C" }
            });

            var rezultat = await _servis.DohvatiRangListu(filter);

            Assert.AreEqual(3, rezultat.Redovi.Count());
        }

        [TestMethod]
        public async Task DohvatiRangListu_RepozitorijVracaRed_MapiraPodatkeIzvrsioca()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>
            {
                new MjesecnaStatistikaKompozicija { IzvrsilacID = "izvrsilac-1", DisplayName = "Majstor A", Username = "majstora" }
            });

            var rezultat = await _servis.DohvatiRangListu(filter);

            var red = rezultat.Redovi.First();
            Assert.AreEqual("izvrsilac-1", red.IzvrsilacID);
            Assert.AreEqual("Majstor A", red.DisplayName);
            Assert.AreEqual("majstora", red.Username);
        }

        [TestMethod]
        public async Task DohvatiRangListu_NemaPodataka_VracaPraznuListu()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>());

            var rezultat = await _servis.DohvatiRangListu(filter);

            Assert.AreEqual(0, rezultat.Redovi.Count());
        }

        [TestMethod]
        public async Task DohvatiRangListu_CancellationToken_ProsljedjujeSeRepozitoriju()
        {
            var filter = new StatistikaFilterDTO { Godina = 2025, Mjesec = 6 };
            PostaviRedove(filter, new List<MjesecnaStatistikaKompozicija>());
            using var cts = new CancellationTokenSource();

            await _servis.DohvatiRangListu(filter, cts.Token);

            _repo.Verify(r => r.DohvatiMeta(2025, 6, cts.Token), Times.Once);
        }
    }
}