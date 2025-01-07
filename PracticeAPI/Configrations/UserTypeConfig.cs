using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class UserTypeConfig : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(1500);

            builder.HasData(new List<UserType>() {
                new UserType
                {
                    Id = 1,
                    Name = "Student",
                    Description = "Test Student"
                },
                new UserType { 
                    Id = 2,
                    Name = "Parent",
                    Description = "Test Parent"
                },
                new UserType
                {
                    Id = 3,
                    Name = "Faculty",
                    Description = "Test Faculty"
                },
                new UserType { 
                    Id = 4,
                    Name = "Supporting Staff",
                    Description = "Test Description"
                }
            });

        }
    }
}
