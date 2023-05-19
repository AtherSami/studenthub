using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Attachment { get; set; }
        public string? Gender { get; set; }
        public string? UserRole { get; set; }
        public string? Address { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
     
 
 public ICollection<UserCommentModel>? Commentss { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiration { get; set; }


    }


}

