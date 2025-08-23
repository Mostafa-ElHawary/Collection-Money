using System;
using System.Reflection;
using AutoMapper;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.Services;
using CollectionApp.Infrastructure.Data;
using CollectionApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CollectionApp.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			if (services is null) throw new ArgumentNullException(nameof(services));
			if (configuration is null) throw new ArgumentNullException(nameof(configuration));

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty.");
			}

			services.AddDbContextPool<AppDbContext>(options =>
			{
				options.UseSqlServer(connectionString, sql =>
				{
					sql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
					sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
				});
			});

			services.AddAutoMapperProfiles();

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

			// Register entity-specific repositories via IUnitOfWork to ensure single DbContext instance
			services.TryAddScoped<ICustomerRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Customers);
			services.TryAddScoped<IContractRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Contracts);
			services.TryAddScoped<IStaffRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Staff);
			services.TryAddScoped<IInstallmentRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Installments);
			services.TryAddScoped<IPaymentRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Payments);
			services.TryAddScoped<IReceiptRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Receipts);
			services.TryAddScoped<IVisitRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Visits);
			services.TryAddScoped<IFollowUpRepository>(sp => sp.GetRequiredService<IUnitOfWork>().FollowUps);
			services.TryAddScoped<ILedgerEntryRepository>(sp => sp.GetRequiredService<IUnitOfWork>().LedgerEntries);

			// Register application services
			services.AddScoped<ICustomerService, CustomerService>();
			services.AddScoped<IContractService, ContractService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IInstallmentService, InstallmentService>();
			services.AddScoped<IStaffService, StaffService>();
			services.AddScoped<IVisitService, VisitService>();
			services.AddScoped<IFollowUpService, FollowUpService>();
			services.AddScoped<ILedgerService, LedgerService>();
			services.AddScoped<IExportService, ExportService>();

			return services;
		}

		public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
		{
			if (services is null) throw new ArgumentNullException(nameof(services));

			// Use strongly-typed assemblies instead of string-based Assembly.Load
			var assemblies = new[]
			{
				typeof(CollectionApp.Application.Interfaces.IUnitOfWork).Assembly,
				typeof(CollectionApp.Infrastructure.Data.AppDbContext).Assembly
			};
			services.AddAutoMapper(cfg =>
			{
				// Additional AutoMapper configuration can go here if needed
			}, assemblies);

			// Optionally validate configuration in development via a build-time check
			services.PostConfigure<MapperConfiguration>(mapperConfig =>
			{
				// No runtime assertion here to avoid startup failures in production
			});

			return services;
		}
	}
}

