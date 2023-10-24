namespace ServiceProject_ServerSide.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool isStaff { get; set; }
        public string Uid { get; set; }
        public string ProfilePic { get; set; }
        public List<Project> Projects { get; set; }

    }
}
