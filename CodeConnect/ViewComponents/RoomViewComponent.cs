using System.Linq;
using System.Security.Claims;
using CodeConnect.Database;
using CodeConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeConnect.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private AppDbContext _ctx;

        public RoomViewComponent(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var projects = _ctx.ChatUsers
                .Include(x => x.Project)
                .Where(x => x.UserId == userId)
                .Select(x => x.Project)
                .ToList();

            return View(projects);
        }
    }
}