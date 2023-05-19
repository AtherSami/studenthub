using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialProject.Models
{
	public class BlogModel
	{
		[Key]
		public int BlogId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Location { get; set; }
		public string? Attachment { get; set; }
		public string? CreateBy { get; set; }
		public DateTime?  CreateDate { get; set; }
		public string? UpdateBy { get; set; }
		public DateTime?  UpdateDate { get; set; }
		public string? Status { get; set; }
		public int? UserID { get; set; }
		public String? FullName { get; set; }

		[NotMapped]
		public List<IFormFile>? Attachments { get; set; }
		

		//public int UserCommentId { get; set; }
        public ICollection<UserCommentModel>? UserComment { get; set; }
    }
}
