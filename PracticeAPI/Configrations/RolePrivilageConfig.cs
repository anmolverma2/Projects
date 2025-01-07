using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticeAPI.Model;

namespace PracticeAPI.Configrations
{
    public class RolePrivilageConfig : IEntityTypeConfiguration<RolePrivilage>
    {
        public void Configure(EntityTypeBuilder<RolePrivilage> builder)
        {
            builder.ToTable("RolePrivilages");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.RolePrivilegeName).IsRequired();
            builder.Property(x => x.Description).IsRequired();
          //  builder.Property(x => x.RoleId).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();

            builder.HasOne(n => n.role).
                WithMany(n => n.rolePrivilages).
                HasForeignKey(x => x.RoleId).
                HasConstraintName("FK_RolePrivilages_Role");
        }
    }
}
