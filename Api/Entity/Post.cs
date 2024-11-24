using System.ComponentModel.DataAnnotations;
using Api.Model;

namespace Api.Entity
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string MediaUrl { get; set; }
        public TypePost Type { get; set; }
        public required string DownloadUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
