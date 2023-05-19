using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
    public class LikeModel
    {
        [Key]
        public int LikeId { get; set; }
        public int? PostId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual PostModel Post { get; set; }
        public virtual UserModel User { get; set; }
    }
}
