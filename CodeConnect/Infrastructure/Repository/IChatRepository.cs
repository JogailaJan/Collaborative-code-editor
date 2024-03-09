using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeConnect.Models;

namespace CodeConnect.Infrastructure.Repository
{
    public interface IChatRepository
    {
        Project GetProject(int id);
        Task CreateProject(string name, string password, string userId);
        Task JoinProject(int chatId, string userId);
        IEnumerable<Project> GetProjects(string userId);
        Task<Message> CreateMessage(int chatId, string message, string userId);
    }
}
