using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ReservationWebApi.DTOs;
using ReservationWebApi.Models;

namespace ReservationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private RoleManager<IdentityRole> _roleManager { get; }

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserForRegisterDTO userForRegisterDTO)
        {
            //Esta sentencia es para crear al adminitrador
            //var crear = await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync("Admin"), "Admin");

            if (await _userManager.FindByEmailAsync(userForRegisterDTO.Email) != null)
                ModelState.AddModelError("Email", "Este email ya ha sido utilizado");

            else if (await _userManager.FindByNameAsync(userForRegisterDTO.Username) != null)
                ModelState.AddModelError("Username", "Este nombre de usuario ya ha sido utilizado");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userForRegisterDTO.Username, Email = userForRegisterDTO.Email , Gender = userForRegisterDTO.Gender};
                var result = await _userManager.CreateAsync(user, userForRegisterDTO.Password);
                if (result.Succeeded)
                {
                    //return BuildToken(userForRegisterDTO);
                    //var addToRole = await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(userForRegisterDTO.Username), "Normal");
                    return StatusCode(201);
                }
                else
                {
                    return BadRequest("Nombre de usuario y/o contraseña incorrectos");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO UserForLoginDTO)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(UserForLoginDTO.Username, UserForLoginDTO.Password,
                                                                      isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return await BuildToken(UserForLoginDTO);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Username y/o Password Incorrectos");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private async Task<IActionResult> BuildToken(UserForLoginDTO userForLoginDTO)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDTO.Username);
            var userInRoles = await _userManager.GetRolesAsync(user);

            if (userInRoles == null)
            {
                ModelState.AddModelError("Roles", "El usuario no pertenece a ningun rol");
                return BadRequest(ModelState);
            }
                           

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Gender, user.Gender),
                //new Claim("MasInformacion", "Cualquier cosa"),                
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userInRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "tudominio.com",
               audience: "tudominio.com",
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration
            });

        }        
    }
}