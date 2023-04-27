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









