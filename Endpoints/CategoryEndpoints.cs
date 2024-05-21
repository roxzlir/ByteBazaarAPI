using ByteBazaarAPI.Data;
using ByteBazaarAPI.DTO;
using ByteBazaarAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private static async Task<Results<Ok<List<Category>>, NotFound<string>>> GetAllCategories(AppDbContext context)
        {
            var categories = await context.Categories.ToListAsync();
            if (!categories.Any())
            {
                return TypedResults.NotFound("No categories found");
            }
            return TypedResults.Ok(categories);
    
        }
        //GET - Hämtar en kategori baserat på ID
        private static async Task<Results<Ok<Category>, NotFound<string>>> GetCategoryById(int id, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return TypedResults.NotFound($"Category with id: {id} found");
            }
            return TypedResults.Ok(category);
        }
        //POST - Lägg till ny kategori
        private static async Task<Created> AddCategory(CategoryDTO categoryDTO, AppDbContext context)
        {
            var category = new Category
            {
                Title = categoryDTO.Title,
                Description = categoryDTO.Description
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return TypedResults.Created();
        }
        //PUT - Uppdatera en existerande kategori
        private static async Task<Results<Ok<CategoryDTO>, NotFound<string>>> UpdateCategory(int id, CategoryDTO updatedCategory, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return TypedResults.NotFound($"Category with id: {id} found");
            }
            category.Title = updatedCategory.Title;
            category.Description = updatedCategory.Description;

            context.Categories.Update(category);
            await context.SaveChangesAsync();
            return TypedResults.Ok(updatedCategory);
        }
        //DELETE - Radera en kategori
        private static async Task<Results<Ok<string>, NotFound<string>>> DeleteCategory(int id, AppDbContext context)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                return TypedResults.NotFound($"Category with id: {id} not found");
            }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return TypedResults.Ok($"Category with id: {id}, was deleted");
        }
    }
}
