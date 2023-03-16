using Invify.Infrastructure;
using Invify.Dtos;
using Invify.Dtos.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDto, string role = "Basic")
        {
            try
            {
                //check if user exists
                var userExists = await _userManager.FindByEmailAsync(userDto.Email);

                //if user empty
                if (userExists != null)
                {
                    return Ok(new { Value = "That email address already exists. Try another." });
                }

                IdentityUser user = new() { UserName = userDto.Username, Email = userDto.Email };
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded && await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "success", Message = "You have registered successfully!" });
                }
                else
                {
                    return BadRequest(new Response { Status="fail", Message= "Role does not exist!"});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "fail", Message = "Error while registering, please try again later" });
            }
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

                var accessToken = _tokenGenerator.GenerateToken(user, roles.Last());

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
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging out");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging out");
            }
        }

    }
}
