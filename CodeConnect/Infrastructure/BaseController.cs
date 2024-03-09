using System.Security.Claims;
using CodeConnect.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CodeConnect.Infrastructure
{
    public class BaseController : Controller
    {
        protected string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}