using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationWebApi.Data;
using ReservationWebApi.DTOs;
using ReservationWebApi.Models;

namespace ReservationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller
    {
        private UserManager<ApplicationUser> _userManager { get; }
        private RoleManager<IdentityRole> _roleManager { get; }

        public RolesController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetRole([FromBody] RolesDTO rolesDTO)
        {
            var role = await _roleManager.FindByIdAsync(rolesDTO.Id);

            if (role != null)
            {
                return Ok(role);
            }

            return NotFound();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RolesDTO rolesDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new IdentityRole { Name = rolesDTO.Name };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return StatusCode(201);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteRole([FromBody] RolesDTO rolesDTO)
        {
            var role = await _roleManager.FindByIdAsync(rolesDTO.Id);

            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok();
            }

            return NotFound();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] RolesDTO rolesDTO)
        {
            var role = await _roleManager.FindByIdAsync(rolesDTO.Id);

            if (role != null)
            {
                role.Name = rolesDTO.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return Ok();
            }

            return NotFound();

        }
    }
}