using ByteBazaarAPI.Data;
using ByteBazaarAPI.DTO;
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
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetAllProducts(AppDbContext context)
        {

            var query = from image in context.ProductImages
                        join prod in context.Products on image.FkProductId equals prod.ProductId
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        select new 
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            Category = cat.Title,
                            ImageId = image.ProductImageId,
                            ImageUrl = image.URL,
                            ImageFk = image.FkProductId

                        };

            var grouped = query.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Quantity = grp.First().Quantity,
                Category = grp.First().Category,
                Images = grp.Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk
                }).ToList()
            }).ToList();
            

            if (!grouped.Any())
            {
                return TypedResults.NotFound("No products found");
            }

            return TypedResults.Ok(grouped);
        }

        //GET - Hämtar en produkt baserat på ID
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetProductById(int id, AppDbContext context)
        {
            var query = from image in context.ProductImages
                        join prod in context.Products on image.FkProductId equals prod.ProductId
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        select new
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            Category = cat.Title,
                            ImageId = image.ProductImageId,
                            ImageUrl = image.URL,
                            ImageFk = image.FkProductId

                        };

            query.Where(i => i.ProductId == id);

            if (query == null)
            {
                return TypedResults.NotFound($"Product with id: {id} not found");
            }

            var grouped = query.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Quantity = grp.First().Quantity,
                Category = grp.First().Category,
                Images = grp.Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk
                }).ToList()
            }).ToList();


            return TypedResults.Ok(grouped);
        }
        //POST - Lägg till ny produkt
        private static async Task<Results<Created<ProductDTO>, NoContent>> AddProduct(ProductDTO model, AppDbContext context)
        {

            using var transaction = context.Database.BeginTransaction();
            try
            {
                var product = new Product
                {
                    Title = model.Title,
                    Description = model.Description,
                    Price = model.Price,
                    FkCategoryId = model.FkCategoryId,
                    Quantity = model.Quantity
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();

                var image = new ProductImage
                {
                    URL = model.ImageURL,
                    FkProductId = product.ProductId
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
        }



        //PUT - Uppdatera en existerande produkt
        private static async Task<Results<Ok<ProductDTO>, NotFound<string>>> UpdateProduct(int id, ProductDTO updatedProduct, AppDbContext context)
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
            product.Quantity = updatedProduct.Quantity;

            context.Products.Update(product);
            await context.SaveChangesAsync();
            return TypedResults.Ok(updatedProduct);
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
