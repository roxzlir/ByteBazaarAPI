using ByteBazaarAPI.Data;
using ByteBazaarAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ByteBazaarAPI.Endpoints
{
    public static class ProductImageEndpoints
    {
        public static void MapProductImageEndpoints(this WebApplication app)
        {
            app.MapGet("/images", GetAllImages);
            app.MapGet("/images/{id:int}", GetImageById);
            app.MapPost("/images", AddImage);
            app.MapPut("/images/{id:int}", UpdateImage);
            app.MapDelete("/images/{id:int}", DeleteImage);
        }

        //GET - Hämtar alla bilder som finns
        private static async Task<Results<Ok<List<ProductImage>>, NotFound<string>>> GetAllImages(AppDbContext context)
        {
            var images = await context.ProductImages.ToListAsync();
            if (!images.Any())
            {
                return TypedResults.NotFound("No images found");
            }
            return TypedResults.Ok(images);

        }
        //GET - Hämtar en bild baserat på ID
        private static async Task<Results<Ok<ProductImage>, NotFound<string>>> GetImageById(int id, AppDbContext context)
        {
            var image = await context.ProductImages.FindAsync(id);
            if (image == null)
            {
                return TypedResults.NotFound($"Image with id: {id} found");
            }
            return TypedResults.Ok(image);
        }
        //POST - Lägg till ny bild
        private static async Task<Created<ProductImage>> AddImage(ProductImage image, AppDbContext context)
        {
            context.ProductImages.Add(image);
            await context.SaveChangesAsync();
            return TypedResults.Created($"/images/{image.ProductImageId}", image);
        }
        //PUT - Uppdatera en existerande bild
        private static async Task<Results<Ok<ProductImage>, NotFound<string>>> UpdateImage(int id, ProductImage updateImage, AppDbContext context)
        {
            var image = await context.ProductImages.FindAsync(id);
            if (image == null)
            {
                return TypedResults.NotFound($"Image with id: {id} found");
            }
            image.URL = updateImage.URL;
            image.FkProductId = updateImage.FkProductId;

            context.ProductImages.Update(image);
            await context.SaveChangesAsync();
            return TypedResults.Ok(image);
        }
        //DELETE - Radera en bild
        private static async Task<Results<Ok<string>, NotFound<string>>> DeleteImage(int id, AppDbContext context)
        {
            var image = await context.ProductImages.FindAsync(id);
            if (image == null)
            {
                return TypedResults.NotFound($"Image with id: {id} not found");
            }
            context.ProductImages.Remove(image);
            await context.SaveChangesAsync();
            return TypedResults.Ok($"Image with id: {id}, was deleted");
        }
    }
}

