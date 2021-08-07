using System.Collections.Generic;
using SampleSnippets.Models;

namespace SampleSnippets.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetProducts();

        Product GetProduct(int productId);

        Product GetProductByIdDescription(int productId, string description);

        bool AddProduct(Product product);

        bool UpdateProduct(Product product);

        bool RemoveProduct(int productId);
    }
}