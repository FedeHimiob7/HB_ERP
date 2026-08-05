using Identity.Domain.Entities;
using Identity.Domain.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.EntitiesConfiguration
{
    public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
    {
        public void Configure(EntityTypeBuilder<EventLog> builder)
        {
            builder.ToTable("EventLogs");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .HasConversion(id => id.Value, value => EventLogId.Create(value));

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.OccurredAt)
                .IsRequired();

            builder.Property(e => e.AttemptedEmail)
                .HasMaxLength(256);

            builder.Property(e => e.EntityType)
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasIndex(e => e.OccurredAt);
            builder.HasIndex(e => e.UserId);
        }
    }
}
