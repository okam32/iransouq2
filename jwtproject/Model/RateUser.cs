using System.ComponentModel.DataAnnotations.Schema;

namespace jwtproject.Model
{
    public class RateUser
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public string DoctorId  { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor DoctorEntitie { get; set; }
    }
}
