using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class InstallmentConfiguration : IEntityTypeConfiguration<Installment>
	{
		public void Configure(EntityTypeBuilder<Installment> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.ContractId).IsRequired();
			builder.Property(x => x.InstallmentNumber).IsRequired();
			builder.Property(x => x.DueDate).IsRequired();
			builder.Property(x => x.Status).IsRequired();

			builder.OwnsOne(x => x.Amount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("Amount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
			});

			builder.OwnsOne(x => x.PaidAmount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("PaidAmount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("PaidCurrency").IsRequired().HasMaxLength(3);
			});

			builder.HasMany(x => x.Payments)
				.WithOne(x => x.Installment)
				.HasForeignKey(x => x.InstallmentId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

