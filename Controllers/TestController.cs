
using Microsoft.AspNetCore.Mvc;

namespace SimpleSocialFeed
{
	[ApiController]
	[Route("api/test")]
	public class TestController : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult> Test()
		{
			return Ok("test");
		}
	}

}