using System.ComponentModel.DataAnnotations;

namespace Api.Entity
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FollowUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
