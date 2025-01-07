using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class UserRoleMappingConfig : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.ToTable("UserRoleMappings");
            builder.HasKey(x => x.Id);

            builder.Property(x =>  x.Id).UseIdentityColumn();
            builder.HasIndex(n => new { n.UserId, n.RoleId }, "UK_UserMapping").IsUnique();

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.RoleId).IsRequired();

            builder.HasOne(x => x.Role).
                WithMany(n => n.userRoleMappings).
                HasForeignKey(x => x.RoleId).
                HasConstraintName("FK_UserRoleMapping_Role");
            builder.HasOne(x => x.User).
                WithMany(n => n.userRoleMappings).
                HasForeignKey(x => x.UserId).
                HasConstraintName("FK_UserRoleMapping_User");

        }
    }
}
