using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class StudentConfig : IEntityTypeConfiguration<CollegeStudent>
    {
        public void Configure(EntityTypeBuilder<CollegeStudent> builder)
        {
            builder.ToTable("collegeStudents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Name).IsRequired().HasMaxLength(250);
            builder.Property(n => n.Age).IsRequired();
            builder.Property(n => n.Address).IsRequired(false).HasMaxLength(500);            
            builder.Property(n => n.Mobile).IsRequired().HasMaxLength(15);

            builder.HasData(new List<CollegeStudent>
            {
                new CollegeStudent
                {
                    Id = 1,
                    Name = "Anmol",
                    Age = 22,
                    Mobile = "7289874520",
                    Email = "anmol@example.com",
                    Address = "123 Elm Street",
                    DOB = new DateTime(2001, 5, 12)
                },
                new CollegeStudent
                {
                    Id = 2,
                    Name = "Abhi",
                    Age = 21,
                    Mobile = "9989874520",
                    Email = "abhi@example.com",
                    Address = "456 Oak Avenue",
                    DOB = new DateTime(2002, 3, 8)
                },
                new CollegeStudent
                {
                    Id = 3,
                    Name = "Neeru",
                    Age = 28,
                    Mobile = "8989874520",
                    Email = "neeru@example.com",
                    Address = "789 Pine Boulevard",
                    DOB = new DateTime(1995, 10, 20)
                },
                new CollegeStudent
                {
                    Id = 4,
                    Name = "Akriti",
                    Age = 30,
                    Mobile = "8889874520",
                    Email = "akriti@example.com",
                    Address = "321 Maple Lane",
                    DOB = new DateTime(1993, 1, 15)
                },
                new CollegeStudent
                {
                    Id = 5,
                    Name = "Pari",
                    Age = 6,
                    Mobile = "6889874520",
                    Email = "pari@example.com",
                    Address = "987 Willow Drive",
                    DOB = new DateTime(2017, 6, 5)
                }
            });

            builder.HasOne(n => n.department).
                WithMany(n => n.studentsList).
                HasForeignKey(x => x.DepartmentId).
                HasConstraintName("FK_Students_Department");

        }
    }
}
