using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WADProject1.Models;
using WADProject1.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TenderContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(TenderContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
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
            EmailConfirmationToken = Guid.NewGuid().ToString(),
            EmailConfirmed = false
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // 認証メールを送信
        var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { token = newUser.EmailConfirmationToken }, protocol: HttpContext.Request.Scheme);
        await _emailService.SendEmailAsync(signUpModel.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        return CreatedAtAction(nameof(Login), new { id = newUser.UserId }, new { newUser.Email });
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        if (user == null)
        {
            return NotFound("A user with this token does not exist.");
        }

        user.EmailConfirmed = true;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok("Email confirmed successfully.");
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
