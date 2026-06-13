using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Entities
{
    public class UserPslConfiguration : IEntityTypeConfiguration<UserPslEntity>
    {
        public void Configure(EntityTypeBuilder<UserPslEntity> builder)
        {
            builder.ToTable("UserPsls");

            builder.HasKey(up => new { up.UserId, up.PslId });

            builder.HasOne(up => up.User)
                .WithMany(u => u.UserPsls)
                .HasForeignKey(up => up.UserId);
        }
    }
}
