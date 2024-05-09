
using ByteBazaarAPI.Data;
using ByteBazaarAPI.Endpoints;
using Microsoft.EntityFrameworkCore;
using System;

namespace ByteBazaarAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGet("/emilproduct", async (AppDbContext context) =>
            {
                var query = from image in context.ProductImages
                            join prod in context.Products on image.FkProductId equals prod.ProductId
                            join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                            select new
                            {
                                prod.Title,
                                prod.Description,
                                prod.Price,
                                category = cat.Title,
                                image.URL

                            };

                var grouped = query.GroupBy(x => x.Title).Select(grp => new
                {
                    Title = grp.Key,
                    Description = grp.First().Description,
                    Price = grp.First().Price,
                    category = grp.First().category,
                    image = grp.Select(x => x.URL).ToList()
                }).ToList();

                return Results.Ok(grouped);
            });




            app.MapProductEndpoints();
            app.MapCategoryEndpoints();
            app.MapProductImageEndpoints();



            app.Run();
        }
    }
}
