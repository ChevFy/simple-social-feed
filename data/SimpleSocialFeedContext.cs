using Microsoft.EntityFrameworkCore;

namespace SimpleSocialFeed
{
	public class SimpleSocialFeedContext : DbContext
	{
		public SimpleSocialFeedContext(DbContextOptions<SimpleSocialFeedContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<User>().HasMany(u => u.Posts).WithOne(p => p.User).HasForeignKey(p => p.UserId);
			// modelBuilder.Entity<User>().HasData(
			// 	new User { Id = 1 }
			// );

			// modelBuilder.Entity<Post>().HasData(
			// 	new Post
			// 	{
			// 		Id = 1,
			// 		Title = "Hello World",
			// 		Describetion = "My first post on this platform!",
			// 		UserId = 1
			// 	},
			// 	new Post
			// 	{
			// 		Id = 2,
			// 		Title = "Learning C#",
			// 		Describetion = "Today I learned about Entity Framework Core seeding.",
			// 		UserId = 1
			// 	},
			// 	new Post
			// 	{
			// 		Id = 3,
			// 		Title = "Weekend Trip",
			// 		Describetion = "Just came back from a great trip to Chiang Mai!",
			// 		UserId = 2
			// 	},
			// 	new Post
			// 	{
			// 		Id = 4,
			// 		Title = "Coffee Thoughts",
			// 		Describetion = "Working remotely from a café. Life is good.",
			// 		UserId = 2
			// 	},
			// 	new Post
			// 	{
			// 		Id = 5,
			// 		Title = "API Design Tips",
			// 		Describetion = "Keep endpoints simple and predictable.",
			// 		UserId = 3
			// 	},
			// 	new Post
			// 	{
			// 		Id = 6,
			// 		Title = "Clean Code",
			// 		Describetion = "Refactoring some old code… feels great!",
			// 		UserId = 3
			// 	});

		}
		public DbSet<User> Users { get; set; }
		public DbSet<Post> Posts { get; set; }

	}

}