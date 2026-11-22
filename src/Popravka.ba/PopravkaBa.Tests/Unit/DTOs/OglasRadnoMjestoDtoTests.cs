using System.ComponentModel.DataAnnotations;
using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class ObjaviOglasRadnoMjestoDtoTests
    {
        private static ObjaviOglasRadnoMjestoDto ValidanDto() => new()
        {
            Naslov = "Vozac kamiona",
            Opis = "Potreban vozac sa iskustvom",
            MjestoID = 1,
            BrojIzvrsilaca = 1,
            VrstaZaposlenja = VrstaZaposlenja.PunoRadnoVrijeme,
            MinPrihod = 1500,
            MaxPrihod = 2500,
            TipIsplate = TipIsplate.Mjesecno,
            Uvjeti = new List<string> { "Vozacka dozvola B", "Dvije godine iskustva", "Poznavanje ruta" },
            VozackeDozvole = new List<VozackaDozvola> { VozackaDozvola.B, VozackaDozvola.CE },
            KategorijeID = new List<int> { 1 },
            Slika = "https://cdn.example.com/posao.jpg"
        };

        [TestMethod]
        public void NoviDto_BrojIzvrsilacaJeJedan()
        {
            // Akcija
            var dto = new ObjaviOglasRadnoMjestoDto();

            // Provjera
            Assert.AreEqual(1, dto.BrojIzvrsilaca);
            Assert.IsNull(dto.MinPrihod);
            Assert.IsNull(dto.MaxPrihod);
            Assert.IsNull(dto.Slika);
            Assert.IsNull(dto.VozackeDozvole);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual("Vozac kamiona", dto.Naslov);
            Assert.AreEqual("Potreban vozac sa iskustvom", dto.Opis);
            Assert.AreEqual(1, dto.MjestoID);
            Assert.AreEqual(1, dto.BrojIzvrsilaca);
            Assert.AreEqual(VrstaZaposlenja.PunoRadnoVrijeme, dto.VrstaZaposlenja);
            Assert.AreEqual(1500, dto.MinPrihod);
            Assert.AreEqual(2500, dto.MaxPrihod);
            Assert.AreEqual(TipIsplate.Mjesecno, dto.TipIsplate);
            CollectionAssert.AreEqual(
                new List<string> { "Vozacka dozvola B", "Dvije godine iskustva", "Poznavanje ruta" },
                dto.Uvjeti);
            CollectionAssert.AreEqual(
                new List<VozackaDozvola> { VozackaDozvola.B, VozackaDozvola.CE },
                dto.VozackeDozvole);
            CollectionAssert.AreEqual(new List<int> { 1 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/posao.jpg", dto.Slika);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        [DataRow(2147483647, true)]
        public void BrojIzvrsilaca_PostujeGranicu(int brojIzvrsilaca, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.BrojIzvrsilaca = brojIzvrsilaca;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow(-1, false)]
        [DataRow(0, true)]
        [DataRow(1000, true)]
        public void Prihodi_PostujuGranicu(int prihod, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.MinPrihod = prihod;
            dto.MaxPrihod = prihod;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [DataTestMethod]
        [DataRow(0, true)]
        [DataRow(3, true)]
        [DataRow(4, false)]
        public void Uvjeti_PostujuMaksimalnoTriStavke(int brojUvjeta, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Uvjeti = Enumerable.Range(1, brojUvjeta)
                                   .Select(i => $"Uvjet {i}")
                                   .ToList();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NedostajeNaslov_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Naslov = null!;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasRadnoMjestoDto.Naslov)));
        }

        [TestMethod]
        public void NedostajeOpis_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Opis = null!;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasRadnoMjestoDto.Opis)));
        }

        [TestMethod]
        public void PraznaListaKategorija_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.KategorijeID = new List<int>();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(ObjaviOglasRadnoMjestoDto.KategorijeID)));
        }

        [TestMethod]
        public void Validate_BezPrihoda_NemaGresaka()
        {
            // Priprema
            var dto = new ObjaviOglasRadnoMjestoDto { MinPrihod = null, MaxPrihod = null };

            // Akcija
            var rezultati = dto.Validate(new ValidationContext(dto)).ToList();

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }

        [TestMethod]
        public void Validate_MaksimalniManjiOdMinimalnog_VracaGreskuNaNivouMaksimalnog()
        {
            // Priprema
            var servisi = new Mock<IServiceProvider>();
            var dto = new ObjaviOglasRadnoMjestoDto { MinPrihod = 2000, MaxPrihod = 1000 };

            // Akcija
            var rezultati = dto.Validate(new ValidationContext(dto, servisi.Object, null)).ToList();

            // Provjera
            Assert.AreEqual(1, rezultati.Count);
            CollectionAssert.Contains(
                rezultati[0].MemberNames.ToList(),
                nameof(ObjaviOglasRadnoMjestoDto.MaxPrihod));
            StringAssert.Contains(rezultati[0].ErrorMessage!, "Maksimalan prihod ne može biti manji od minimalnog.");
            servisi.Verify(s => s.GetService(It.IsAny<Type>()), Times.Never);
        }

        [TestMethod]
        public void Validate_JednakiPrihodi_NemaGresaka()
        {
            // Priprema
            var dto = new ObjaviOglasRadnoMjestoDto { MinPrihod = 1500, MaxPrihod = 1500 };

            // Akcija
            var rezultati = dto.Validate(new ValidationContext(dto)).ToList();

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }
    }

    [TestClass]
    public class UrediOglasRadnoMjestoDtoTests
    {
        private static UrediOglasRadnoMjestoDto ValidanDto() => new()
        {
            OglasID = 3,
            Naslov = "Azuriran oglas",
            Opis = "Azuriran opis posla",
            MjestoID = 2,
            BrojIzvrsilaca = 2,
            VrstaZaposlenja = VrstaZaposlenja.UgovorDjelo,
            MinPrihod = 1000,
            MaxPrihod = 2000,
            TipIsplate = TipIsplate.Jednokratno,
            Uvjeti = new List<string> { "Iskustvo" },
            VozackeDozvole = new List<VozackaDozvola> { VozackaDozvola.C },
            KategorijeID = new List<int> { 4 },
            Slika = "https://cdn.example.com/uredi-posao.jpg"
        };

        [TestMethod]
        public void NoviDto_BrojIzvrsilacaJeJedan()
        {
            // Akcija
            var dto = new UrediOglasRadnoMjestoDto();

            // Provjera
            Assert.AreEqual(1, dto.BrojIzvrsilaca);
            Assert.IsNull(dto.MinPrihod);
            Assert.IsNull(dto.MaxPrihod);
            Assert.IsNull(dto.Slika);
            Assert.IsNull(dto.VozackeDozvole);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija i provjera
            Assert.AreEqual(3, dto.OglasID);
            Assert.AreEqual("Azuriran oglas", dto.Naslov);
            Assert.AreEqual("Azuriran opis posla", dto.Opis);
            Assert.AreEqual(2, dto.MjestoID);
            Assert.AreEqual(2, dto.BrojIzvrsilaca);
            Assert.AreEqual(VrstaZaposlenja.UgovorDjelo, dto.VrstaZaposlenja);
            Assert.AreEqual(1000, dto.MinPrihod);
            Assert.AreEqual(2000, dto.MaxPrihod);
            Assert.AreEqual(TipIsplate.Jednokratno, dto.TipIsplate);
            CollectionAssert.AreEqual(new List<string> { "Iskustvo" }, dto.Uvjeti);
            CollectionAssert.AreEqual(new List<VozackaDozvola> { VozackaDozvola.C }, dto.VozackeDozvole);
            CollectionAssert.AreEqual(new List<int> { 4 }, dto.KategorijeID);
            Assert.AreEqual("https://cdn.example.com/uredi-posao.jpg", dto.Slika);
        }

        [TestMethod]
        public void ValidanDto_NemaGresaka()
        {
            // Priprema
            var dto = ValidanDto();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }

        [TestMethod]
        public void NevalidanBrojIzvrsilaca_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.BrojIzvrsilaca = 0;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasRadnoMjestoDto.BrojIzvrsilaca)));
        }

        [TestMethod]
        public void NegativanPrihod_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.MinPrihod = -1;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasRadnoMjestoDto.MinPrihod)));
        }

        [TestMethod]
        public void PreviseUvjeta_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Uvjeti = new List<string> { "Jedan", "Dva", "Tri", "Cetiri" };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasRadnoMjestoDto.Uvjeti)));
        }

        [TestMethod]
        public void PraznaListaKategorija_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.KategorijeID = new List<int>();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(UrediOglasRadnoMjestoDto.KategorijeID)));
        }

        [TestMethod]
        public void Validate_MaksimalniManjiOdMinimalnog_VracaGresku()
        {
            // Priprema
            var dto = new UrediOglasRadnoMjestoDto { MinPrihod = 3000, MaxPrihod = 500 };

            // Akcija
            var rezultati = dto.Validate(new ValidationContext(dto)).ToList();

            // Provjera
            Assert.AreEqual(1, rezultati.Count);
            CollectionAssert.Contains(
                rezultati[0].MemberNames.ToList(),
                nameof(UrediOglasRadnoMjestoDto.MaxPrihod));
        }
    }
}
