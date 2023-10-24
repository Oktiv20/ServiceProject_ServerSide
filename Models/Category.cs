namespace ServiceProject_ServerSide.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public List<Project> Projects { get; set; }

    }
}
