using Invify.Dtos.Authentication;
using Invify.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Invify.Services;

namespace Invify.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenGenerator _tokenGenerator;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IAuthenticationService authenticationService,
            TokenGenerator tokenGenerator,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public async Task<Response> RegisterAsync([FromBody] RegisterRequest registerRequest, string role = "Basic")
        {
            try
            {
                //check if user exists
                var userExists = await _userManager.FindByEmailAsync(registerRequest.Email);

                //if user empty
                if (userExists != null)
                {
                    return (new Response { Status = "fail", Message = "That email address already exists. Try another." });
                }

                IdentityUser user = new() { UserName = registerRequest.Username, Email = registerRequest.Email };
                var result = await _userManager.CreateAsync(user, registerRequest.Password);

                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                        await _userManager.AddToRoleAsync(user, role);
                    return new Response { Status = "success", Message = "You have registered successfully!" };
                }
                return new Response { Status = "bad request", Message = "Unable to create user, please try again later" };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering");
                return new Response { Status = "fail", Message = "Error while registering, please try again later" };
            }
        }

        public async Task<Response> LoginAsync([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(tokenRequest.UserName, tokenRequest.Password, false, false);

                if (!result.Succeeded)
                {
                    return new Response { Status = "fail", Message = "Invalid Username or Password" };
                }

                var user = await _userManager.FindByNameAsync(tokenRequest.UserName);

                var roles = await _userManager.GetRolesAsync(user);

                var accessToken = _tokenGenerator.GenerateToken(user, roles[0]);
                
                var tokenResponse = new TokenResponse
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = accessToken,
                };
                
                return new Response { Status = "success", Message = tokenResponse.ToString() };
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging in");
                return new Response { Status = "", Message = "Error while logging in" };
            }
        }

        public async Task<Response> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return new Response { Status = "success", Message = "You have logged out successfully! " };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging out");
                return new Response { Status = "fail", Message = "Error while logging out" };
            }
        }


    }
}
