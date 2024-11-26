using System.ComponentModel.DataAnnotations;

namespace Api.Entity
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
