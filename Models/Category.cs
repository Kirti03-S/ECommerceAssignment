using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerceWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,1000,ErrorMessage ="Invalid Order Number")]
        public int DisplayOrder { get; set; }
    }
}
