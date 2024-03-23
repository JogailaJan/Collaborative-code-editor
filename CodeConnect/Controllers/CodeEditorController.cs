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
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CodeConnect.Controllers
{
    [Authorize]
    public class CodeEditorController : BaseController
    {
        private IChatRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public CodeEditorController( UserManager<ApplicationUser> userManager, IChatRepository repo)
        {
            _userManager = userManager;
            _repo = repo;
        }


        public async Task<IActionResult> SendUpdate(
            int roomId,
            string data,
            [FromServices] IHubContext<CodeHub> codeEditor)
        {

            await codeEditor.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveUpdate", new
                {
                    data
                });
            //return RedirectToAction("Project", "Home", new { id = roomId });
            return Ok();
        }
    }
}
