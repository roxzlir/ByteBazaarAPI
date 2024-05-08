using ByteBazaarAPI.Data;
using ByteBazaarAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ByteBazaarAPI.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            app.MapGet("/products", GetAllProducts);
            app.MapGet("/products/{id:int}", GetProductById);
            app.MapPost("/products", AddProduct);
            app.MapPut("/products/{id:int}", UpdateProduct);
            app.MapDelete("/products/{id:int}", DeleteProduct);
        }

        //GET - Hämtar alla produkter som finns
        private static async Task<Results<Ok<List<Product>>, NotFound<string>>> GetAllProducts(AppDbContext context)
        {
            var products = await context.Products.ToListAsync();
            if (!products.Any())
            {
                return TypedResults.NotFound("No product not found");
            }
            return TypedResults.Ok(products);
        }

        //GET - Hämtar en produkt baserat på ID
        private static async Task<Results<Ok<Product>, NotFound<string>>> GetProductById(int id, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound($"Product with id: {id} not found");
            }
            return TypedResults.Ok(product);
        }
        //POST - Lägg till ny person
        private static async Task<Created<Product>> AddProduct(Product product, AppDbContext context)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return TypedResults.Created($"/products/{product.ProductId}", product);
        }
        //PUT - Uppdatera en existerande produkt
        private static async Task<Results<Ok<Product>, NotFound<string>>> UpdateProduct(int id, Product updatedProduct, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return TypedResults.NotFound($"Product with id: {id} not found");
            }
            /*product = updatedProduct;*/ //Vet ej om detta räcker egentligen bara?


            product.Title = updatedProduct.Title;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.FkCategoryId = updatedProduct.FkCategoryId;



            context.Products.Update(product);
            await context.SaveChangesAsync();
            return TypedResults.Ok(product);
        }
        //DELETE - Radera en produkt
        private static async Task<Results<Ok<string>, NotFound<string>>> DeleteProduct(int id, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if(product == null)
            {
                return TypedResults.NotFound($"Product with id: {id} not found");
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return TypedResults.Ok($"Product with id: {id} was deleted");
        }

    }
}
