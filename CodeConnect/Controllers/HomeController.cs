using CodeConnect.Areas.Identity.Data;
using CodeConnect.Database;
using CodeConnect.Hubs;
using CodeConnect.Infrastructure;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CodeConnect.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private IChatRepository _repo;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IChatRepository repo)
        {
            _logger = logger;
            this._userManager = userManager;
            _repo = repo;
        }

        public IActionResult Index()
        {
            var projects = _repo.GetProjects(GetUserId());

            return View(projects);
        }

        public IActionResult Find([FromServices] AppDbContext ctx)
        {
            var users = ctx.Users
                .Where(x => x.Id != User.GetUserId())
                .ToList();

            return View(users);
        }

        [HttpGet("{id}")]
        public IActionResult Project(int id)
        {
            return View(_repo.GetProject(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(string name, string password)
        {
            await _repo.CreateProject(name, password, GetUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinProject(int id)
        {
            await _repo.JoinProject(id, GetUserId());

            return RedirectToAction("Project", "Home", new { id = id });
        }

        public async Task<IActionResult> SendMessage(
            int roomId,
            string message,
            [FromServices] IHubContext<ChatHub> chat)
        {
            var Message = await _repo.CreateMessage(roomId, message, "User");
            await chat.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    Text = Message.Text,
                    Name = Message.Name,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
                });
            return RedirectToAction("Project", "Home", new { id = roomId });
        }
    }
}
