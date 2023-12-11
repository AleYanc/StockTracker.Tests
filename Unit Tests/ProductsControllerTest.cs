using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTracker.Controllers;
using StockTracker.DTO;
using StockTracker.DTO.Product;
using StockTracker.Models;
using StockTracker.Services;
using StockTracker.Tests.Helpers;
using StockTracker.Wrappers;
using System.Text;
using Moq;

namespace StockTracker.Tests.Unit_Tests
{
    [TestClass]
    public class ProductsControllerTest() : TestBase
    {
        [TestMethod]
        public async Task GetProducts()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test"} );

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            context.Products.Add(new()
            {
                Name = "Test",
                CategoryId = category!.Id
            });
            context.Products.Add(new()
            {
                Name = "Test",
                CategoryId = category!.Id
            });

            var mock = new Mock<IUriService>();

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var pagination = new PaginationDTO();

            var controller = new ProductsController(context, mapper, null, mock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.GetProducts(pagination);
            var result = response.Result as ObjectResult;
            var data = result?.Value as PagedResponse<List<GetProductDTO>>;

            Assert.IsNotNull(response);
            Assert.AreEqual(2, data?.Data.Count);
        }

        [TestMethod]
        public async Task GetProducts_Empty()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var pagination = new PaginationDTO();

            var mock = new Mock<IUriService>();

            var controller = new ProductsController(context, mapper, null, mock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.GetProducts(pagination);
            var result = response.Result as ObjectResult;
            var data = result?.Value as PagedResponse<List<GetProductDTO>>;

            Assert.IsNotNull(response);
            Assert.AreEqual(0, data?.Data.Count);
        }

        [TestMethod]
        public async Task GetProducts_WithPagination()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            var fakeProductList = new FakeProductList();
            var products = fakeProductList.GetTwentyFakeProductList(category!.Id);

            foreach (var product in products)
            {
                context.Products.Add(product);
            }

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var pagination = new PaginationDTO()
            {
                PageSize = 10
            };

            var mock = new Mock<IUriService>();

            var controller = new ProductsController(context, mapper, null, mock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.GetProducts(pagination);
            var result = response.Result as ObjectResult;
            var data = result?.Value as PagedResponse<List<GetProductDTO>>;

            Assert.IsNotNull(response);
            Assert.AreEqual(10, data?.Data.Count);
            Assert.AreEqual(20, data?.TotalRecords);
            Assert.AreEqual(2, data?.TotalPages);
        }

        [TestMethod]
        public async Task GetProduct()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            context.Products.Add(new()
            {
                Id = 1,
                Name = "Test",
                CategoryId = category!.Id
            });

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var controller = new ProductsController(context, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            var response = await controller.GetProduct(1);
            var result = response!.Value;

            Assert.IsNotNull(response);
            Assert.AreEqual(1, result?.Id);
        }

        [TestMethod]
        public async Task GetProduct_NonExistent()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new ProductsController(context, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.GetProduct(1);
            var data = response.Value;
            var result = response.Result as StatusCodeResult;

            Assert.AreEqual(404, result?.StatusCode);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public async Task PostProduct_WithoutImage()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            PostProductDTO productDTO = new()
            {
                Name = "Test",
                CategoryId = category!.Id
            };

            var initialCount = await context.Products.CountAsync();

            var controller = new ProductsController(context, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.PostProduct(productDTO);

            var afterPostCount = await context.Products.CountAsync();

            Assert.AreEqual(0, initialCount);
            Assert.AreEqual(1, afterPostCount);

            var product = await context.Products.FirstOrDefaultAsync();
            Assert.IsNotNull(product);
            Assert.AreEqual("0000000001", product.ProductCode!.Code);
        }

        [TestMethod]
        public async Task PostProduct_WithImage()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            var content = Encoding.UTF8.GetBytes("Imagen de prueba");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";

            List<IFormFile> files = new List<IFormFile>() 
            { 
                file
            };

            PostProductDTO productDTO = new()
            {
                Name = "Test",
                CategoryId = category!.Id,
                Images = files
            };

            var mock = new Mock<IFileSaver>();
            mock.Setup(x => x.SaveFile(content, ".jpg", "products", file.ContentType))
                .Returns(Task.FromResult("url"));

            var initialCount = await context.Products.CountAsync();

            var controller = new ProductsController(context, mapper, mock.Object, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.PostProduct(productDTO);

            var afterPostCount = await context.Products.CountAsync();

            Assert.AreEqual(0, initialCount);
            Assert.AreEqual(1, afterPostCount);
        }

        [TestMethod]
        public async Task UpdateProduct()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            context.Products.Add(new()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                CategoryId = category!.Id
            });

            await context.SaveChangesAsync();

            var previousProduct = await context.Products.FirstOrDefaultAsync(x => x.Id == 1);

            var context2 = BuildContext(dbName);

            var controller = new ProductsController(context2, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            PutProductDTO dto = new PutProductDTO()
            {
                Name = "Test 2",
                Description = "Test 2",
            };

            var response = await controller.PutProduct(1, dto);

            var newProduct = await context2.Products.FirstOrDefaultAsync(x => x.Id == 1);

            Assert.AreNotEqual(previousProduct!.Name, newProduct!.Name);
            Assert.AreNotEqual(previousProduct!.Description, newProduct!.Description);
        }

        [TestMethod]
        public async Task UpdateProduct_NonExistent()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            var controller = new ProductsController(context, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            PutProductDTO dto = new PutProductDTO()
            {
                Name = "Test 2",
                Description = "Test 2",
            };

            var response = await controller.PutProduct(1, dto) as StatusCodeResult;

            Assert.AreEqual(404, response!.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProduct()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = ConfigureAutoMapper();

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            context.Products.Add(new()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                CategoryId = category!.Id
            });

            await context.SaveChangesAsync();

            var initialCount = await context.Products.CountAsync();

            var context2 = BuildContext(dbName);

            var controller = new ProductsController(context2, mapper, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.DeleteProduct(1);
            var afterCount = await context2.Products.CountAsync();

            Assert.AreNotEqual(initialCount, afterCount);
            Assert.AreEqual(0, afterCount);
        }

        [TestMethod]
        public async Task DeleteProduct_NonExistent()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var controller = new ProductsController(context, null, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.DeleteProduct(1) as StatusCodeResult;

            Assert.AreEqual(404, response!.StatusCode);
        }

        [TestMethod]
        public async Task GetProductImages()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            string imageUrl = "[\"https://test/image.jpg\", \"https://test/image2.jpg\", \"https://test/image3.jpg\", \"https://test/image4.jpg\"]";

            context.Products.Add(new()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                ImagesUrl = imageUrl,
                CategoryId = category!.Id
            });

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var controller = new ProductsController(context2, null, null, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.GetProductImages(1);
            var result = (ObjectResult) response.Result!;
            var list = (List<string>) result.Value!;

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("https://test/image.jpg", list[0]);
            Assert.AreEqual("https://test/image2.jpg", list[1]);
            Assert.AreEqual("https://test/image3.jpg", list[2]);
            Assert.AreEqual("https://test/image4.jpg", list[3]);
        }

        [TestMethod]
        public async Task DeleteImageFromProduct()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            context.Categories.Add(new Category { Name = "Test" });

            await context.SaveChangesAsync();

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Name == "Test");

            string imageUrl = "[\"https://test/image.jpg\"]";

            context.Products.Add(new()
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
                ImagesUrl = imageUrl,
                CategoryId = category!.Id
            });

            await context.SaveChangesAsync();

            var initialProduct = await context.Products.FindAsync(1);

            var context2 = BuildContext(dbName);

            var mock = new Mock<IFileSaver>();

            var controller = new ProductsController(context2, null, mock.Object, null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var response = await controller.DeleteImageFromProduct("https://test/image.jpg", 1);

            var afterProduct = await context2.Products.FindAsync(1);

            Assert.AreNotEqual(initialProduct!.ImagesUrl, afterProduct!.ImagesUrl);
        }
    }
}
