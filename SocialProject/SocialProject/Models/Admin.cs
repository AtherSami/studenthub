using System.ComponentModel.DataAnnotations;

namespace SocialProject.Models
{
    public class Admin
    {

        [Key]
        public int AdminId { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;


    }
}
