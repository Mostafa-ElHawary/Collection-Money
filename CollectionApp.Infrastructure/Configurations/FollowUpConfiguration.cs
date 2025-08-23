using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class FollowUpConfiguration : IEntityTypeConfiguration<FollowUp>
	{
		public void Configure(EntityTypeBuilder<FollowUp> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.CustomerId).IsRequired();
			builder.Property(x => x.StaffId).IsRequired();
			builder.Property(x => x.ContractId);
			builder.Property(x => x.ScheduledDate).IsRequired();
			builder.Property(x => x.ActualDate);
			builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
			builder.Property(x => x.Priority).IsRequired().HasMaxLength(50);
			builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
			builder.Property(x => x.Notes).HasMaxLength(1000);
			builder.Property(x => x.Outcome).HasMaxLength(500);

			builder.HasOne(x => x.Customer)
				.WithMany()
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Contract)
				.WithMany()
				.HasForeignKey(x => x.ContractId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany(x => x.FollowUps)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

