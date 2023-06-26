using ExceptionHandling.Middlewares;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;

namespace TaskManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<TaskContext>(opt =>
  opt.UseSqlServer(builder.Configuration.GetConnectionString("TaskContext")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<TaskContext>();
                context.Database.EnsureCreated();
                // DbInitializer.Initialize(context);
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "Login",
                //    pattern: "/login",
                //    defaults: new { controller = "User", action = "Login" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Login}");
            });

            app.Run();
        }
    }
}