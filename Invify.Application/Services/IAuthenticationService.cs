﻿using Invify.Dtos;
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
        Task<AuthenticationResponse> LoginAsync(AuthenticationRequest autne);
        Task<Response> RegisterAsync(RegisterRequest registerRequest);
        Task<Response> LogoutAsync();

    }
}