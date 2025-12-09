

namespace SimpleSocialFeed
{
	public class Post
	{
		public int Id {get; set;}
		public string Title {get; set;}
		public string? Describetion {get; set;}

		public int UserId {get; set;}
		public User User {get; set;}
	}
	
}