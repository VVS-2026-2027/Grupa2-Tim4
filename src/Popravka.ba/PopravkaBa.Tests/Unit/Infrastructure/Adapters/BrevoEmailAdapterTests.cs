using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq.Protected;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;
using PopravkaBa.Infrastructure.Adapters;
using PopravkaBa.Infrastructure.Adapters.Options;

namespace PopravkaBa.Tests.Unit.Infrastructure.Adapters
{
    [TestClass]
    public class BrevoEmailAdapterTests
    {
        private const string Host = "smtp.test.local";
        private const int Port = 2525;
        private const string Login = "test-login";
        private const string SmtpKljuc = "test-kljuc";
        private const string PosiljalacEmail = "noreply@popravka.test";
        private const string PosiljalacIme = "Popravka.ba";

        private Mock<ISmtpClient> _smtp = null!;
        private BrevoEmailAdapter _adapter = null!;
        private MimeMessage? _poslanaPoruka;

        [TestInitialize]
        public void Inicijalizacija()
        {
            _poslanaPoruka = null;
            _smtp = new Mock<ISmtpClient>();
            _smtp.Setup(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                 .Callback<MimeMessage, CancellationToken, ITransferProgress>((poruka, _, _) => _poslanaPoruka = poruka)
                 .ReturnsAsync("OK");

            // Parcijalni mock: stvarni kod adaptera se izvršava, samo se SMTP klijent zamjenjuje
            var adapterMock = new Mock<BrevoEmailAdapter>(KreirajOpcije());
            adapterMock.Protected()
                       .Setup<ISmtpClient>("KreirajSmtpKlijent")
                       .Returns(_smtp.Object);
            _adapter = adapterMock.Object;
        }

        private static IOptions<BrevoEmailOptions> KreirajOpcije() => Options.Create(new BrevoEmailOptions
        {
            Host = Host,
            Port = Port,
            Login = Login,
            SmtpKey = SmtpKljuc,
            FromEmail = PosiljalacEmail,
            FromName = PosiljalacIme
        });

        private static EmailVerifikacijaFirme KreirajVerifikaciju() => new()
        {
            NazivFirme = "Test Firma d.o.o.",
            AdminEmail = "admin@popravka.test",
            Firma = new Firma { Email = "firma@popravka.test", NazivFirme = "Test Firma d.o.o." },
            StatusVerifikacije = Status.Prihvaceno,
            DatumObrade = new DateTime(2026, 11, 20)
        };

        private sealed class IzlozeniBrevoAdapter(IOptions<BrevoEmailOptions> opcije) : BrevoEmailAdapter(opcije)
        {
            public ISmtpClient Kreiraj() => KreirajSmtpKlijent();
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_PovezujeSeNaHostIPortIzOpcija()
        {
            await _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>");

            _smtp.Verify(s => s.ConnectAsync(Host, Port, SecureSocketOptions.StartTls, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_AutentificiraSeLoginomIKljucem()
        {
            await _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>");

            _smtp.Verify(s => s.AuthenticateAsync(Login, SmtpKljuc, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_PosiljalacJeIzOpcija()
        {
            await _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>");

            var posiljalac = _poslanaPoruka!.From.Mailboxes.Single();
            Assert.AreEqual(PosiljalacIme, posiljalac.Name);
            Assert.AreEqual(PosiljalacEmail, posiljalac.Address);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_PorukaImaPrimaocaSubjektIHtmlTijelo()
        {
            await _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt poruke", "<p>Tijelo</p>");

            Assert.IsNotNull(_poslanaPoruka);
            Assert.AreEqual("korisnik@test.ba", _poslanaPoruka.To.Mailboxes.Single().Address);
            Assert.AreEqual("Subjekt poruke", _poslanaPoruka.Subject);
            Assert.AreEqual("<p>Tijelo</p>", _poslanaPoruka.HtmlBody);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_ZatvaraVezuIOslobadjaKlijenta()
        {
            await _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>");

            _smtp.Verify(s => s.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
            _smtp.Verify(s => s.Dispose(), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_IspravniPodaci_PozivaSmtpKorakeIspravnimRedoslijedom()
        {
            var strogiSmtp = new Mock<ISmtpClient>(MockBehavior.Strict);
            var redoslijed = new MockSequence();
            strogiSmtp.InSequence(redoslijed).Setup(s => s.ConnectAsync(Host, Port, SecureSocketOptions.StartTls, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            strogiSmtp.InSequence(redoslijed).Setup(s => s.AuthenticateAsync(Login, SmtpKljuc, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            strogiSmtp.InSequence(redoslijed).Setup(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>())).ReturnsAsync("OK");
            strogiSmtp.InSequence(redoslijed).Setup(s => s.DisconnectAsync(true, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            strogiSmtp.Setup(s => s.Dispose());
            var adapterMock = new Mock<BrevoEmailAdapter>(KreirajOpcije());
            adapterMock.Protected().Setup<ISmtpClient>("KreirajSmtpKlijent").Returns(strogiSmtp.Object);

            await adapterMock.Object.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>");

            strogiSmtp.VerifyAll();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("korisnik@")]
        [DataRow("@test.ba")]
        [DataRow("korisnik@@test.ba")]
        public async Task PosaljiEmailAsync_NeispravanEmailPrimaoca_BacaParseException(string primalac)
        {
            await Assert.ThrowsExceptionAsync<ParseException>(
                () => _adapter.PosaljiEmailAsync(primalac, "Subjekt", "<p>Tijelo</p>"));
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_NeispravanEmailPrimaoca_NePovezujeSeNaServer()
        {
            await Assert.ThrowsExceptionAsync<ParseException>(
                () => _adapter.PosaljiEmailAsync("korisnik@", "Subjekt", "<p>Tijelo</p>"));

            _smtp.Verify(s => s.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_PrimalacNull_BacaArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _adapter.PosaljiEmailAsync(null!, "Subjekt", "<p>Tijelo</p>"));
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_NeuspjesnaAutentifikacija_BacaIzuzetakINeSaljePoruku()
        {
            _smtp.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new AuthenticationException("Pogrešni podaci za prijavu"));

            await Assert.ThrowsExceptionAsync<AuthenticationException>(
                () => _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>"));

            _smtp.Verify(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_GreskaPriSlanju_KlijentSeIpakOslobadja()
        {
            _smtp.Setup(s => s.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                 .ThrowsAsync(new ServiceNotConnectedException());

            await Assert.ThrowsExceptionAsync<ServiceNotConnectedException>(
                () => _adapter.PosaljiEmailAsync("korisnik@test.ba", "Subjekt", "<p>Tijelo</p>"));

            _smtp.Verify(s => s.Dispose(), Times.Once);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_Notifikacija_SaljeNaEmailPrimaocaSaFiksnimSubjektom()
        {
            var notifikacija = EmailNotifikacijaOglas.ZaBrisanjeZbogNeaktivnosti("vlasnik@test.ba", "Amar", "Popravka bojlera", 7);

            await _adapter.PosaljiEmailAsync(notifikacija);

            Assert.IsNotNull(_poslanaPoruka);
            Assert.AreEqual("vlasnik@test.ba", _poslanaPoruka.To.Mailboxes.Single().Address);
            Assert.AreEqual("[Popravka.ba] Obavijest o oglasu", _poslanaPoruka.Subject);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_Notifikacija_TijeloJeFormatiraniEmail()
        {
            var notifikacija = EmailNotifikacijaOglas.ZaBrisanjeZbogNeaktivnosti("vlasnik@test.ba", "Amar", "Popravka bojlera", 7);

            await _adapter.PosaljiEmailAsync(notifikacija);

            StringAssert.Contains(_poslanaPoruka!.HtmlBody, "Popravka bojlera");
            StringAssert.Contains(_poslanaPoruka.HtmlBody, "Amar");
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_VerifikacijaZaAdmina_SaljeAdminuSaNazivomFirmeUSubjektu()
        {
            var verifikacija = KreirajVerifikaciju();

            await _adapter.PosaljiEmailAsync(verifikacija, zaAdmina: true);

            Assert.AreEqual("admin@popravka.test", _poslanaPoruka!.To.Mailboxes.Single().Address);
            Assert.AreEqual("[Popravka.ba] Zahtjev za verifikaciju firme Test Firma d.o.o.", _poslanaPoruka.Subject);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_VerifikacijaZaFirmu_SaljeFirmiOdgovor()
        {
            var verifikacija = KreirajVerifikaciju();

            await _adapter.PosaljiEmailAsync(verifikacija, zaAdmina: false);

            Assert.AreEqual("firma@popravka.test", _poslanaPoruka!.To.Mailboxes.Single().Address);
            Assert.AreEqual("Odgovor: [Popravka.ba] Zahtjev za verifikaciju firme", _poslanaPoruka.Subject);
        }

        [TestMethod]
        public async Task PosaljiEmailAsync_VerifikacijaZaFirmu_TijeloSadrziIshodVerifikacije()
        {
            var verifikacija = KreirajVerifikaciju();

            await _adapter.PosaljiEmailAsync(verifikacija, zaAdmina: false);

            StringAssert.Contains(_poslanaPoruka!.HtmlBody, "Vaša firma je verificirana");
        }

        [TestMethod]
        public void KreirajSmtpKlijent_Podrazumijevano_VracaMailKitSmtpClient()
        {
            var adapter = new IzlozeniBrevoAdapter(KreirajOpcije());

            using var klijent = adapter.Kreiraj();

            Assert.IsInstanceOfType(klijent, typeof(SmtpClient));
        }
    }
}
