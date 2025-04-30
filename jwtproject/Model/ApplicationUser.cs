using Microsoft.AspNetCore.Identity;

namespace jwtproject.Model
{
    public class ApplicationUser : IdentityUser
    {
       
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
