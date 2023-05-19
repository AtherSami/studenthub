using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
	public class EventModel
	{
		[Key]
		public int EventId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Location { get; set; }
		public string? Attachment { get; set; }
		public string? Duration { get; set; }
		public string? GuestEmail { get; set; }
		public string? CreateBy { get; set; }
		public string? CreateDate { get; set; }
		public string? UpdateBy { get; set; }
		public string? UpdateDate { get; set; }
		public string? Status { get; set; }
		public string UserID { get; set; }
        //public int UserCommentId { get; set; }
        //public ICollection<UserModel> User { get; set; }//[user many blog one]
        //public ICollection<UserCommentModel> UserComment { get; set; }
    }
}
