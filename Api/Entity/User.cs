using System.ComponentModel.DataAnnotations;

namespace Api.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool ProfilePrivacy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Comments> Comments { get; set; } = new List<Comments>();
        public ICollection<Notification> Notification { get; set; } = new List<Notification>();

    }
}
