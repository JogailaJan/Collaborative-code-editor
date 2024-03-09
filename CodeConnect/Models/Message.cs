using System;

namespace CodeConnect.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
