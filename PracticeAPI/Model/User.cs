namespace PracticeAPI.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public int UserTypeId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CretedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<UserRoleMapping> userRoleMappings { get; set; }
        public virtual UserType userType { get; set; }
    }
}
