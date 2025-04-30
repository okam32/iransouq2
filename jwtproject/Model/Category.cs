using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace jwtproject.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PicAddress { get; set; }
        // دسته اصلی `ParentId` ندارد (null است)، بقیه دسته‌ها دارای والد هستند.
        public int? ParentId { get; set; }
        

        //[JsonIgnore]
        // لیست زیرمجموعه‌های هر دسته
        public List<Category> SubCategories { get; set; } = new List<Category>();

        public List<DoctorCategory> DoctorCategories { get; set; }

    }

}
