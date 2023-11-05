using lab5.Data;
using lab5.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

internal partial class Program {
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration.GetConnectionString("MSSQL");
        builder.Services.AddDbContext<InsuranceCompanyContext>(option => option.UseSqlServer(connectionString));
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        builder.Services
            .AddDefaultIdentity<IdentityUser>()
            .AddDefaultTokenProviders()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<InsuranceCompanyContext>();

        builder.Services.AddTransient<InsuranceCompanyCache>();
        builder.Services.AddMemoryCache();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddControllersWithViews(options => {
            options.CacheProfiles.Add("ModelCache",
                new CacheProfile() {
                    Location = ResponseCacheLocation.Any,
                    Duration = 2 * 16 + 240
                });
        });


        var app = builder.Build();

        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseStaticFiles();
        app.UseSession();
        app.UseDbInitializerMiddleware();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
