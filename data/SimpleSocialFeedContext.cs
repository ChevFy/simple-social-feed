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

		}
		public DbSet<User> Users { get; set; }
		public DbSet<Post> Posts { get; set; }

	}

}