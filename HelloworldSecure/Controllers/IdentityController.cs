using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;


namespace HelloworldSecure.Controllers;
public class IdentityController : Controller
{

    [HttpGet]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var idToken = await HttpContext.GetTokenAsync("id_token");
        string appOrigin = Url.Action("Index", "Home", null, Request.Scheme)!;
        string redirectURL = $"{appOrigin.Remove(appOrigin.Length - 1, 1)}";
        Console.WriteLine($"redirectURL: {redirectURL}");
        Console.WriteLine($"idToken: {idToken}");

       return Redirect($"https://api.asgardeo.io/t/sagaraorg/oidc/logout?id_token_hint={idToken}&post_logout_redirect_uri={redirectURL}");
    }



}


