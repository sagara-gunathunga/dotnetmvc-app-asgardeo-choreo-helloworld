using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;


namespace HelloworldSecure.Controllers;
public class IdentityController : Controller
{
    
    // public IActionResult Index()
    // {
    //     return View();
    // }

    [HttpGet]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

}


