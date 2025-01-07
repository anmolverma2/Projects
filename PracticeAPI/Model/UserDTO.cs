namespace PracticeAPI.Model
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public int UserTypeId { get; set; }
    }
}
