using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SimpleSocialFeed
{
	[ApiController]
	[Route("api/post")]
	public class PostController : ControllerBase
	{
		public readonly SimpleSocialFeedContext _db;
		public readonly IConfiguration _config;

		public PostController(IConfiguration config, SimpleSocialFeedContext db)
		{
			_db = db;
			_config = config;
		}


		[HttpGet]
		public async Task<IActionResult> GetAllPost()
		{
			var Posts = await _db.Posts.ToListAsync() ;
			if(Posts.Count == 0)
				return NotFound("Not found");
			return Ok(Posts);
		}

		[HttpGet("page")]
		public async Task<IActionResult> GetPosts(int skip , int take)
		{
			var posts = await _db.Posts.OrderBy(p => p.Id).Skip(skip).Take(take).ToArrayAsync();

			return Ok(posts);
		}


		// [HttpGet("/posts?skip={skip}&take={take}")]


		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreatedPost([FromBody] CreatePostDto request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userIdClaim =  User.FindFirst("user_id");
			if(userIdClaim is null)
				return Unauthorized("Please Login!!");
			int userId = int.Parse(userIdClaim.Value);
			var post = new Post
			{
				Title = request.Title,
				Describtion = request.Describtion,
				UserId = userId,
			};
			_db.Posts.Add(post);
			await _db.SaveChangesAsync();

			return Ok( new { success = "success" });
		}
		
	}
}