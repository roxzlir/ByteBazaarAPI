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
        private static async Task<IActionResult> GetAllProducts(AppDbContext context)
        {
            var products = await context.Products.ToListAsync();
            if (!products.Any())
            {
                return new NotFoundObjectResult("No product found");
            }
            return new OkObjectResult(products);
        }
        //GET - Hämtar en produkt baserat på ID
        private static async Task<IActionResult> GetProductById(int id, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundObjectResult($"Product with id: {id} found");
            }
            return new OkObjectResult(product);
        }
        //POST - Lägg till ny person
        private static async Task<IActionResult> AddProduct(Product product, AppDbContext context)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return new CreatedResult($"/products/{product.ProductId}", product);
        }
        //PUT - Uppdatera en existerande produkt
        private static async Task<IActionResult> UpdateProduct(int id, Product updatedProduct, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundObjectResult($"Product with id: {id} found");
            }
            /*product = updatedProduct;*/ //Vet ej om detta räcker egentligen bara?


            product.Title = updatedProduct.Title;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Image = updatedProduct.Image;
            product.FkCategoryId = updatedProduct.FkCategoryId;

            context.Products.Update(product);
            await context.SaveChangesAsync();
            return new OkObjectResult(product);
        }
        //DELETE - Radera en produkt
        private static async Task<IActionResult> DeleteProduct(int id, AppDbContext context)
        {
            var product = await context.Products.FindAsync(id);
            if(product == null)
            {
                return new NotFoundObjectResult($"Product with id: {id} found");
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return new NoContentResult();
        }

    }
}
