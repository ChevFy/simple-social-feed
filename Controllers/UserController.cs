
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SimpleSocialFeed
{
	[ApiController]
	[Route("api/User")]
	public class UserController : ControllerBase
	{
		public readonly SimpleSocialFeedContext _db;
		public readonly IConfiguration _config;
		
		public UserController(IConfiguration config , SimpleSocialFeedContext db)
		{
			_db = db;
			_config = config;
		}

		[HttpGet("posts")]
		[Authorize]
		public async Task<IActionResult> GetUserPosts()
		{
			var userIdClaim = User.FindFirst("user_id");
			if(userIdClaim is null)
				return Unauthorized("Please Login!!");

			int userId = int.Parse(userIdClaim.Value);
			var posts = await _db.Posts.Where(p => p.UserId == userId).ToListAsync();

			return Ok(posts);

		}

	}
	
}