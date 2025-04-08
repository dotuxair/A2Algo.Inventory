using A2Algo.Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace A2Algo.Inventory
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("InventoryDatabase");
            builder.Services.AddDbContext<InventoryDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddControllers();

            // CORS policy setup
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            // Other middleware
            app.UseAuthorization();

            // Map controllers for API endpoints
            app.MapControllers();

            app.Run();
        }
    }
}
