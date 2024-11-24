using System.ComponentModel.DataAnnotations;

namespace Api.Entity
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }
        public required string Content { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
