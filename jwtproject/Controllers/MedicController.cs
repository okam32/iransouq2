using jwtproject.Data;
using jwtproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtproject.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MedicController : ControllerBase
    {
        private readonly AppDbContext _context;

        
    }
}
