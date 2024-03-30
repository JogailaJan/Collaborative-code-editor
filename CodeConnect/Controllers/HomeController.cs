using CodeConnect.Areas.Identity.Data;
using CodeConnect.Database;
using CodeConnect.Hubs;
using CodeConnect.Infrastructure;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Infrastructure.Respository;
using CodeConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CodeConnect.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private IChatRepository _repo;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IChatRepository repo)
        {
            _logger = logger;
            _userManager = userManager;
            _repo = repo;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var projects = _repo.GetProjects(GetUserId());
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    HttpContext.Session.SetString("ChosenUserName", user.ChosenUserName);

                }
            }
            return View(projects);
        }

        [HttpGet("{id}")]
        public IActionResult Project(int id)
        {
            return View(_repo.GetProject(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(string name, string password, string userName)
        {
            await _repo.CreateProject(name, password, GetUserId(), userName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _repo.DeleteProject(id);
            return RedirectToAction(nameof(Index)); // Redirect to the list of projects
        }
        //[HttpGet]
        //public async Task<IActionResult> JoinProject(int id)
        //{
        //  await _repo.JoinProject(id, GetUserId());

        //return RedirectToAction("Project", "Home", new { id = id });
        //}
        [HttpPost]
        public async Task<IActionResult> JoinProject(string name, string password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from claims.
            try
            {
                var project = await _repo.JoinProjectAsync(name, password, userId);
                return RedirectToAction("Index");
                //return Ok(new { message = "User added to project successfully." });
            }
            catch (Exception ex)
            {
                // Determine the type of exception to return the correct response.
                if (ex.Message == "Project not found.")
                {
                    return NotFound(ex.Message);
                }
                else if (ex.Message == "Password is incorrect." || ex.Message == "User is already a member of the project.")
                {
                    return BadRequest(ex.Message);
                }
                else
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }
        }

        public async Task<IActionResult> SendMessage(
            int roomId,
            string message,
            string userName,
            [FromServices] IHubContext<ChatHub> chat)
        {
            var Message = await _repo.CreateMessage(roomId, message, userName);

            await chat.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    Text = Message.Text,
                    Name = Message.Name,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
                });
            //return RedirectToAction("Project", "Home", new { id = roomId });
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCodeFile(int projectId, string content)
        {
            try
            {
                // Assuming you have a method in your repository to handle the update
                await _repo.UpdateCodeFileAsync(projectId, content);
                return Json(new { success = true, message = "Code file updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
