using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmacy.API.Dtos.UsersDtos;
using Pharmacy.Domain.Entities;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminUsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // GET api/adminusers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserToReturnDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserToReturnDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserToReturnDto
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(result);
        }

        // POST api/adminusers/promote/{userId}
        [HttpPost("promote/{userId}")]
        public async Task<ActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest("User is already an admin");

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User promoted to admin successfully");
        }

        // POST api/adminusers/demote/{userId}
        [HttpPost("demote/{userId}")]
        public async Task<ActionResult> DemoteToUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest("User is not an admin");

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User demoted to user successfully");
        }
    }
}