using ByteBazaarAPI.Data;
using ByteBazaarAPI.DTO;
using ByteBazaarAPI.Migrations;
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
            app.MapGet("/category/{id:int}/products", GetProductByCategoryId);
            app.MapGet("/products/search/{search}", GetProductsBySearch);
            //app.MapGet("/products/results/{id:int}/page/{id:int}", GetProductsPaginated);
        }

        //GET - Hämtar alla produkter som finns
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetAllProducts(AppDbContext context)
        {

            var query = from prod in context.Products
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        join image in context.ProductImages on prod.ProductId equals image.FkProductId into images
                        from img in images.DefaultIfEmpty()
                        select new
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            IsCampaign = prod.IsCampaign,
                            CampaignPercent = prod.CampaignPercent,
                            TempPrice = prod.TempPrice,
                            FkCategoryId = prod.FkCategoryId,
                            CategoryId = cat.CategoryId,
                            CategoryTitle = cat.Title,
                            CategoryDescription = cat.Description,
                            ImageId = img != null ? img.ProductImageId : (int?)null,
                            ImageUrl = img != null ? img.URL : null,
                            ImageFk = img != null ? img.FkProductId : (int?)null
                        };

            var results = await query.ToListAsync();

            var grouped = results.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Quantity = grp.First().Quantity,
                FkCategoryId = grp.First().FkCategoryId,
                IsCampaign = grp.First().IsCampaign,
                CampaignPercent = grp.First().CampaignPercent,
                TempPrice = grp.First().TempPrice,
                Category = new Category
                {
                    CategoryId = grp.First().CategoryId,
                    Title = grp.First().CategoryTitle,
                    Description = grp.First().CategoryDescription
                },
                Images = grp.Where(x => x.ImageId != null).Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId.Value,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk.Value
                }).ToList()
            }).ToList();

            if (!grouped.Any())
            {
                return TypedResults.NotFound("No products found");
            }

            return TypedResults.Ok(grouped);
        }

        //GET - Hämtar en produkt baserat på ID
        private static async Task<Results<Ok<ProductWithImagesDTO>, NotFound<string>>> GetProductById(int id, AppDbContext context)
        {

                var query = from prod in context.Products
                            join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                            join image in context.ProductImages on prod.ProductId equals image.FkProductId into images
                            from img in images.DefaultIfEmpty()
                            where prod.ProductId == id
                            select new
                            {
                                ProductId = prod.ProductId,
                                Title = prod.Title,
                                Description = prod.Description,
                                Price = prod.Price,
                                Quantity = prod.Quantity,
                                FkCategoryId = prod.FkCategoryId,
                                IsCampaign = prod.IsCampaign,
                                CampaignPercent = prod.CampaignPercent,
                                TempPrice = prod.TempPrice,
                                CategoryId = cat.CategoryId,
                                CategoryTitle = cat.Title,
                                CategoryDescription = cat.Description,
                                ImageId = img != null ? img.ProductImageId : (int?)null,
                                ImageUrl = img != null ? img.URL : null,
                                ImageFk = img != null ? img.FkProductId : (int?)null
                            };

                var productData = await query.ToListAsync();

                if (productData == null || !productData.Any())
                {
                    return TypedResults.NotFound($"Product with id: {id} not found");
                }

                var grouped = productData.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
                {
                    ProductId = grp.Key,
                    Title = grp.First().Title,
                    Description = grp.First().Description,
                    Price = grp.First().Price,
                    Quantity = grp.First().Quantity,
                    IsCampaign = grp.First().IsCampaign,
                    CampaignPercent = grp.First().CampaignPercent,
                    TempPrice = grp.First().TempPrice,
                    FkCategoryId = grp.First().FkCategoryId,
                    Category = new Category
                    {
                        CategoryId = grp.First().CategoryId,
                        Title = grp.First().CategoryTitle,
                        Description = grp.First().CategoryDescription
                    },
                    Images = grp.Where(x => x.ImageId != null).Select(x => new ProductImage
                    {
                        ProductImageId = x.ImageId.Value,
                        URL = x.ImageUrl,
                        FkProductId = x.ImageFk.Value
                    }).ToList()
                }).FirstOrDefault();

                if (grouped == null)
                {
                    return TypedResults.NotFound($"Product with id: {id} not found");
                }

                return TypedResults.Ok(grouped);

        }
        //POST - Lägg till ny produkt
        private static async Task<Results<Created, NoContent>> AddProduct(ProductDTO model, AppDbContext context)
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
                    Quantity = model.Quantity,
                    IsCampaign = model.IsCampaign,
                    CampaignPercent = model.CampaignPercent,
                    TempPrice = model.TempPrice,
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
                return TypedResults.Created();
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
            product.Quantity = updatedProduct.Quantity;
            product.IsCampaign = updatedProduct.IsCampaign;
            product.CampaignPercent = updatedProduct.CampaignPercent;
            product.TempPrice = updatedProduct.TempPrice;


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

        //GET - Hämtar alla produkter baserat på id i FkCategoryId
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetProductByCategoryId(int id, AppDbContext context)
        {
            var query = from image in context.ProductImages
                        join prod in context.Products on image.FkProductId equals prod.ProductId
                        where prod.FkCategoryId == id
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        select new
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            IsCampaign = prod.IsCampaign,
                            CampaignPercent = prod.CampaignPercent,
                            TempPrice = prod.TempPrice,
                            FkCategoryId = prod.FkCategoryId,
                            CategoryId = cat.CategoryId,
                            CategoryTitle = cat.Title,
                            CategoryDescription = cat.Description,
                            ImageId = image.ProductImageId,
                            ImageUrl = image.URL,
                            ImageFk = image.FkProductId

                        };

            var productData = query.ToList();

            if (productData == null || !productData.Any())
            {
                return TypedResults.NotFound($"Could not find any products in category with: {id}");
            }

            var grouped = productData.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Quantity = grp.First().Quantity,
                IsCampaign = grp.First().IsCampaign,
                CampaignPercent = grp.First().CampaignPercent,
                TempPrice = grp.First().TempPrice,
                FkCategoryId = grp.First().FkCategoryId,
                Category = new Category
                {
                    CategoryId = grp.First().CategoryId,
                    Title = grp.First().CategoryTitle,
                    Description = grp.First().CategoryDescription
                },
                Images = grp.Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk
                }).ToList()
            }).ToList();


            return TypedResults.Ok(grouped);
        }

        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetProductsBySearch(string search, AppDbContext context)
        {

            var query = from image in context.ProductImages
                        join prod in context.Products on image.FkProductId equals prod.ProductId
                        where prod.Title.Contains(search)
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        select new
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            IsCampaign = prod.IsCampaign,
                            CampaignPercent = prod.CampaignPercent,
                            TempPrice = prod.TempPrice,
                            FkCategoryId = prod.FkCategoryId,
                            CategoryId = cat.CategoryId,
                            CategoryTitle = cat.Title,
                            CategoryDescription = cat.Description,
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
                IsCampaign = grp.First().IsCampaign,
                CampaignPercent = grp.First().CampaignPercent,
                TempPrice = grp.First().TempPrice,
                FkCategoryId = grp.First().FkCategoryId,
                Category = new Category
                {
                    CategoryId = grp.First().CategoryId,
                    Title = grp.First().CategoryTitle,
                    Description = grp.First().CategoryDescription
                },

                Images = grp.Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk
                }).ToList()
            }).ToList();


            if (!grouped.Any())
            {
                return TypedResults.NotFound($"No products with {search} in there Title found");
            }

            return TypedResults.Ok(grouped);
        }

        //GET paginated result all products
        private static async Task<Results<Ok<List<ProductWithImagesDTO>>, NotFound<string>>> GetProductsPaginated(int results, int page, AppDbContext context)
        {
            if (results <= 0 || page <= 0)
            {
                return TypedResults.NotFound("Invalid pagination parameters");
            }
            Console.WriteLine("Start");

            var query = from prod in context.Products
                        join cat in context.Categories on prod.FkCategoryId equals cat.CategoryId
                        join image in context.ProductImages on prod.ProductId equals image.FkProductId into images
                        from img in images.DefaultIfEmpty()
                        select new
                        {
                            ProductId = prod.ProductId,
                            Title = prod.Title,
                            Description = prod.Description,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            IsCampaign = prod.IsCampaign,
                            CampaignPercent = prod.CampaignPercent,
                            TempPrice = prod.TempPrice,
                            FkCategoryId = prod.FkCategoryId,
                            CategoryId = cat.CategoryId,
                            CategoryTitle = cat.Title,
                            CategoryDescription = cat.Description,
                            ImageId = img != null ? img.ProductImageId : (int?)null,
                            ImageUrl = img != null ? img.URL : null,
                            ImageFk = img != null ? img.FkProductId : (int?)null
                        };

            var totalResults = await query.CountAsync();
            if (totalResults == 0)
            {
                return TypedResults.NotFound("No products found");
            }

            var paginatedResults = await query
            .OrderBy(p => p.ProductId) // Order by ProductId to ensure consistent results
            .Skip((page - 1) * results)
            .Take(results)
            .ToListAsync();

            if (!paginatedResults.Any())
            {
                return TypedResults.NotFound("No products found for the given page and result quantity");
            }

            var grouped = paginatedResults.GroupBy(x => x.ProductId).Select(grp => new ProductWithImagesDTO
            {
                ProductId = grp.Key,
                Title = grp.First().Title,
                Description = grp.First().Description,
                Price = grp.First().Price,
                Quantity = grp.First().Quantity,
                IsCampaign = grp.First().IsCampaign,
                CampaignPercent = grp.First().CampaignPercent,
                TempPrice = grp.First().TempPrice,
                FkCategoryId = grp.First().FkCategoryId,
                Category = new Category
                {
                    CategoryId = grp.First().CategoryId,
                    Title = grp.First().CategoryTitle,
                    Description = grp.First().CategoryDescription
                },
                Images = grp.Where(x => x.ImageId != null).Select(x => new ProductImage
                {
                    ProductImageId = x.ImageId.Value,
                    URL = x.ImageUrl,
                    FkProductId = x.ImageFk.Value
                }).ToList()
            }).ToList();

            if (!grouped.Any())
            {
                return TypedResults.NotFound("No products found");
            }
            Console.WriteLine(grouped);
            return TypedResults.Ok(grouped);
        }


    }
}
