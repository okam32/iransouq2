using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace jwtproject.Model
{
    public class DoctorCategory
    {
        [Key]
        public int Id { get; set; }
        public string DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        [JsonIgnore]
        public Doctor Doctor { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public Category Category { get; set; }
    }

}
