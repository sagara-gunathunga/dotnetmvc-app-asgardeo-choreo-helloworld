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


