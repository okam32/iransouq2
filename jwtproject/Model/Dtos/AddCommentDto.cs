using System.ComponentModel.DataAnnotations;

namespace jwtproject.Model.Dtos
{
    public class AddCommentDto
    {
        
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
