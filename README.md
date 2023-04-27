# dotnetmvc-app-asgardeo-choreo-helloworld

Step 1 - Setting up the HelloWrold .NET Core MVC web application. 

dotnet new mvc


https://localhost:7139


Callback URL - https://localhost:7139/signin-oidc



Steps 2 - Adding a Secure page 

dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect


HomeController.cs

using Microsoft.AspNetCore.Authorization;


     [Authorize]
    public IActionResult Secure()
    {
        return View();
    }


Secure.cshtml

@{
    ViewData["Title"] = "Secure";
}
<h1>@ViewData["Title"]</h1>

<div class="text-center">
    <h1>This is a secure page!</h1>
    <p>Only authenticated users can view this page</p>
</div>



 _Layout.cshtml
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Secure">Secure</a>
                        </li> 

Program.cs

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;


// Set cookies to store the authentication information and “oidc” as the authentication provider.
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.Cookie.Name = "oidc";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
})
.AddOpenIdConnect(options => 
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ClientId = "8zOQefev4kZR6esq7VKbna2zEw4a";           
    options.ClientSecret = "RaM1mlUTcGHeKGXWlvGiXaxMNika";       
    options.Authority = "https://api.asgardeo.io/t/sagaraorg/oauth2/token";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.ResponseMode = OpenIdConnectResponseMode.Query;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("openid");
    options.Scope.Add("email");
    options.Scope.Add("profile");
});



app.UseAuthorization();



Steps 3 - Adding login button ( Not a login page)



 _Layout.cshtml


 <ul class="navbar-nav">
                        @if (User.Identity.IsAuthenticated == true)
                        {
                            <li class="nav-item">
                                <a class="nav-link text-dark" title="Manage">@User.Claims.Where(claim =>claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname" ).First().Value</a>
                            </li>
                        }
                        else
                        {
                            <li class="nav-item">
                                <a class="nav-link text-dark" asp-controller="Identity" asp-action="Login">Login</a>
                            </li>
                        }
                        </ul>    


                        IdentityController.cs

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

    

}  


Step 4 - Adding logout button 

 _Layout.cshtml

<ul class="navbar-nav">
                        @if (User?.Identity?.IsAuthenticated == true)
                        {
                            <li class="nav-item">
                                <a class="nav-link text-dark" title="Manage">@User.Claims.Where(claim =>claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname" ).First().Value</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link text-dark" asp-area="" asp-controller="Identity" asp-action="Logout">Logout</a>
                            </li>
                        }
                        else
                        {
                            <li class="nav-item">
                                <a class="nav-link text-dark" asp-controller="Identity" asp-action="Login">Login</a>
                            </li>
                        }
</ul>



using Microsoft.AspNetCore.Authentication.Cookies;
 
[HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var idToken = await HttpContext.GetTokenAsync("id_token");
        string appOrigin = Url.Action("Index", "Home", null, Request.Scheme)!;
        string redirectURL = $"{appOrigin.Remove(appOrigin.Length - 1, 1)}";

        return Redirect($"https://api.asgardeo.io/t/sagaraorg/oidc/logout?id_token_hint={idToken}&post_logout_redirect_uri={redirectURL}");
    } 


Steps 5 - Claims and tokens 

dotnet add package Newtonsoft.Json


IDModel.cs

using System;

namespace HelloworldSecure.Models;
public class IDModel
{
    public IEnumerable<System.Security.Claims.Claim>? Claims { get; set; }
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfileURL { get; set; }
}






<div>
    <h4>Profile Pic</h4>
    <img src=@Model.ProfileURL class="profile-pic"/>
    <h4>Claims</h4>
    <ul>
        @foreach (var claim in Model.Claims)
        {
            <li><b>@claim.Type</b> <span>@claim.Value</span></li>

        }
    </ul>
    <h4>Access Token</h4>
    <p class="field">@Model.AccessToken</p>
    <h4>ID Token</h4>
    <p class="field">@Model.IdToken</p>
    <h4>Refresh Token</h4>
    <p class="field">@Model.RefreshToken</p>
</div>



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



Improvemnts 

1. https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
2. Injectiing configuration paramters 
3. CSS to align the menu 

