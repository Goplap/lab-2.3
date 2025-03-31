using System.ComponentModel.DataAnnotations;

namespace BulletinBoard.DAL.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<Ad> Ads { get; set; } = new List<Ad>();
    }
}
