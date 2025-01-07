namespace PracticeAPI.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<RolePrivilage> rolePrivilages { get; set; }
        public virtual ICollection<UserRoleMapping> userRoleMappings { get; set; }
    }
}
