

using Invify.Domain.Entities;
using Invify.Infrastructure.Configuration;
using Invify.Dtos.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Invify.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class ManagerController : ControllerBase
    {
        
    }
}
