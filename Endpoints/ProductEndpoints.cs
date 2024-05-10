using ByteBazaarAPI.Data;
using ByteBazaarAPI.DTO;
using ByteBazaarAPI.Models;
using ByteBazaarAPI.ViewModels;
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
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetAllProducts(AppDbContext context)
        {

            var query = from image in context.ProductImages
                        join prod in context.Products on image.FkProductId equals prod.ProductId
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        select new ProductWithImagesDTO
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Category = cat.Title,
                            Images = new List<string> { image.URL }
                        };

            var grouped = query.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Category = grp.First().Category,
                Images = grp.Select(x => x.Images.Single()).ToList()
            }).ToList();

            if (!grouped.Any())
            {
                return TypedResults.NotFound("No products found");
            }

            return TypedResults.Ok(grouped);
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
        //POST - Lägg till ny produkt
        private static async Task<Results<Created<ProductWithImageVM>, NoContent>> AddProduct(ProductWithImageVM model, AppDbContext context)
        {

            using var transaction = context.Database.BeginTransaction();
            try
            {

                context.Products.Add(model.Product);
                await context.SaveChangesAsync();

                var image = new ProductImage
                {
                    URL = model.ImageURL,
                    FkProductId = model.Product.ProductId
                };
                context.ProductImages.Add(image);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
                return TypedResults.Created($"/products/{model}", model);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occurred while adding the product and image",
                    Detail = ex.Message // Du kan lägga till mer information här om du vill
                };
                return TypedResults.NoContent();
            }


            //context.Products.Add(product);
            //await context.SaveChangesAsync();

            //var image = new ProductImage
            //{
            //    URL = product.ProductURL,
            //    FkProductId = product.ProductId,
            //};

            //context.ProductImages.Add(image);
            //await context.SaveChangesAsync();

            //return TypedResults.Created($"/products/{product.ProductId}", product);
        }


        //var addedProduct = await context.Products.Where(x => x.Title == product.Title).FirstOrDefaultAsync();


        //var connectImage = await context.ProductImages.Where(x => x.URL == addedProduct.Images.Where(x => x.URL == "d");


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
