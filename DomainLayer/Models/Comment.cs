using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime CommentTime { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }
        public int ItemId { get; set; }
        public string ItemType { get; set; }
    }
}
