namespace ServiceProject_ServerSide.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Image { get; set; }
        public int Duration { get; set; }
        public string Date { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }  
        public List<User> Users { get; set; }

    }
}
