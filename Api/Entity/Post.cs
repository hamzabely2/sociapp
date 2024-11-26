using System.ComponentModel.DataAnnotations;
using Api.Model;

namespace Api.Entity
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string Type { get; set; }
        public string DownloadUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
