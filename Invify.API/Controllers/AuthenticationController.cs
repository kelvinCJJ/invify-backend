using Invify.Dtos;
using Invify.Dtos.Authentication;
using Invify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invify.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authenticationService;

        public AuthenticationController(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var authResponse = await _authenticationService.RegisterAsync(registerRequest);

                if (authResponse.Success == true)
                {
                    return Ok(authResponse);
                }

                return BadRequest(authResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Error while registering, please try again later" });
            }
        }

        //login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest tokenRequest)
        {
            try
            {
                var user = await _authenticationService.LoginAsync(tokenRequest);
                
                if (user.Success == false)
                {
                    if (user.Message != null)
                    {
                        return BadRequest(user);
                    }
                    return BadRequest(user);
                    //return NotFound(new Response { Success = false, Message = "Invalid username or password" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging in");
            }
        }

        //logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var response = await _authenticationService.LogoutAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while logging out");
            }
        }

    }
}
