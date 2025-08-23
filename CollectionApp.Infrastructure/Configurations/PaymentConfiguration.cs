using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
	{
		public void Configure(EntityTypeBuilder<Payment> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.ContractId).IsRequired();
			builder.Property(x => x.InstallmentId).IsRequired();
			builder.Property(x => x.PaymentDate).IsRequired();
			builder.Property(x => x.PaymentMethod).IsRequired();
			builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
			builder.Property(x => x.Notes).HasMaxLength(1000);

			builder.OwnsOne(x => x.Amount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("Amount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
			});

			builder.HasOne(x => x.Contract)
				.WithMany(x => x.Payments)
				.HasForeignKey(x => x.ContractId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Installment)
				.WithMany(x => x.Payments)
				.HasForeignKey(x => x.InstallmentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany(x => x.Payments)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

