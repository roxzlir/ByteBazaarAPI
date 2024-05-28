##### API
https://bytebazaarapi.azurewebsites.net/
##### WEB SHOP
https://bytebazaarclient.azurewebsites.net/

# ByteBazaar API - in use at the ByteBazaar web shop!
## Debug Thug's API application
We received a task to create an web shop and an API service that is meant to be used together. 

##### Technical notes:
We built an API that is published on azure with it's own database. The API contains all products, categories and images related to products. 
The ByteBazaarWeb (see repo https://github.com/ZoranDotNet/BiteBazaarWeb) is also published on azure with it's own database to store all the favorite products, carts, order, account's and everything that is need to give you the ultimate shopping experience.
We have used ASP.Core with C# for this API build. 

### Our endpoint
#### GET requests
/products - All Products
/products/{id:int} - Product by ID
/category/{id:int}/products - Products by Category ID
/products/search/{search} - Search for Product titles
/products/results/{results:int}/page/{page:int} - Products Paginated Result
/categories - All Categories
/categories/{id:int} - Category by ID
/products/images - All Images
/products/images/{id:int} - Image by ID

#### POST requests
/products - Add Product
/categories - Add Category
/products/images - Add Image

#### PUT requests
/products/{id:int} -  Update Product
/categories/{id:int} . Update Category

#### DELETE requests
/products/{id:int} - Delete Product           
/categories/{id:int} - Delete Category
/products/images/{id:int} - Delete Image


#### Thank you for using Debug Thugs Application


#### The Debug Thugs Team:
##### -> Angelica Lindström
##### -> Zoran Matovic
##### -> Emil Nordin
##### -> Theres Sundberg Selin
