using ByteBazaarAPI.Data;
using ByteBazaarAPI.DTO;
using ByteBazaarAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ByteBazaarAPI.Endpoints
{
    public static class ProductImageEndpoints
    {
        public static void MapProductImageEndpoints(this WebApplication app)
        {
            app.MapGet("/products/images", GetAllImages);
            app.MapGet("/products/images/{id:int}", GetImageById);
            app.MapPost("/products/images", AddImage);
            app.MapPut("/products/images/{id:int}", UpdateImage);
            app.MapDelete("/products/images/{id:int}", DeleteImage);
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
        private static async Task<Created<ProductImageDTO>> AddImage(ProductImageDTO imageDTO, AppDbContext context)
        {
            var image = new ProductImage
            {
                URL = imageDTO.URL,
                FkProductId = imageDTO.FkProductId
            };
            context.ProductImages.Add(image);
            await context.SaveChangesAsync();
            return TypedResults.Created($"/product/image/{imageDTO.URL}", imageDTO);
        }
        //PUT - Uppdatera en existerande bild
        private static async Task<Results<Ok<ProductImageDTO>, NotFound<string>>> UpdateImage(int id, ProductImageDTO updateImage, AppDbContext context)
        {
            
            //var image = from i in context.ProductImages where i.URL == url select new { i };

            //image.ToListAsync();

            
            var image = await context.ProductImages.FindAsync();

            if (image == null)
            {
                return TypedResults.NotFound($"Image with id: {id} not found");
            }
            image.URL = updateImage.URL;
            image.FkProductId = updateImage.FkProductId;

            

            context.ProductImages.Update(image);
            await context.SaveChangesAsync();
            return TypedResults.Ok(updateImage);
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

