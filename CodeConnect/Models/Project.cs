using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeConnect.Models
{
    public class Project
    {
        public Project()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
            CodeFiles = new List<CodeFile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Owner { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }
        public ICollection<CodeFile> CodeFiles { get; set; }
    }
}
