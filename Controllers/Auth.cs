using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using netcore_rest_api.Configuration;
using netcore_rest_api.Models.DTOs.Requests;
using netcore_rest_api.Models.DTOs.Responses;

namespace netcore_rest_api.Controllers {

  [ApiController]
  [Route("api/v1/[controller]")] // api/v1/Auth
  public class Auth : ControllerBase {
    private readonly UserManager<IdentityUser> userManager;
    private readonly JwtConfig jwtConfig;
    private readonly AuthResponse errorResponse = new AuthResponse {
      Errors = new List<string> { "Invalid request" }
    };

    public Auth(UserManager<IdentityUser> manager, IOptionsMonitor<JwtConfig> options) {
      userManager = manager;
      jwtConfig = options.CurrentValue;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequest user) {
      if (!ModelState.IsValid) return BadRequest(errorResponse);

      var existing = await userManager.FindByEmailAsync(user.Email);
      if (existing != null) return BadRequest(errorResponse);

      var newUser = new IdentityUser { Email = user.Email, UserName = user.Email };
      var result = await userManager.CreateAsync(newUser, user.Password);
      if (!result.Succeeded) return BadRequest(new AuthResponse {
        Errors = result.Errors.Select(e => e.Description).ToList(),
      });

      return Ok(new AuthResponse { Token = GenerateJwtToken(newUser) });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest user) {
      if (!ModelState.IsValid) return BadRequest(errorResponse);

      var existing = await userManager.FindByEmailAsync(user.Email);
      if (existing == null) return BadRequest(errorResponse);
      if (!(await userManager.CheckPasswordAsync(existing, user.Password))) return BadRequest(errorResponse);

      return Ok(new AuthResponse { Token = GenerateJwtToken(existing) });
    }

    private string GenerateJwtToken(IdentityUser user) {
      var handler = new JwtSecurityTokenHandler();
      return handler.WriteToken(handler.CreateToken(new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(new[] {
              new Claim("id", user.Id),
              new Claim(JwtRegisteredClaimNames.Email, user.Email),
              new Claim(JwtRegisteredClaimNames.Sub, user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // used to support refreshing an expired token
          }),
        Expires = DateTime.UtcNow.AddHours(6),
        SigningCredentials = new SigningCredentials(
              new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Secret)),
              SecurityAlgorithms.HmacSha256Signature)
      }));
    }
  }
}