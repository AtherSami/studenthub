using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
	public class UserCommentModel
	{
		[Key]
		public int UserCommentId { get; set; }
		public string? Body { get; set; }
		public string? Reaction { get; set; }
		public string? CreateBy { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? UpdateBy { get; set; }
		public DateTime? UpdateDate { get; set; }
		public int? UserId { get; set; }
		public string? FullName { get; set; }
		public string? Attachment { get; set; }

		
		public UserModel? User { get; set; }
		public int? BlogId { get; set; }
		public BlogModel? Blog { get; set; }
	}
}