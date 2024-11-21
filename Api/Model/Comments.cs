using System.ComponentModel.DataAnnotations;

namespace Api.Model
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}
