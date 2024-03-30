using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using CodeConnect.Database;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeConnect.Infrastructure.Respository
{
    public class ChatRepository : IChatRepository
    {
        private AppDbContext _ctx;

        public ChatRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<Message> CreateMessage(int chatId, string message, string userName)
        {
            var Message = new Message
            {
                ProjectId = chatId,
                Text = message,
                Name = userName,
                Timestamp = DateTime.Now
            };

            _ctx.Messages.Add(Message);
            await _ctx.SaveChangesAsync();

            return Message;
        }


        public async Task CreateProject(string name, string password, string userId, string userName)
        {
            var project = new Project
            {
                Name = name,
                Password = password,
                Owner = userName,
                CreationDate = DateTime.Now
                //Type = ChatType.Room
            };

            project.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Admin
            });

            _ctx.Projects.Add(project);

            await _ctx.SaveChangesAsync();

            await CreateCodeFileAsync(project.Id, "Default.cs");
        }
        public async Task DeleteProject(int id)
        {
            var project = await _ctx.Projects.FindAsync(id);
            if (project == null)
            {
                throw new Exception("Project not found.");
            }

            _ctx.Projects.Remove(project);
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

        public async Task<Project> JoinProjectAsync(string name, string password, string userId)
        {
            var project = await _ctx.Projects
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Name == name);

            if (project == null)
            {
                throw new Exception("Project not found.");
            }
            if (project.Password != password) // This should be a secure password check in production code.
            {
                throw new Exception("Password is incorrect.");
            }
            if (project.Users.Any(u => u.UserId == userId))
            {
                throw new Exception("User is already a member of the project.");
            }

            project.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Member // Adjust role accordingly.
            });

            await _ctx.SaveChangesAsync();

            return project; // Return the project if successful.
        }

        public async Task<CodeFile> CreateCodeFileAsync(int projectId, string fileName)
        {
            var project = await _ctx.Projects.FindAsync(projectId);
            if (project == null)
                throw new Exception("Project not found.");

            var codeFile = new CodeFile
            {
                FileName = "main.txt",
                Content = "// Initial code",
                LastModified = DateTime.Now,
                ProjectId = projectId
            };

            _ctx.CodeFiles.Add(codeFile);
            await _ctx.SaveChangesAsync();
            return codeFile;
        }
        public async Task UpdateCodeFileAsync(int codeFileId, string newContent)
        {
            var oldCodeFile = await _ctx.CodeFiles.FindAsync(codeFileId);
            if (oldCodeFile == null)
                throw new Exception("Code file not found.");
            var newCodeFile = oldCodeFile;
            newCodeFile.Content = newContent;
            newCodeFile.LastModified = DateTime.Now;
            _ctx.Entry(oldCodeFile).CurrentValues.SetValues(newCodeFile);
            await _ctx.SaveChangesAsync();
        }

        public CodeFile GetCodeFile(int codeFileId)
        {
            return _ctx.CodeFiles
                .FirstOrDefault(x => x.Id == codeFileId);
        }

        public IEnumerable<CodeFile> GetProjectCodeFiles(int projectId)
        {
            return _ctx.CodeFiles
                .Where(x => x.ProjectId == projectId)
                .ToList();
        }

    }
}