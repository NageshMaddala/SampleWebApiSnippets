using System.ComponentModel.DataAnnotations;

namespace SampleSnippets.Models
{
    public class Product
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Description { get; set; }

        public Product(int productId, string description)
        {
            ProductId = productId;
            Description = description;
        }

        public Details ProductDetails { get; set; }
    }

    public class Details
    {
        public int Id { get; set; }
    }
}