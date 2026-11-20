using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using PopravkaBa.Infrastructure.Adapters;
using PopravkaBa.Infrastructure.Adapters.Options;

namespace PopravkaBa.Tests.Unit.Infrastructure.Adapters
{
    [TestClass]
    public class S3FileStorageAdapterTests
    {
        private const string PublicBucket = "test-public";
        private const string PrivateBucket = "test-private";
        private const string PublicBaseUrl = "https://cdn.popravka.test";

        private static readonly string CsvPutanja =
            Path.Combine(AppContext.BaseDirectory, "TestData", "s3_ekstenzije.csv");

        private Mock<IAmazonS3> _s3 = null!;
        private S3FileStorageAdapter _adapter = null!;
        private PutObjectRequest? _putZahtjev;
        private DeleteObjectRequest? _deleteZahtjev;
        private GetPreSignedUrlRequest? _signedZahtjev;

        [TestInitialize]
        public void Inicijalizacija()
        {
            _putZahtjev = null;
            _deleteZahtjev = null;
            _signedZahtjev = null;

            _s3 = new Mock<IAmazonS3>();
            _s3.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
               .Callback<PutObjectRequest, CancellationToken>((zahtjev, _) => _putZahtjev = zahtjev)
               .ReturnsAsync(new PutObjectResponse());
            _s3.Setup(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
               .Callback<DeleteObjectRequest, CancellationToken>((zahtjev, _) => _deleteZahtjev = zahtjev)
               .ReturnsAsync(new DeleteObjectResponse());
            _s3.Setup(s => s.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
               .Callback<GetPreSignedUrlRequest>(zahtjev => _signedZahtjev = zahtjev)
               .Returns("https://potpisani.link.test/prilog");

            _adapter = KreirajAdapter(PublicBaseUrl);
        }

        private S3FileStorageAdapter KreirajAdapter(string publicBaseUrl) =>
            new(_s3.Object, Options.Create(new R2Options
            {
                PublicBucket = PublicBucket,
                PrivateBucket = PrivateBucket,
                PublicBaseURL = publicBaseUrl
            }));

        // Eksterni izvor podataka (CSV)
        public static IEnumerable<object[]> IzvuciIzCsv() =>
            File.ReadLines(CsvPutanja)
                .Skip(1)
                .Where(red => !string.IsNullOrWhiteSpace(red))
                .Select(red => red.Split(','))
                .Select(k => new object[] { k[0], k[1] });

        #region Extension

        [TestMethod]
        [DynamicData(nameof(IzvuciIzCsv), DynamicDataSourceType.Method)]
        public void Extension_PodaciIzCsv_VracaOcekivanuEkstenziju(string contentType, string ocekivano)
        {
            var ekstenzija = S3FileStorageAdapter.Extension(contentType);

            Assert.AreEqual(ocekivano, ekstenzija);
        }

        [TestMethod]
        public void Extension_ContentTypeNull_VracaPrazanString()
        {
            var ekstenzija = S3FileStorageAdapter.Extension(null!);

            Assert.AreEqual(string.Empty, ekstenzija);
        }

        #endregion

        #region SpremiPublic

        [TestMethod]
        public async Task SpremiPublic_Slika_SpremaUJavniBucketSaIspravnimPodacima()
        {
            using var sadrzaj = new MemoryStream([1, 2, 3]);

            await _adapter.SpremiPublic(sadrzaj, "image/png");

            Assert.IsNotNull(_putZahtjev);
            Assert.AreEqual(PublicBucket, _putZahtjev.BucketName);
            Assert.AreEqual("image/png", _putZahtjev.ContentType);
            Assert.AreSame(sadrzaj, _putZahtjev.InputStream);
            Assert.AreEqual(true, _putZahtjev.DisablePayloadSigning);
        }

        [TestMethod]
        public async Task SpremiPublic_Jpeg_KljucJeUFolderuSlikeSaGuidomIEkstenzijom()
        {
            using var sadrzaj = new MemoryStream();

            await _adapter.SpremiPublic(sadrzaj, "image/jpeg");

            StringAssert.Matches(_putZahtjev!.Key, new System.Text.RegularExpressions.Regex(@"^slike/[0-9a-f]{32}\.jpg$"));
        }

        [TestMethod]
        public async Task SpremiPublic_NepoznatTip_KljucNemaEkstenziju()
        {
            using var sadrzaj = new MemoryStream();

            await _adapter.SpremiPublic(sadrzaj, "text/plain");

            StringAssert.Matches(_putZahtjev!.Key, new System.Text.RegularExpressions.Regex(@"^slike/[0-9a-f]{32}$"));
        }

        [TestMethod]
        public async Task SpremiPublic_Slika_VracaJavniUrlSaKljucem()
        {
            using var sadrzaj = new MemoryStream();

            var url = await _adapter.SpremiPublic(sadrzaj, "image/webp");

            Assert.AreEqual($"{PublicBaseUrl}/{_putZahtjev!.Key}", url);
        }

        [TestMethod]
        public async Task SpremiPublic_DvaSpremanja_GenerisuRazliciteKljuceve()
        {
            using var sadrzaj = new MemoryStream();

            var prviUrl = await _adapter.SpremiPublic(sadrzaj, "image/png");
            var drugiUrl = await _adapter.SpremiPublic(sadrzaj, "image/png");

            Assert.AreNotEqual(prviUrl, drugiUrl);
        }

        [TestMethod]
        public async Task SpremiPublic_ProslijedjenToken_ProsljedjujeGaS3Klijentu()
        {
            using var sadrzaj = new MemoryStream();
            using var izvor = new CancellationTokenSource();

            await _adapter.SpremiPublic(sadrzaj, "image/png", izvor.Token);

            _s3.Verify(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), izvor.Token), Times.Once);
        }

        [TestMethod]
        public async Task SpremiPublic_S3BacaGresku_GreskaSeProsljedjuje()
        {
            _s3.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new AmazonS3Exception("Bucket ne postoji"));
            using var sadrzaj = new MemoryStream();

            await Assert.ThrowsExceptionAsync<AmazonS3Exception>(
                () => _adapter.SpremiPublic(sadrzaj, "image/png"));
        }

        #endregion

        #region SpremiPrivate

        [TestMethod]
        public async Task SpremiPrivate_Dokument_SpremaUPrivatniBucketSaIspravnimPodacima()
        {
            using var sadrzaj = new MemoryStream([1, 2, 3]);

            await _adapter.SpremiPrivate(sadrzaj, "rjesenje.pdf", "application/pdf");

            Assert.IsNotNull(_putZahtjev);
            Assert.AreEqual(PrivateBucket, _putZahtjev.BucketName);
            Assert.AreEqual("application/pdf", _putZahtjev.ContentType);
            Assert.AreSame(sadrzaj, _putZahtjev.InputStream);
            Assert.AreEqual(true, _putZahtjev.DisablePayloadSigning);
        }

        [TestMethod]
        public async Task SpremiPrivate_Dokument_VracaKljucUFolderuPriloziSaEkstenzijomFajla()
        {
            using var sadrzaj = new MemoryStream();

            var kljuc = await _adapter.SpremiPrivate(sadrzaj, "uvjerenje.docx", "application/octet-stream");

            StringAssert.Matches(kljuc, new System.Text.RegularExpressions.Regex(@"^prilozi/[0-9a-f]{32}\.docx$"));
            Assert.AreEqual(kljuc, _putZahtjev!.Key);
        }

        [TestMethod]
        public async Task SpremiPrivate_FajlBezEkstenzije_KljucNemaEkstenziju()
        {
            using var sadrzaj = new MemoryStream();

            var kljuc = await _adapter.SpremiPrivate(sadrzaj, "dokument", "application/pdf");

            StringAssert.Matches(kljuc, new System.Text.RegularExpressions.Regex(@"^prilozi/[0-9a-f]{32}$"));
        }

        #endregion

        #region GetSignedURL

        [TestMethod]
        public void GetSignedURL_Kljuc_VracaUrlKojiVratiS3()
        {
            var url = _adapter.GetSignedURL("prilozi/abc.pdf", TimeSpan.FromMinutes(15));

            Assert.AreEqual("https://potpisani.link.test/prilog", url);
        }

        [TestMethod]
        public void GetSignedURL_Kljuc_TraziGetHttpsLinkIzPrivatnogBucketa()
        {
            _adapter.GetSignedURL("prilozi/abc.pdf", TimeSpan.FromMinutes(15));

            Assert.IsNotNull(_signedZahtjev);
            Assert.AreEqual(PrivateBucket, _signedZahtjev.BucketName);
            Assert.AreEqual("prilozi/abc.pdf", _signedZahtjev.Key);
            Assert.AreEqual(HttpVerb.GET, _signedZahtjev.Verb);
            Assert.AreEqual(Protocol.HTTPS, _signedZahtjev.Protocol);
        }

        [TestMethod]
        public void GetSignedURL_Trajanje_LinkIsticeNakonZadanogVremena()
        {
            var trajanje = TimeSpan.FromHours(1);
            var prije = DateTime.UtcNow;

            _adapter.GetSignedURL("prilozi/abc.pdf", trajanje);

            var poslije = DateTime.UtcNow;
            Assert.IsTrue(_signedZahtjev!.Expires >= prije.Add(trajanje) && _signedZahtjev.Expires <= poslije.Add(trajanje));
        }

        #endregion

        #region ObrisiPublic / ObrisiPrivate

        [TestMethod]
        [DataRow("https://cdn.popravka.test", DisplayName = "Bazni URL bez kose crte")]
        [DataRow("https://cdn.popravka.test/", DisplayName = "Bazni URL sa kosom crtom")]
        public async Task ObrisiPublic_JavniUrl_BriseKljucIzJavnogBucketa(string publicBaseUrl)
        {
            var adapter = KreirajAdapter(publicBaseUrl);

            await adapter.ObrisiPublic("https://cdn.popravka.test/slike/abc123.jpg");

            Assert.IsNotNull(_deleteZahtjev);
            Assert.AreEqual(PublicBucket, _deleteZahtjev.BucketName);
            Assert.AreEqual("slike/abc123.jpg", _deleteZahtjev.Key);
        }

        [TestMethod]
        public async Task ObrisiPrivate_Kljuc_BriseIstiKljucIzPrivatnogBucketa()
        {
            await _adapter.ObrisiPrivate("prilozi/abc123.pdf");

            Assert.IsNotNull(_deleteZahtjev);
            Assert.AreEqual(PrivateBucket, _deleteZahtjev.BucketName);
            Assert.AreEqual("prilozi/abc123.pdf", _deleteZahtjev.Key);
        }

        [TestMethod]
        public async Task ObrisiPrivate_ProslijedjenToken_ProsljedjujeGaS3Klijentu()
        {
            using var izvor = new CancellationTokenSource();

            await _adapter.ObrisiPrivate("prilozi/abc123.pdf", izvor.Token);

            _s3.Verify(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), izvor.Token), Times.Once);
        }

        #endregion
    }
}
