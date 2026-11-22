using PopravkaBa.Application.DTOs;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Tests.Helpers;

namespace PopravkaBa.Tests.Unit.DTOs
{
    [TestClass]
    public class RadnoVrijemeDtoTests
    {
        [TestMethod]
        public void NoviDto_VremenaSuNull()
        {
            // Akcija
            var dto = new RadnoVrijemeDto();

            // Provjera
            Assert.IsNull(dto.OtvorenoOd);
            Assert.IsNull(dto.OtvorenoDo);
        }

        [TestMethod]
        public void PostavljenaVremena_SeVracaju()
        {
            // Priprema
            var dto = new RadnoVrijemeDto
            {
                OtvorenoOd = new TimeSpan(8, 0, 0),
                OtvorenoDo = new TimeSpan(16, 30, 0)
            };

            // Akcija i provjera
            Assert.AreEqual(new TimeSpan(8, 0, 0), dto.OtvorenoOd);
            Assert.AreEqual(new TimeSpan(16, 30, 0), dto.OtvorenoDo);
        }
    }

    [TestClass]
    public class LoginDtoTests
    {
        [TestMethod]
        public void NoviDto_ZapamtiMeJeFalse()
        {
            // Akcija
            var dto = new LoginDto();

            // Provjera
            Assert.IsFalse(dto.ZapamtiMe);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var dto = new LoginDto
            {
                EmailUsername = "korisnik@example.com",
                Lozinka = "Lozinka123!",
                ZapamtiMe = true
            };

            // Akcija i provjera
            Assert.AreEqual("korisnik@example.com", dto.EmailUsername);
            Assert.AreEqual("Lozinka123!", dto.Lozinka);
            Assert.IsTrue(dto.ZapamtiMe);
        }

        [TestMethod]
        public void Validacija_BezEmailaIBezLozinke_VracaGreskeZaObaPolja()
        {
            // Priprema
            var dto = new LoginDto { EmailUsername = null!, Lozinka = null! };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(2, rezultati.Count);
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(LoginDto.EmailUsername)));
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(LoginDto.Lozinka)));
        }

        [TestMethod]
        public void Validacija_ValidanDto_NemaGresaka()
        {
            // Priprema
            var dto = new LoginDto
            {
                EmailUsername = "korisnik",
                Lozinka = "tajna"
            };

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(0, rezultati.Count);
        }
    }

    [TestClass]
    public class RegistracijaKlijentaDtoTests
    {
        private static RegistracijaKlijentaDto ValidanDto() => new()
        {
            KorisnickoIme = "klijent1",
            Ime = "Amina",
            Prezime = "Hadzic",
            Email = "klijent@example.com",
            Lozinka = "Lozinka123!",
            PotvrdaLozinke = "Lozinka123!",
            MjestaID = new List<int> { 1 }
        };

        [TestMethod]
        public void NoviDto_OpcijaPoljaSuNull()
        {
            // Akcija
            var dto = new RegistracijaKlijentaDto();

            // Provjera
            Assert.IsNull(dto.PotvrdaLozinke);
            Assert.IsNull(dto.MjestaID);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var mjesta = new List<int> { 2, 4 };
            var dto = ValidanDto();
            dto.MjestaID = mjesta;

            // Akcija i provjera
            Assert.AreEqual("klijent1", dto.KorisnickoIme);
            Assert.AreEqual("Amina", dto.Ime);
            Assert.AreEqual("Hadzic", dto.Prezime);
            Assert.AreEqual("klijent@example.com", dto.Email);
            Assert.AreEqual("Lozinka123!", dto.Lozinka);
            Assert.AreEqual("Lozinka123!", dto.PotvrdaLozinke);
            CollectionAssert.AreEqual(mjesta, dto.MjestaID);
        }

        [DataTestMethod]
        [DataRow(2, false)]
        [DataRow(3, true)]
        [DataRow(50, true)]
        [DataRow(51, false)]
        public void KorisnickoIme_PostujeGraniceDuzine(int duzina, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.KorisnickoIme = new string('k', duzina);

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NeispravanEmail_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Email = "nije-email";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaKlijentaDto.Email)));
        }

        [TestMethod]
        public void NedostajeLozinka_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Lozinka = null!;
            dto.PotvrdaLozinke = null;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaKlijentaDto.Lozinka)));
        }

        [TestMethod]
        public void PotvrdaLozinkeSeNePoklapa_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.PotvrdaLozinke = "DrugaLozinka1!";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaKlijentaDto.PotvrdaLozinke)));
        }

        [TestMethod]
        public void PraznaListaMjesta_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.MjestaID = new List<int>();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaKlijentaDto.MjestaID)));
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
    }

    [TestClass]
    public class RegistracijaMajstoraDtoTests
    {
        private static RegistracijaMajstoraDto ValidanDto() => new()
        {
            KorisnickoIme = "majstor1",
            Ime = "Milan",
            Prezime = "Jovanovic",
            Email = "majstor@example.com",
            Lozinka = "Lozinka123!",
            PotvrdaLozinke = "Lozinka123!",
            KategorijeID = new List<int> { 3 },
            MjestaID = new List<int> { 1 }
        };

        [TestMethod]
        public void NoviDto_KolekcijeSuNull()
        {
            // Akcija
            var dto = new RegistracijaMajstoraDto();

            // Provjera
            Assert.IsNull(dto.PotvrdaLozinke);
            Assert.IsNull(dto.KategorijeID);
            Assert.IsNull(dto.MjestaID);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var kategorije = new List<int> { 2, 6 };
            var mjesta = new List<int> { 5 };
            var dto = ValidanDto();
            dto.KategorijeID = kategorije;
            dto.MjestaID = mjesta;

            // Akcija i provjera
            Assert.AreEqual("majstor1", dto.KorisnickoIme);
            Assert.AreEqual("Milan", dto.Ime);
            Assert.AreEqual("Jovanovic", dto.Prezime);
            Assert.AreEqual("majstor@example.com", dto.Email);
            Assert.AreEqual("Lozinka123!", dto.Lozinka);
            Assert.AreEqual("Lozinka123!", dto.PotvrdaLozinke);
            CollectionAssert.AreEqual(kategorije, dto.KategorijeID);
            CollectionAssert.AreEqual(mjesta, dto.MjestaID);
        }

        [DataTestMethod]
        [DataRow("Lozinka123!", true)]
        [DataRow("lozinka1", true)]
        [DataRow("12345678", true)]
        [DataRow("lozinka", false)]
        [DataRow("lozinkaa", false)]
        [DataRow("loz1", false)]
        public void Lozinka_PostujePraviloOsamZnakovaICifre(string lozinka, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.Lozinka = lozinka;
            dto.PotvrdaLozinke = lozinka;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NeispravanEmail_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Email = "majstor.example.com";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaMajstoraDto.Email)));
        }

        [TestMethod]
        public void PotvrdaLozinkeSeNePoklapa_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.PotvrdaLozinke = "NestoDrugo123!";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaMajstoraDto.PotvrdaLozinke)));
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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaMajstoraDto.KategorijeID)));
        }

        [TestMethod]
        public void NedostajeListaMjesta_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.MjestaID = null!;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaMajstoraDto.MjestaID)));
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
    }

    [TestClass]
    public class RegistracijaFirmeDtoTests
    {
        private static RegistracijaFirmeDto ValidanDto() => new()
        {
            KorisnickoIme = "firma1",
            NazivFirme = "Popravka d.o.o.",
            Email = "firma@example.com",
            Lozinka = "Lozinka123!",
            PotvrdaLozinke = "Lozinka123!",
            VelicinaFirme = VelicinaFirme.Mala,
            WebStranica = "https://popravka.ba",
            KategorijeID = new List<int> { 1 },
            MjestaID = new List<int> { 1 }
        };

        [TestMethod]
        public void NoviDto_ImaZadanuVelicinuMikro()
        {
            // Akcija
            var dto = new RegistracijaFirmeDto();

            // Provjera
            Assert.AreEqual(VelicinaFirme.Mikro, dto.VelicinaFirme);
            Assert.IsNull(dto.PotvrdaLozinke);
            Assert.IsNull(dto.WebStranica);
            Assert.IsNull(dto.KategorijeID);
            Assert.IsNull(dto.MjestaID);
        }

        [TestMethod]
        public void PostavljeneVrijednosti_SeVracaju()
        {
            // Priprema
            var kategorije = new List<int> { 4, 9 };
            var mjesta = new List<int> { 2 };
            var dto = ValidanDto();
            dto.KategorijeID = kategorije;
            dto.MjestaID = mjesta;
            dto.VelicinaFirme = VelicinaFirme.Velika;

            // Akcija i provjera
            Assert.AreEqual("firma1", dto.KorisnickoIme);
            Assert.AreEqual("Popravka d.o.o.", dto.NazivFirme);
            Assert.AreEqual("firma@example.com", dto.Email);
            Assert.AreEqual("Lozinka123!", dto.Lozinka);
            Assert.AreEqual("Lozinka123!", dto.PotvrdaLozinke);
            Assert.AreEqual(VelicinaFirme.Velika, dto.VelicinaFirme);
            Assert.AreEqual("https://popravka.ba", dto.WebStranica);
            CollectionAssert.AreEqual(kategorije, dto.KategorijeID);
            CollectionAssert.AreEqual(mjesta, dto.MjestaID);
        }

        [DataTestMethod]
        [DataRow("https://popravka.ba", true)]
        [DataRow("nije url", false)]
        [DataRow(null, true)]
        public void WebStranica_PostujeUrlPravilo(string? webStranica, bool ocekivanoValidan)
        {
            // Priprema
            var dto = ValidanDto();
            dto.WebStranica = webStranica;

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.AreEqual(ocekivanoValidan, rezultati.Count == 0);
        }

        [TestMethod]
        public void NeispravanEmail_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.Email = "firma@";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaFirmeDto.Email)));
        }

        [TestMethod]
        public void PotvrdaLozinkeSeNePoklapa_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.PotvrdaLozinke = "NestoDrugo123!";

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaFirmeDto.PotvrdaLozinke)));
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
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaFirmeDto.KategorijeID)));
        }

        [TestMethod]
        public void PraznaListaMjesta_VracaGresku()
        {
            // Priprema
            var dto = ValidanDto();
            dto.MjestaID = new List<int>();

            // Akcija
            var rezultati = DtoValidacija.Validiraj(dto);

            // Provjera
            Assert.IsTrue(DtoValidacija.ImaGreskuZa(rezultati, nameof(RegistracijaFirmeDto.MjestaID)));
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
    }
}
