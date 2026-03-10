using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.ProductSpecs;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Domain.Specification;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;
        public ProductController(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }
        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProducts()
        {
            var spec = new ProductWithCategorySpecs();
            var products = await _productRepo.GetAllWithSpecAsync(spec);

            var result = products.Select(p => new ProductToReturnDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.NameEn
            });

            return Ok(result);
        }
        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            var spec = new ProductWithCategorySpecs(id);
            var product = await _productRepo.GetWithSpecAsync(spec);
            if (product == null)
                return NotFound();
            var result = new ProductToReturnDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.NameEn
            };
            return Ok(result);

        }
    }
}
