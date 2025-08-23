using CollectionApp.Domain.Entities;
using CollectionApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class ContractConfiguration : IEntityTypeConfiguration<Contract>
	{
		public void Configure(EntityTypeBuilder<Contract> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.ContractNumber).IsRequired().HasMaxLength(64);
			builder.Property(x => x.CustomerId).IsRequired();
			builder.Property(x => x.StaffId).IsRequired(false);
			builder.Property(x => x.StartDate).IsRequired();
			builder.Property(x => x.EndDate);
			builder.Property(x => x.Status).IsRequired();
			builder.Property(x => x.InterestRate).IsRequired();
			builder.Property(x => x.NumberOfInstallments).IsRequired();

			builder.HasOne(x => x.Customer)
				.WithMany(x => x.Contracts)
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany(x => x.Contracts)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.OwnsOne(x => x.TotalAmount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("TotalAmount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
			});

			builder.OwnsOne(x => x.PrincipalAmount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("PrincipalAmount").HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("PrincipalCurrency").HasMaxLength(3);
			});

			builder.HasMany(x => x.Installments)
				.WithOne(x => x.Contract)
				.HasForeignKey(x => x.ContractId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(x => x.Payments)
				.WithOne(x => x.Contract)
				.HasForeignKey(x => x.ContractId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(x => x.StaffId).HasDatabaseName("IX_Contract_StaffId");

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

