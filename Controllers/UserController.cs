
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;

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

		// [HttpGet("/posts/")]
		// public async Task<IActionResult> GetAllPost()
		// {
			
		// }

	}
	
}