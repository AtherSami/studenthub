using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
	public class PostCommentModel
	{
		[Key]
		public int PostCommentId { get; set; }	
		public string? Body { get; set; }	
		public string? Reaction { get; set; }
		public string? CreateBy { get; set; }
		public DateTime? CreateDate { get; set; }
		public string? UpdateBy { get; set; }
		public DateTime? UpdateDate { get; set; }
        public int? UserID { get; set; }
		public string? FullName { get; set; }
		public string? Attachment { get; set; }

		//public UserModel? User { get; set; }
		public int? PostId { get; set; }
		public PostModel? Post { get; set; }
		//public string BlogID { get; set; }
	}
}
