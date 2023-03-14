using Invify.Domain.Entities;
using Invify.Infrastructure;
using invify_backend.Dtos.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.Swagger;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Invify.API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenGenerator _tokenGenerator;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            TokenGenerator tokenGenerator,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDto)
        {
            //check if user exists
            var userExists = await _userManager.FindByEmailAsync(userDto.Email);

            //if user empty
            if (userExists != null)
            {
                return Ok(new { Value = "This email address already exists" });
            }

            var result = await _userManager.CreateAsync(
                new IdentityUser { UserName = userDto.Username, Email = userDto.Email },
                userDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            //await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO userDto)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(userDto.UserName, userDto.Password, false, false);

                if (!result.Succeeded)
                {
                    return BadRequest("Invalid login attempt");
                }

                var user = await _userManager.FindByNameAsync(userDto.UserName);

                var roles = await _userManager.GetRolesAsync(user);

                var accessToken = _tokenGenerator.GenerateToken(user);

                return Ok(new LoginResponseDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = accessToken,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging in");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging in");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

    }
}
