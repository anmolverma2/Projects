using Microsoft.EntityFrameworkCore;
using PracticeAPI.Configrations;
using PracticeAPI.Model;
using System.Data;

namespace PracticeAPI.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
        public DbSet<CollegeStudent> collegeStudents { get; set; }
        public DbSet<Department> collegeDepartments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePrivilage> RolePrivilages { get; set; }
        public DbSet<UserRoleMapping> UserRoleMappings { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserImage> UserImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.ApplyConfiguration(new StudentConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new RolePrivilageConfig());
            modelBuilder.ApplyConfiguration(new UserRoleMappingConfig());
            modelBuilder.ApplyConfiguration(new UserTypeConfig());
            modelBuilder.ApplyConfiguration(new UserImageConfig());

            //modelBuilder.Entity<CollegeStudent>(entity =>
            //{
            //    entity.HasKey(n => n.Id);
            //    entity.Property(n => n.Name)
            //          .IsRequired()
            //          .HasMaxLength(250);
            //    entity.Property(n => n.Age)
            //          .IsRequired();
            //    entity.Property(n => n.Address)
            //          .IsRequired(false)
            //          .HasMaxLength(500);

            //    entity.Property(n => n.Mobile)
            //          .IsRequired()
            //          .HasMaxLength(15);
            //});

            //modelBuilder.Entity<CollegeStudent>().HasData(new List<CollegeStudent>
            //{
            //    new CollegeStudent {
            //    Id = 1,
            //    Name = "Anmol",
            //    Age = 22,
            //    Mobile = "7289874520"
            //},
            //new CollegeStudent {
            //    Id = 2,
            //    Name = "Abhi",
            //    Age = 21,
            //    Mobile = "9989874520"
            //},
            //new CollegeStudent {
            //    Id = 3,
            //    Name = "Neeru",
            //    Age = 28,
            //    Mobile = "8989874520"
            //},
            //new CollegeStudent {
            //    Id = 4,
            //    Name = "Akriti",
            //    Age = 30,
            //    Mobile = "8889874520"
            //},
            //new CollegeStudent {
            //    Id = 5,
            //    Name = "Pari",
            //    Age = 6,
            //    Mobile = "6889874520"
            //}
            //});

        }


    }
}
