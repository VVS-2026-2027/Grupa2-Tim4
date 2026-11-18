using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using PopravkaBa.Domain.Enums;
using PopravkaBa.Domain.Models;

namespace PopravkaBa.Tests.Helpers
{
    // Zajednički zamjenski objekti (Moq) za Identity i kontrolere.
    public static class MockHelper
    {
        public static Mock<UserManager<ApplicationUser>> KreirajUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        public static Mock<SignInManager<ApplicationUser>> KreirajSignInManager(
            UserManager<ApplicationUser> userManager)
        {
            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                null!,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);
        }

        public static ClaimsPrincipal KreirajKorisnika(string id, KorisnickeUloge uloga)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, $"{id}@test.ba"),
                new Claim(ClaimTypes.Role, uloga.ToString())
            };
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }

        public static ClaimsPrincipal KreirajAnonimnogKorisnika() => new(new ClaimsIdentity());

        public static T PostaviKontekst<T>(T kontroler, ClaimsPrincipal? korisnik = null) where T : Controller
        {
            var httpContext = new DefaultHttpContext { User = korisnik ?? KreirajAnonimnogKorisnika() };
            kontroler.ControllerContext = new ControllerContext { HttpContext = httpContext };
            kontroler.TempData = new TempDataDictionary(httpContext, new Mock<ITempDataProvider>().Object);
            return kontroler;
        }
    }
}
