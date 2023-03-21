using Invify.Dtos;
using Invify.Dtos.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Services
{
    public interface IIdentityService
    {
        Task<Response> LoginAsync(TokenRequest tokenRequest);
        Task<Response> RegisterAsync(RegisterRequest registerRequest, string role);
        Task<Response> LogoutAsync();

    }
}
