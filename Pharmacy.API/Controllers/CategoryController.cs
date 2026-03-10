using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        public CategoryController(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryToReturnDto>>> GetCategories()
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
    }
}
