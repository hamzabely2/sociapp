using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class Post
{
    
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    [NotMapped] 
    public IFormFile MediaUrl { get; set; }
    public string Type { get; set; }
    public string DownloadUrl { get; set; }
    public int UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}
 