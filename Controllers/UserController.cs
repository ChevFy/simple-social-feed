
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


		[HttpPost("post")]
		[Authorize]
		public async Task<IActionResult> CreatedPost([FromBody] CreatePostDto request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userIdClaim = User.FindFirst("user_id");
			if (userIdClaim is null)
				return Unauthorized("Please Login!!");
			int userId = int.Parse(userIdClaim.Value);
			var post = new Post
			{
				Title = request.Title,
				Describtion = request.Describtion,
				UserId = userId,
				DateCreated = DateTime.UtcNow
			};
			_db.Posts.Add(post);
			await _db.SaveChangesAsync();

			return Ok(new { success = "success" });
		}

		[HttpGet("posts")]
		[Authorize]
		public async Task<IActionResult> GetUserPosts()
		{
			var userIdClaim = User.FindFirst("user_id");
			if(userIdClaim is null)
				return Unauthorized("Please Login!!");

			int userId = int.Parse(userIdClaim.Value);
			var user = await _db.Users.FindAsync(userId);
			var posts = await _db.Posts.Where(p => p.UserId == userId).ToListAsync();

			var postsDTO = posts.Select(p => new PostDTO
			{
				Id = p.Id,
				Title = p.Title,
				Describtion = p.Describtion,
				Username = user.Username,
				DateCreated = p.DateCreated
			}).ToList();

			return Ok(postsDTO);

		}

		[HttpDelete("post")]
		[Authorize]
		public async Task<IActionResult> DeletePostById(int id)
		{
			var userIdClaim = User.FindFirst("user_id");
			if(userIdClaim is null)
				return Unauthorized("Please Login!!");

			var post = await _db.Posts.FindAsync(id);
			if( post is null)
				return NotFound();
			
			_db.Posts.Remove(post);
			await _db.SaveChangesAsync();
			
			return Ok("remove success");
		}

	}
	
}