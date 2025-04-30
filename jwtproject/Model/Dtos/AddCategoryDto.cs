namespace jwtproject.Model.Dtos
{
    public class AddCategoryDto
    {
        public string Name { get; set; }
        public int? ParentId { get; set; } // اگر مقدار نداشته باشد، دسته اصلی است
        public string PicAddress { get; set; }

    }
}
