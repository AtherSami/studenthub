namespace SocialProject.Models
{
	public class NotificationModel
	{
		public int Id { get; set; }
		public string? Message { get; set; }
		public NotificationType Type { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsRead { get; set; }
		public int? SenderId { get; set; }
		public int? ReceiverId { get; set; }
		public string? SenderName { get; set; }
		public int? PostId { get; set; }
		public string? ReceiverName { get; set; }
		public string? Senderpic { get;set;}

	}

	public enum NotificationType
	{
		PostRequest,
		PostAccepted,
		Comment,
		Like,
		
	}

}
