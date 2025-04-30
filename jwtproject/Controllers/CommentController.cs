using jwtproject.Data;
using jwtproject.Model;
using jwtproject.Model.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace jwtproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // اضافه کردن نظر به یک مقاله (فقط برای کاربران لاگین شده)
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto model)
        {
            var article = await _context.Articles.FindAsync(model.ArticleId);
            if (article == null)
            {
                return NotFound(new { message = "Article not found" });
            }

            // دریافت کاربر فعلی از Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var comment = new Comment
            {
                Title = model.Title,
                Content = model.Content,
                ArticleId = model.ArticleId,
                UserId = userId
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment added successfully", comment });
        }
        [HttpPost]
        [Route("confirmcomment/{id}")]
        public async Task<IActionResult> ConfirmComment([FromRoute] int id)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
                if (comment == null)
                {
                    return NotFound(new { Message = "comment dose not exist" });
                }

                comment.IsApproved = true;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "comment confirm successfully." });
            }
            return BadRequest("Invalid Method.");
        }

        [HttpGet]
        [Route("getcomments")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _context.Comments
                .Include(a=>a.User)
                .ToListAsync();

            return Ok(comments);
        }

        [HttpGet]
        [Route("getarticlecomment/{id}")]
        public async Task<IActionResult> GetArticleComments([FromRoute]int id)
        {
            var comments = await _context.Comments.Where(x=> x.ArticleId == id).ToListAsync();
            if (comments == null)
            {
                return NotFound(new { Message = "no comments for this article" });
            }
            return Ok(comments);
        }
        [HttpGet]
        [Route("getcomments/{id}")]
        public async Task<IActionResult> GetComment([FromRoute]int id)
        {
            var comment = _context.Comments.FirstOrDefault(x=>x.Id == id);
            if(comment == null)
            {
                return NotFound(new {Message = "comment not found"});
            }
            return Ok(comment);
        }
    }
}
