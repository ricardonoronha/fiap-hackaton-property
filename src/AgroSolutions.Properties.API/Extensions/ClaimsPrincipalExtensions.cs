using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        var email = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedAccessException("Claim 'sub' não encontrada.");

        return email;
    }
}