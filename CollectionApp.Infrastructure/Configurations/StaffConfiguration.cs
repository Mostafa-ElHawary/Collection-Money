using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class StaffConfiguration : IEntityTypeConfiguration<Staff>
	{
		public void Configure(EntityTypeBuilder<Staff> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.EmployeeId).IsRequired().HasMaxLength(64);
			builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
			builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
			builder.Property(x => x.Position).IsRequired().HasMaxLength(100);
			builder.Property(x => x.Department).IsRequired().HasMaxLength(100);
			builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
			builder.Property(x => x.HireDate).IsRequired();
			builder.Property(x => x.IsActive).IsRequired();

			builder.OwnsOne(x => x.Phone, p =>
			{
				p.Property(v => v.CountryCode).HasColumnName("PhoneCountryCode").IsRequired().HasMaxLength(3);
				p.Property(v => v.AreaCode).HasColumnName("PhoneAreaCode").IsRequired().HasMaxLength(5);
				p.Property(v => v.Number).HasColumnName("PhoneNumber").IsRequired().HasMaxLength(12);
			});

			builder.HasMany(x => x.Payments)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.HasMany(x => x.Receipts)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(x => x.Visits)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(x => x.FollowUps)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(x => x.Contracts)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.StaffId);

			builder.Navigation(x => x.Contracts).UsePropertyAccessMode(PropertyAccessMode.Field);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

