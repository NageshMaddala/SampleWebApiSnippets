using System.Collections.Generic;
using System.Web.Http;
using SampleSnippets.ActionFilters;
using SampleSnippets.Interfaces;
using SampleSnippets.Models;

namespace SampleSnippets.Controllers
{
    //for web api always prefix with api.
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        private readonly IProductRepository _ProductRepository;
        public ProductsController(IProductRepository productRepository)
        {
            _ProductRepository = productRepository;
        }

        //Routes are defined here.
        //api/products/GetAllProducts
        [HttpGet, Route("PullAllProducts")]
        public IEnumerable<Product> GetProducts()
        {
            return _ProductRepository.GetProducts();
        }

        //api/products/GetProduct/id
        [HttpGet, Route("GetProduct")]
        //[RouterTimerFilter]
        public Product RetrieveProduct(int id)
        {
            return _ProductRepository.GetProduct(id);
        }

        //Model Binding
        //api/products/GetProductIDDesc/1/description/description1
        [HttpGet, Route("GetProductIDDesc/{id}/description/{description}")]
        public Product GetProductBasedOnIdAndDescription(int id, string description)
        {
            return _ProductRepository.GetProductByIdDescription(id, description);
        }

        //api/products/AddProduct
        //Body contains the Product
        //Model State Validation
        [HttpPost, Route("AddProduct")]
        public IHttpActionResult AddProduct(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_ProductRepository.AddProduct(product))
                return Ok();

            return BadRequest("Something went wrong!");
        }

        //api/products/UpdateProduct
        //Body contains the product
        [HttpPost, Route("UpdateProduct")]
        //[ValidateModelState]
        public IHttpActionResult UpdateProduct(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_ProductRepository.UpdateProduct(product))
                return Ok();

            return BadRequest("Something went wrong!");
        }

        //api/products/Delete/id
        public void Delete(int productId)
        {
            _ProductRepository.RemoveProduct(productId);
        }
    }
}
