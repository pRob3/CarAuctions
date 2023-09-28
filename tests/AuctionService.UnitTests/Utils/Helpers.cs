using System.Security.Claims;

namespace AuctionService.UnitTests;

public class Helpers
{
  public static ClaimsPrincipal GetClaimsPrincipal()
  {
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
    var identity = new ClaimsIdentity(claims, "TestAuthType");

    return new ClaimsPrincipal(identity);
  }
}
