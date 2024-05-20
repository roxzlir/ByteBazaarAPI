
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

            //Get Products.Where(x => x.FkCategotyId == id)   
            //Beh�vs en endpoint f�r detta, kommer en int id, Beh�ver �ven Get Products.Where(x => x.Title.Contains(search), 
            //kommer en string search i anropet.S� 2 endpoints



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

            app.MapProductEndpoints();
            app.MapCategoryEndpoints();
            app.MapProductImageEndpoints();



            app.Run();
        }
    }
}
