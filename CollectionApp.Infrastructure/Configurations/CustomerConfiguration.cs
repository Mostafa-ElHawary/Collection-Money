using CollectionApp.Domain.Entities;
using CollectionApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
	{
		public void Configure(EntityTypeBuilder<Customer> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
			builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
			builder.Property(x => x.NationalId).IsRequired().HasMaxLength(64);
			builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
			builder.Property(x => x.DateOfBirth);

			builder.OwnsOne(x => x.Address, a =>
			{
				a.Property(p => p.Street).HasColumnName("Street").IsRequired().HasMaxLength(256);
				a.Property(p => p.City).HasColumnName("City").IsRequired().HasMaxLength(100);
				a.Property(p => p.State).HasColumnName("State").IsRequired().HasMaxLength(100);
				a.Property(p => p.Country).HasColumnName("Country").IsRequired().HasMaxLength(100);
				a.Property(p => p.PostalCode).HasColumnName("PostalCode").IsRequired().HasMaxLength(20);
			});

			builder.OwnsOne(x => x.Phone, p =>
			{
				p.Property(v => v.CountryCode).HasColumnName("PhoneCountryCode").IsRequired().HasMaxLength(3);
				p.Property(v => v.AreaCode).HasColumnName("PhoneAreaCode").IsRequired().HasMaxLength(5);
				p.Property(v => v.Number).HasColumnName("PhoneNumber").IsRequired().HasMaxLength(12);
			});

			builder.HasMany(x => x.Contracts)
				.WithOne(x => x.Customer)
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

