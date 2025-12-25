using System.ComponentModel.DataAnnotations;

namespace SimpleSocialFeed
{
	public class PostDTO
	{
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string? Describtion { get; set; }

		public DateTime DateCreated { get; set; }

		public string Username{ get; set; }
	}
}