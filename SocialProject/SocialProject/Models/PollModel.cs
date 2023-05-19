using System.ComponentModel.DataAnnotations;


namespace SocialProject.Models
{
	public class PollModel
	{
		[Key]
		public int PollId { get; set; }	
		public string? PollTitle { get; set; }
		public string? PollChoices { get; set;}
		public string? CreateBy { get; set; }
		public string? CreateDate { get; set; }
		public string? UpdateBy { get; set; }
		public string? UpdateDate { get; set; }
		//public string UserID { get; set; }

	}
}
