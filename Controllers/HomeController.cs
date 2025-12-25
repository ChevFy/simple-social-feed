using Microsoft.AspNetCore.Mvc;

namespace SimpleSocialFeed
{
	public class HomeController : Controller
	{
		[HttpGet("/")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("/login")]
		public IActionResult Login()
		{
			return View();
		}

		[HttpGet("/register")]
		public IActionResult Register()
		{
			return View();
		}

		[HttpGet("/create-post")]
		public IActionResult CreatePost()
		{
			return View();
		}

		[HttpGet("/posts")]
		public IActionResult Posts()
		{
			return View();
		}

		[HttpGet("/my-posts")]
		public IActionResult MyPosts()
		{
			return View();
		}
	}
}
