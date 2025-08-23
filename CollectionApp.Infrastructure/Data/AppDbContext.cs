using Microsoft.EntityFrameworkCore;
using CollectionApp.Domain.Entities;

namespace CollectionApp.Infrastructure.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Customer> Customers => Set<Customer>();
		public DbSet<Staff> Staff => Set<Staff>();
		public DbSet<Contract> Contracts => Set<Contract>();
		public DbSet<Installment> Installments => Set<Installment>();
		public DbSet<Payment> Payments => Set<Payment>();
		public DbSet<Receipt> Receipts => Set<Receipt>();
		public DbSet<Visit> Visits => Set<Visit>();
		public DbSet<FollowUp> FollowUps => Set<FollowUp>();
		public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();




		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);

            base.OnModelCreating(modelBuilder);
		}
	}
}
