using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos.AccountDto;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Services;
using System.Security.Claims;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ICartService _cartService;
        private readonly IGenericRepository<Address> _addressRepo;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, ICartService cartService, IGenericRepository<Address> addressRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _cartService = cartService;
            _addressRepo = addressRepo;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
                return Unauthorized("Invalid email or password");

            // Assign cart if cartId is provided from frontend
            if (!string.IsNullOrEmpty(model.CartId))
            {
                await _cartService.AssignCartToUserAsync(model.CartId, user.Id);
            }

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }

        // Register  api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var user = new AppUser
            {
                DisplayName = model.FirstName + " " + model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, "Customer");

            // Assign cart if cartId is provided from frontend
            if (!string.IsNullOrEmpty(model.CartId))
            {
                await _cartService.AssignCartToUserAsync(model.CartId, user.Id);
            }

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }

        // GetCurrentUser api/account
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindByEmailAsync(email!);

            if (user == null)
                return Unauthorized();

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);
            
            if (user == null) return Unauthorized();

            var addresses = await _addressRepo.GetAllAsync();
            var address = addresses.FirstOrDefault(a => a.AppUserId == user.Id);

            if (address == null) return NotFound("Address not found.");

            return Ok(new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                Area = address.Area,
                City = address.City,
                Country = address.Country
            });
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);

            if (user == null) return Unauthorized();

            var addresses = await _addressRepo.GetAllAsync();
            var address = addresses.FirstOrDefault(a => a.AppUserId == user.Id);

            if (address == null)
            {
                address = new Address
                {
                    AppUserId = user.Id,
                    Street = addressDto.Street,
                    Area = addressDto.Area,
                    City = addressDto.City,
                    Country = addressDto.Country
                };
                await _addressRepo.AddAsync(address);
            }
            else
            {
                address.Street = addressDto.Street;
                address.Area = addressDto.Area;
                address.City = addressDto.City;
                address.Country = addressDto.Country;
                _addressRepo.Update(address);
            }

            await _addressRepo.SaveChangesAsync();

            return Ok(new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                Area = address.Area,
                City = address.City,
                Country = address.Country
            });
        }

        //GET /api/admin/users

    }
}
