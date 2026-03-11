using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Services;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IImageService _imageService;

        public CategoryController(IGenericRepository<Category> categoryRepo, IImageService imageService)
        {
            _categoryRepo = categoryRepo;
            _imageService = imageService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryToReturnDto>>> GetCategories()
        {
            var categories = await _categoryRepo.GetAllAsync();

            var data = categories.Select(c => new CategoryToReturnDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                NameRu = c.NameRu,
                ImageUrl = c.ImageUrl
            }).ToList();

            return Ok(data);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryToReturnDto>> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetAsync(id);
            if (category == null)
                return NotFound();

            var result = new CategoryToReturnDto
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                NameRu = category.NameRu,
                ImageUrl = category.ImageUrl
            };

            return Ok(result);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryToReturnDto>> CreateCategory([FromForm] CategoryCreateDto dto)
        {
            var imageUrl = await _imageService.UploadImageAsync(dto.Image, "categories");

            var category = new Category
            {
                NameAr = dto.NameAr,
                NameEn = dto.NameEn,
                NameRu = dto.NameRu,
                ImageUrl = imageUrl
            };

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, new CategoryToReturnDto
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                NameRu = category.NameRu,
                ImageUrl = category.ImageUrl
            });
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateDto dto)
        {
            var category = await _categoryRepo.GetAsync(id);
            if (category == null)
                return NotFound();

            category.NameAr = dto.NameAr;
            category.NameEn = dto.NameEn;
            category.NameRu = dto.NameRu;

            if (dto.Image != null)
            {
                var newImage = await _imageService.UploadImageAsync(dto.Image, "categories");

                _imageService.DeleteImage(category.ImageUrl);

                category.ImageUrl = newImage;
            }

            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetAsync(id);
            if (category == null)
                return NotFound();

            _imageService.DeleteImage(category.ImageUrl);
            _categoryRepo.Delete(category);
            await _categoryRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}
