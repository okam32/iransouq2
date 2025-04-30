using jwtproject.Model.Dtos;
using jwtproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using jwtproject.Data;
using Microsoft.EntityFrameworkCore;

namespace jwtproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        


        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto model)
        {

            var parentcategory = await _appDbContext.Category.FirstOrDefaultAsync(x => x.Id == model.ParentId);
            if (parentcategory == null)
            {
                
            }

            var category = new Category
            {
                Name = model.Name,
                ParentId = model.ParentId,
                PicAddress = model.PicAddress
            };

            await _appDbContext.Category.AddAsync(category);
            await _appDbContext.SaveChangesAsync();
            return Ok( "category add successfully.");
           
        }

        // دریافت لیست دسته‌بندی‌های درختی (فقط دسته‌های اصلی)
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var rootCategories =await _appDbContext.Category
                .Include(x => x.SubCategories)
                .ToListAsync();
            foreach(var cat in rootCategories)
            {
                cat.SubCategories = await _appDbContext.Category.Where(x=>x.ParentId == cat.Id).ToListAsync();
            }
            return Ok(rootCategories);
        }

        // دریافت زیرمجموعه‌های یک دسته خاص
        [HttpGet("GetSubCategories/{parentId}")]
        public async Task<IActionResult> GetSubCategories(int parentId)
        {
            var parent =await _appDbContext.Category.Where(x=> x.ParentId == parentId).ToListAsync();
            if (parent == null)
                return NotFound(new { message = "Category not found" });

            return Ok(parent);
        }
        [HttpPost]
        [Route("editcategoryPUT/{id}")]
        public async Task<IActionResult> EditCategoty([FromRoute]int id, [FromBody] AddCategoryDto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var category = await _appDbContext.Category.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                category.Name = model.Name;
                category.ParentId = model.ParentId;

                await _appDbContext.SaveChangesAsync();

                return Ok("categoty update successfully");
            }
            return BadRequest("Invalid Method.");
            
        }
        [HttpPost]
        [Route("deletecategory/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int id)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "DELETE")
            {
                var category = await _appDbContext.Category.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound("category not found");
                }
                _appDbContext.Category.Remove(category);
                await _appDbContext.SaveChangesAsync();

                return Ok("category removed successfully");
            }
            return BadRequest("Invalid Request.");
        }
        
       
    }
}
