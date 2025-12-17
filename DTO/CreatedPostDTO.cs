using System.ComponentModel.DataAnnotations;

namespace SimpleSocialFeed
{
	public class CreatePostDto
	{
		[Required]
		public string Title { get; set; }

		public string? Describtion { get; set; }
	}

}