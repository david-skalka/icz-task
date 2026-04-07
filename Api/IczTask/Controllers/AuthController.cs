using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IczTask.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IczTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IOptions<JwtOptions> jwt) : ControllerBase
{
    private readonly JwtOptions _jwt = jwt.Value;

    // ✅ POST: /api/auth/login
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
    {
        if (req is not { Login: "host", Password: "icz" })
            return Unauthorized(new { message = "Invalid credentials" });


        var claims = new[]
        {
            new Claim(ClaimTypes.Name, req.Login),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
            signingCredentials: credentials);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token));
    }
}

public record LoginRequest(string Login, string Password);

public record LoginResponse(string Token);