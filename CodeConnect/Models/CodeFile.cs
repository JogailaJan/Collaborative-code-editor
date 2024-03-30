namespace CodeConnect.Models
{
    public class CodeFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Content { get; set; }
        public DateTime LastModified { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

}
