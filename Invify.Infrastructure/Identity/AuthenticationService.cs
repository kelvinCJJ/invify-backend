using Invify.Dtos;
using Invify.Dtos.Authentication;
using Invify.Infrastructure;
using Invify.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Invify.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenGenerator _tokenGenerator;
        private ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            TokenGenerator tokenGenerator,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public async Task<Response> RegisterAsync(RegisterRequest registerRequest)
        {
            //check if user exists
            var userExists = await _userManager.FindByEmailAsync(registerRequest.Email);

            //if user empty
            if (userExists != null)
            {
                return (new Response { Success = false, Message = "That email address already exists. Try another." });
            }

            IdentityUser user = new() { UserName = registerRequest.Username, Email = registerRequest.Email };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync(registerRequest.Role))
                {
                    await _userManager.AddToRoleAsync(user, registerRequest.Role);
                }
                return new Response { Success = true, Message = "You have registered successfully!" };
            }
            return new Response { Success = false, Message = "Unable to create user, please try again later" };
        }

        public async Task<AuthenticationResponse> LoginAsync(AuthenticationRequest authenticationRequest)
        {
            var user = await _userManager.FindByNameAsync(authenticationRequest.Email);

            if (user == null)
            {
                return new AuthenticationResponse { Errors = new[] { "User does not exists" } };
            }

            var result = await _signInManager.PasswordSignInAsync(authenticationRequest.Email, authenticationRequest.Email, false, false);

            if (!result.Succeeded)
            {
                return new AuthenticationResponse { Errors = new[] { "Incorrect password"} };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenGenerator.GenerateToken(user, roles[0]);

            var authenticationResponse = new AuthenticationResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = accessToken,
            };

            return authenticationResponse;

        }

        public async Task<Response> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return new Response { Success = true, Message = "You have logged out successfully! " };

        }


    }
}
