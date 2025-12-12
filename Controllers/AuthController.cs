using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace SimpleSocialFeed
{
	[ApiController]
	[Route("api/Auth")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly SimpleSocialFeedContext _db;
		private readonly PasswordService _passwordService;

		public AuthController(IConfiguration config, SimpleSocialFeedContext db , PasswordService passwordService)
		{
			_config = config;
			_db = db;
			_passwordService = passwordService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLogin request)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u=> u.Username == request.Username);
			if(user is null)
				return NotFound();
			if(!_passwordService.VerifyPassword(request.Password, user.passwordHash , user.passwordSalt))
			{
				Console.WriteLine(user.passwordHash);
				return BadRequest("Wrong password");
			}

			string token = GenerateJwtToken(user.Id);
			return Ok(new { token }); ;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserRegister request)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u=> u.Username == request.Username);
			if( user is not null)
				return BadRequest("This Username already exists");
			if (request.ConfirmPassword != request.Password)
				return BadRequest("Password doesn't match");
			
			_passwordService.CreatePasswordHash(request.Password,out byte[] hash , out byte[] salt);

			var new_user = new User
			{
				Username = request.Username,
				passwordHash = hash,
				passwordSalt = salt
			};

			_db.Users.Add(new_user);
			await _db.SaveChangesAsync();

			return Ok("Registered");
		}

		[HttpPut("changepassword")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] UserChangePassword request)
		{
			int userId = int.Parse(User.FindFirst("user_id")!.Value);
			var user = await _db.Users.FindAsync(userId);
			if(user is null)
				return NotFound();

			if(!_passwordService.VerifyPassword(request.OldPassword, user.passwordHash , user.passwordSalt))
				return BadRequest("Old password doesn't match!!");

			_passwordService.CreatePasswordHash(request.NewPassword , out byte[] newHash, out byte[] newSalt);

			User updatedUser = new User
			{
				Id = userId,
				Username = user.Username,
				passwordHash = newHash,
				passwordSalt = newSalt,
				Posts = user.Posts
			};


			_db.Entry(user).CurrentValues.SetValues(updatedUser);
			await _db.SaveChangesAsync();

			return Ok(new { sucesss = "Change Password Success" });
		}

		private string GenerateJwtToken(int UserId)
		{
			string Id = Convert.ToString(UserId);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, Id) ,
				new Claim("user_id", Id.ToString(), ClaimValueTypes.Integer),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["JWT:Issuer"],
				audience: _config["JWT:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}