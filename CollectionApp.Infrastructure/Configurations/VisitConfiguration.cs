using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class VisitConfiguration : IEntityTypeConfiguration<Visit>
	{
		public void Configure(EntityTypeBuilder<Visit> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.CustomerId).IsRequired();
			builder.Property(x => x.StaffId).IsRequired();
			builder.Property(x => x.VisitDate).IsRequired();
			builder.Property(x => x.Purpose).IsRequired().HasMaxLength(200);
			builder.Property(x => x.Notes).HasMaxLength(1000);
			builder.Property(x => x.Outcome).HasMaxLength(500);
			builder.Property(x => x.NextVisitDate);
			builder.Property(x => x.Duration);

			builder.HasOne(x => x.Customer)
				.WithMany()
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany(x => x.Visits)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

