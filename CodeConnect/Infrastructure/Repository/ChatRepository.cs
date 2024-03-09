using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeConnect.Database;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeConnect.Infrastructure.Respository
{
    public class ChatRepository : IChatRepository
    {
        private AppDbContext _ctx;

        public ChatRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<Message> CreateMessage(int chatId, string message, string userId)
        {
            var Message = new Message
            {
                ProjectId = chatId,
                Text = message,
                Name = userId,
                Timestamp = DateTime.Now
            };

            _ctx.Messages.Add(Message);
            await _ctx.SaveChangesAsync();

            return Message;
        }


        public async Task CreateProject(string name, string password, string userId)
        {
            var project = new Project
            {
                Name = name,
                Password = password,
                //Type = ChatType.Room
            };

            project.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Admin
            });

            _ctx.Projects.Add(project);

            await _ctx.SaveChangesAsync();
        }

        public Project GetProject(int id)
        {
            return _ctx.Projects
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Project> GetProjects(string userId)
        {
            return _ctx.Projects
                .Include(x => x.Users)
                .Where(x => !x.Users
                    .Any(y => y.UserId == userId))
                .ToList();
        }

        public async Task JoinProject(int chatId, string userId)
        {
            var chatUser = new ChatUser
            {
                ProjectId = chatId,
                UserId = userId,
                Role = UserRole.Member
            };

            _ctx.ChatUsers.Add(chatUser);

            await _ctx.SaveChangesAsync();
        }
    }
}