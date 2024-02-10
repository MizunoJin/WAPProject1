using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WADProject1.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TenderContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(TenderContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginModel login)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(x => x.Email == login.Email && x.Password == login.Password);

        if (user == null) return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    // サインアップ（ユーザー登録）機能
    [HttpPost("signup")]
    public async Task<ActionResult<User>> SignUp([FromBody] LoginModel signUpModel)
    {
        if (await _context.Users.AnyAsync(u => u.Email == signUpModel.Email))
        {
            return BadRequest("Email is already in use.");
        }

        var newUser = new User
        {
            Email = signUpModel.Email,
            Password = signUpModel.Password, // 実際のアプリではパスワードをハッシュ化するべき
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(newUser);

        return CreatedAtAction(nameof(Login), new { id = newUser.UserId }, new { Token = token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(1);

        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.UserId.ToString()),
            },
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
