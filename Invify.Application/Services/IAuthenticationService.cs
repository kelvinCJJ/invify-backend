using Invify.Dtos;
using Invify.Dtos.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Services
{
    public interface IAuthenticationService
    {
        Task<Response> LoginAsync(AuthenticationRequest authenticationRequest);
        Task<Response> RegisterAsync(RegisterRequest registerRequest, string role);
        Task<bool> IsLoggedInAsync(string userId);
        Task<Response> LogoutAsync();

    }
}
