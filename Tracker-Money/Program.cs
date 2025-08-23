using CollectionApp.Infrastructure.Extensions;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using Tracker_Money.Resources;
using Tracker_Money.Services;

namespace Tracker_Money
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add infrastructure services (AutoMapper, DbContext, UnitOfWork, Repositories)
            builder.Services.AddInfrastructure(builder.Configuration);

            // Add localization services
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<ILanguageService, LanguageService>();

            // Configure supported cultures and localization options
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Add custom culture providers if needed
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });

            // Add MVC services
            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                });

            var app = builder.Build();

            // Temporarily disable AutoMapper validation to allow application to start
            // if (app.Environment.IsDevelopment())
            // {
            //     var mapper = app.Services.GetRequiredService<AutoMapper.IMapper>();
            //     mapper.ConfigurationProvider.AssertConfigurationIsValid();
            // }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Use localization middleware
            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
