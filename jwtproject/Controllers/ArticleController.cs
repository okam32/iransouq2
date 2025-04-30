using jwtproject.Data;
using jwtproject.Model.Dtos;
using jwtproject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jwtproject.Controllers
{
    [Route("blog")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticleController(AppDbContext context)
        {
            _context = context;
        }

        // دریافت لیست مقالات
        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _context.Articles.ToListAsync();

            return Ok(articles);
        }

        // دریافت مقاله بر اساس `Id` همراه با نظرات
        [HttpGet("GetArticle/{id}")]
        public async Task<IActionResult> GetArticle([FromRoute]int id)
        {
            var article = await _context.Articles
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.PublishDate,
                    a.ThumbnailImage,
                    a.Content,
                    Comments = a.Comments
                    .Where(c => c.IsApproved)
                    .Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.Content,
                        c.CreatedAt,
                        UserName = c.User.FirstName + " " + c.User.LastName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (article == null)
                return NotFound(new { message = "Article not found" });

            return Ok(article);
        }

        // ایجاد مقاله جدید
        [HttpPost("CreateArticle")]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto model)
        {
            var article = new Article
            {
                Title = model.Title,
                ThumbnailImage = model.ThumbnailImage,
                Content = model.Content
            };

            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Article created successfully", article });
        }

        [HttpPost("EditArticlePUT/{id}")]
        public async Task<IActionResult> EditArticle([FromRoute]int id, [FromBody] CreateArticleDto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                {
                    return NotFound(new { message = "Article not found" });
                }

                // بروزرسانی اطلاعات مقاله
                article.Title = model.Title ?? article.Title;
                article.ThumbnailImage = model.ThumbnailImage ?? article.ThumbnailImage;
                article.Content = model.Content ?? article.Content;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Article updated successfully", article });
            }
            return BadRequest("Invalid Method.");
        }

        [HttpPost("DeleteArticle/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "DELETE")
            {
                var article = await _context.Articles
                .Include(a => a.Comments) // حذف همراه با نظرات
                .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                {
                    return NotFound(new { message = "Article not found" });
                }

                _context.Comments.RemoveRange(article.Comments); // حذف تمام نظرات مرتبط
                _context.Articles.Remove(article); // حذف مقاله

                await _context.SaveChangesAsync();
                return Ok(new { message = "Article deleted successfully" });
            }
            return BadRequest("Invalid Method.");
        }

        [HttpPost("ApproveCommentPUT/{commentId}")]
        public async Task<IActionResult> ApproveComment([FromRoute]int commentId)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null)
                {
                    return NotFound(new { message = "Comment not found" });
                }

                comment.IsApproved = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Comment approved successfully" });
            }
            return BadRequest("Invalid Method.");
        }

    }
}
