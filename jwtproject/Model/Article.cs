using System.ComponentModel.DataAnnotations;

namespace jwtproject.Model
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        public string ThumbnailImage { get; set; }

        [Required]
        public string Content { get; set; }

        // لیست نظرات مربوط به مقاله
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
