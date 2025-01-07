using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("collegeDepartments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.DepartmentName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).IsRequired(false).HasMaxLength(500);

            builder.HasData(new List<Department>
            {
               new Department
               {
                   Id = 1,
                   DepartmentName = "Computer Science",
                   Description = "Focuses on programming, algorithms, and software development."
               },
               new Department
               {
                   Id = 2,
                   DepartmentName = "Mechanical Engineering",
                   Description = "Deals with design, construction, and mechanics of machines."
               },
               new Department
               {
                   Id = 3,
                   DepartmentName = "Civil Engineering",
                   Description = "Specializes in infrastructure design and construction."
               },
               new Department
               {
                   Id = 4,
                   DepartmentName = "Electrical Engineering",
                   Description = "Covers electrical systems, electronics, and power generation."
               },
               new Department
               {
                   Id = 5,
                   DepartmentName = "Mathematics",
                   Description = "Focuses on theoretical and applied mathematics."
               },
               new Department
               {
                   Id = 6,
                   DepartmentName = "Physics",
                   Description = "Explores principles of matter, energy, and their interactions."
               }
           });

        }
    }
}
