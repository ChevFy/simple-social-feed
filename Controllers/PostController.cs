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
			var posts = await _db.Posts
				.Include(p => p.User)
				.OrderByDescending(p => p.DateCreated)
				.Select(p => new PostDTO
				{
					Id = p.Id,
					Title = p.Title,
					Describtion = p.Describtion,
					DateCreated = p.DateCreated,
					Username = p.User != null ? p.User.Username : "Unknown"
				})
				.ToListAsync();

			if (posts.Count == 0)
				return NotFound("Not found");
			return Ok(posts);
		}

		[HttpGet("page")]
		public async Task<IActionResult> GetPosts(int skip , int take)
		{
			var posts = await _db.Posts
				.Include(p => p.User)
				.OrderByDescending(p => p.DateCreated)
				.Skip(skip)
				.Take(take)
				.Select(p => new PostDTO
				{
					Id = p.Id,
					Title = p.Title,
					Describtion = p.Describtion,
					DateCreated = p.DateCreated,
					Username = p.User != null ? p.User.Username : "Unknown"
				})
				.ToArrayAsync();

			return Ok(posts);
		}

		
	}
}