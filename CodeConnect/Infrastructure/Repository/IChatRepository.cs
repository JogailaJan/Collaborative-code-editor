using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeConnect.Infrastructure.Repository
{
    public interface IChatRepository
    {
        Project GetProject(int id);
        Task CreateProject(string name, string password, string userId, string userName);
        Task DeleteProject(int id);
        Task<Project> JoinProjectAsync(string name, string password, string userId);
        IEnumerable<Project> GetProjects(string userId);
        Task<Message> CreateMessage(int chatId, string message, string userName);
        Task UpdateCodeFileAsync(int projectId, string content);
    }
}
