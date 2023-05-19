using SocialProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace SocialProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        
        {

        }
        public DbSet<UserModel> UserModels { get; set; }
       // public IEnumerable<object> Usermodel { get; internal set; }
		public DbSet<PostModel> PostModel{ get; set; }
		//public IEnumerable<object> PostModelss { get; internal set; }
		public DbSet<Admin> Admins { get; set; }
		public DbSet<PostCommentModel> Comments { get; set; }
		public DbSet<BlogModel> Blogs { get; set; }
		public DbSet<UserCommentModel> UserComments { get; set; }
        public DbSet<LikeModel> Likes { get; set; }
		public DbSet<NotificationModel> Notifications { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<BlogModel>()
				.HasMany(b => b.UserComment)
				.WithOne(c => c.Blog)
				.HasForeignKey(c => c.BlogId);



			modelBuilder.Entity<PostModel>()
				.HasMany(p => p.Comments)
				.WithOne(c => c.Post)
				.HasForeignKey(c => c.PostId);
			{
				modelBuilder.Entity<UserCommentModel>()
				.HasOne(c => c.User)
				.WithMany(u => u.Commentss)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			}

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<IdentityUserLogin<string>>()
				.HasKey(l => new { l.LoginProvider, l.ProviderKey });
		}
	}
}