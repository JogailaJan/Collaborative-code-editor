using CodeConnect.Areas.Identity.Data;
using CodeConnect.Models;
using System;

namespace CodeConnect.Models
{
    public class ChatUser
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public UserRole Role { get; set; }
    }
}