using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
	{
		public void Configure(EntityTypeBuilder<LedgerEntry> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.TransactionDate).IsRequired();
			builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
			builder.Property(x => x.ReferenceType).IsRequired().HasMaxLength(100);
			builder.Property(x => x.ReferenceId);
			builder.Property(x => x.ContractId);
			builder.Property(x => x.CustomerId);
			builder.Property(x => x.StaffId);

			builder.OwnsOne(x => x.DebitAmount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("DebitAmount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("DebitCurrency").IsRequired().HasMaxLength(3);
			});

			builder.OwnsOne(x => x.CreditAmount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("CreditAmount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("CreditCurrency").IsRequired().HasMaxLength(3);
			});

			builder.OwnsOne(x => x.Balance, m =>
			{
				m.Property(p => p.Amount).HasColumnName("Balance").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("BalanceCurrency").IsRequired().HasMaxLength(3);
			});

			builder.HasOne(x => x.Contract)
				.WithMany()
				.HasForeignKey(x => x.ContractId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Customer)
				.WithMany()
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany()
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

