using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using appointmentSystem.Controllers.Features.Clients;
using appointmentSystem.Data;
using appointmentSystem.Models;
using appointmentSystem.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace appointmentSystem.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public AuthController(IConfiguration configuration, AppDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] ClientViewModel client)
    {
        var validatedClient = _dbContext.Clients.FirstOrDefault(c =>
            c.Id == client.Id && c.Name == client.Name && c.Phone == client.Phone);

        if (validatedClient != null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new("Id", validatedClient.Id.ToString()),
                new("Name", validatedClient.Name),
                new("Phone", validatedClient.Phone)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        return Unauthorized();
    }
}