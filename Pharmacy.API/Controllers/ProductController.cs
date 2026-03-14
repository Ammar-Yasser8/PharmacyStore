using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos;
using Pharmacy.API.Helpers;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.ProductSpecs;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Domain.Specification;
using Pharmacy.Services;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IImageService _imageService;

        public ProductController(IGenericRepository<Product> productRepo, IImageService imageService)
        {
            _productRepo = productRepo;
            _imageService = imageService;
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
        [Authorize(Roles = "Admin")]
        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductToReturnDto>> CreateProduct([FromForm] ProductCreateDto dto)
        {
            var imageUrl = await _imageService.UploadImageAsync(dto.Image, "products");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ImageUrl = imageUrl
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, new ProductToReturnDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            });
        }
        [Authorize(Roles = "Admin")]

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
        {
            var product = await _productRepo.GetAsync(id);
            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            if (dto.Image != null)
            {
                var newImage = await _imageService.UploadImageAsync(dto.Image, "products");

                _imageService.DeleteImage(product.ImageUrl);

                product.ImageUrl = newImage;
            }

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productRepo.GetAsync(id);
            if (product == null)
                return NotFound();

            _imageService.DeleteImage(product.ImageUrl);
            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}
