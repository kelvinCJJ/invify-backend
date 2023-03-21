using Invify.Infrastructure;
using Invify.Dtos;
using Invify.Dtos.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Invify.Services;

namespace Invify.API.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthenticationController(
            IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, string role = "Basic")
        {
            try
            {
                if (registerRequest != null)
                {

                    var userRegistered = await _identityService.RegisterAsync(registerRequest, role);
                    if (userRegistered.Status == "success")
                    {
                        return Ok(userRegistered);
                    }
                    else
                    {
                        return BadRequest(userRegistered);
                    }
                }

                return BadRequest(new Response { Status = "fail", Message = "Role does not exist!" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "fail", Message = "Error while registering, please try again later" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] TokenRequest tokenRequest)
        {
            try
            {

                if (tokenRequest != null)
                {
                    var user = await _identityService.LoginAsync(tokenRequest);
                    if (user != null)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest(new Response { Status = "fail", Message = "Role does not exist!" });
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging in");
            }
        }

        //
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _identityService.LogoutAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging out");
            }
        }

    }
}
