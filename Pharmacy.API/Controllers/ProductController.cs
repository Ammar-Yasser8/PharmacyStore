using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos;
using Pharmacy.API.Helpers;
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
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithCategorySpecs(specParams);
            var products = await _productRepo.GetAllWithSpecAsync(spec);

            var countSpec = new ProductWithCountSpecs(specParams);
            var count = await _productRepo.GetCountAsync(countSpec);

            var data = products.Select(p => new ProductToReturnDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.NameEn
            }).ToList();

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
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
