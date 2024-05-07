using ByteBazaarAPI.Data;
using ByteBazaarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteBazaarAPI.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this WebApplication app)
        {
            app.MapGet("/categories", GetAllCategories);
            app.MapGet("/categories/{id:int}", GetCategoryById);
            app.MapPost("/categories", AddCategory);
            app.MapPut("/categories/{id:int}", UpdateCategory);
            app.MapDelete("/categories/{id:int}", DeleteCategory);
        }

        //GET - Hämtar alla kategorier som finns
        private static async Task<IActionResult> GetAllCategories(AppDbContext context)
        {
            var categories = await context.Categories.ToListAsync();
            if (!categories.Any())
            {
                return new NotFoundObjectResult("No categories found");
            }
            return new OkObjectResult(categories);
        }
        //GET - Hämtar en kategori baserat på ID
        private static async Task<IActionResult> GetCategoryById(int id, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundObjectResult($"Category with id: {id} found");
            }
            return new OkObjectResult(category);
        }
        //POST - Lägg till ny kategori
        private static async Task<IActionResult> AddCategory(Category category, AppDbContext context)
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return new CreatedResult($"/categories/{category.CategoryId}", category);
        }
        //PUT - Uppdatera en existerande kategori
        private static async Task<IActionResult> UpdateCategory(int id, Category updatedCategory, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundObjectResult($"Category with id: {id} found");
            }
                category.Title = updatedCategory.Title;

            context.Categories.Update(category);
            await context.SaveChangesAsync();
            return new OkObjectResult(category);
        }
        //DELETE - Radera en kategori
        private static async Task<IActionResult> DeleteCategory(int id, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundObjectResult($"Category with id: {id} found");
            }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
