using System.Collections.Generic;
using System.Linq;
using SampleSnippets.Interfaces;
using SampleSnippets.Models;

namespace SampleSnippets.DAL
{
    public class ProductsRepository : IProductRepository
    {
        private readonly List<Product> _ProductsList;
        public ProductsRepository()
        {
            _ProductsList = new List<Product>();

            for (int i = 0; i < 2; i++)
            {
                _ProductsList.Add(new Product(i, "Description" + i));
            }
        }

        public List<Product> GetProducts()
        {
            return _ProductsList;
        }

        public Product GetProduct(int productId)
        {
            return _ProductsList.Find(p => p.ProductId == productId);
        }

        public Product GetProductByIdDescription(int productId, string description)
        {
            var product = _ProductsList.FirstOrDefault(p => p.ProductId == productId && p.Description == description);
            return product;
        }

        public bool AddProduct(Product product)
        {
            _ProductsList.Add(product);
            return true;
        }

        public bool UpdateProduct(Product product)
        {
            Product originalProduct = _ProductsList.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (originalProduct != null)
            {
                originalProduct.Description = product.Description;
                return true;
            }

            return false;
        }

        public bool RemoveProduct(int productId)
        {
            Product product = _ProductsList.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                _ProductsList.Remove(product);
                return true;
            }

            return false;
        }
    }
}