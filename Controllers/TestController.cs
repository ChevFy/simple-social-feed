
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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