using System.ComponentModel.DataAnnotations;

namespace Api.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public required string UserNmame { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool ProfilePrivacy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
