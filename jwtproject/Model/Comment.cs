using System.ComponentModel.DataAnnotations;

namespace jwtproject.Model
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // تعیین وضعیت تأیید
        public bool IsApproved { get; set; } = false;

        // ارتباط با مقاله
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        // ارتباط با کاربر لاگین شده
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }

}
