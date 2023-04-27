using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using HelloworldSecure.Models;

namespace HelloworldSecure.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Secure()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        Console.WriteLine("accessToken ", accessToken);

        var idToken = await HttpContext.GetTokenAsync("id_token");
        var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
        string displayName = User.Claims.Where(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname").First().Value;
        IEnumerable<System.Security.Claims.Claim> claims = User.Claims;

        // Getting the profile picture URL from the userinfo endpoint
        // to demonstrate how an API request can be dispatched to a
        // protected endpoint using the access token.
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        string userinfoEndpoint = $"https://api.asgardeo.io/t/sagaraorg/oauth2/userinfo";
        using var response = await httpClient.GetAsync(userinfoEndpoint);

        string profilePic = "https://img.freepik.com/free-psd/3d-illustration-person-with-sunglasses_23-2149436188.jpg";

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(content)!;

            profilePic = json.profile;
        }


        return View(new IDModel
        {
            Claims = claims,
            AccessToken = accessToken,
            DisplayName = displayName,
            IdToken = idToken,
            RefreshToken = refreshToken,
            ProfileURL = profilePic
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
