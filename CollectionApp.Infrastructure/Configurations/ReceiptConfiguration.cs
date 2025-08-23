using CollectionApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollectionApp.Infrastructure.Configurations
{
	public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
	{
		public void Configure(EntityTypeBuilder<Receipt> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.ReceiptNumber).IsRequired().HasMaxLength(64);
			builder.Property(x => x.PaymentId).IsRequired();
			builder.Property(x => x.CustomerId).IsRequired();
			builder.Property(x => x.IssueDate).IsRequired();
			builder.Property(x => x.Description).HasMaxLength(1000);
			builder.Property(x => x.StaffId).IsRequired();

			builder.OwnsOne(x => x.Amount, m =>
			{
				m.Property(p => p.Amount).HasColumnName("Amount").IsRequired().HasPrecision(18, 2);
				m.Property(p => p.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
			});

			builder.HasOne(x => x.Payment)
				.WithOne(x => x.Receipt)
				.HasForeignKey<Receipt>(x => x.PaymentId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.Customer)
				.WithMany()
				.HasForeignKey(x => x.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Staff)
				.WithMany(x => x.Receipts)
				.HasForeignKey(x => x.StaffId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.CreatedAt).IsRequired();
			builder.Property(x => x.UpdatedAt).IsRequired();
		}
	}
}

