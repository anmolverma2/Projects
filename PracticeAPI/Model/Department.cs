namespace PracticeAPI.Model
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<CollegeStudent> studentsList { get; set; }
    }
}
