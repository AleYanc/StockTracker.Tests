using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.DTO.PriceLists;
using StockTracker.DTO.Product;
using StockTracker.DTO.Sale;
using StockTracker.DTO.Stock;
using StockTracker.Models;
using StockTracker.Services;

namespace StockTracker.Tests
{
    public class TestBase
    {
        protected DatabaseConnection BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DatabaseConnection>()
                .UseInMemoryDatabase(dbName).Options;

            var dbContext = new DatabaseConnection(options);
            return dbContext;
        }

        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                // Product
                options.CreateMap<Product, GetProductDTO>()
                    .ReverseMap();
                options.CreateMap<PostProductDTO, Product>();
                options.CreateMap<PutProductDTO, Product>();

                // Category
                options.CreateMap<Category, SimpleCategoryDTO>()
                    .ReverseMap();

                // Stock
                options.CreateMap<Stock, GetStockDTO>()
                    .ReverseMap();
                options.CreateMap<Stock, SimpleStockDTO>()
                    .ReverseMap();
                options.CreateMap<PostStockDTO, Stock>();

                // Sale
                options.CreateMap<Sale, GetSalesDTO>()
                    .ReverseMap();
                options.CreateMap<PostSaleDTO, Sale>();

                // PriceList
                options.CreateMap<PriceList, GetPriceListDTO>()
                    .ReverseMap();
                options.CreateMap<PostPriceListDTO, PriceList>();
                options.CreateMap<PutPriceListDTO, PriceList>();

                // Product Code
                options.CreateMap<ProductCode, SimpleCodeDTO>()
                    .ReverseMap();
            });
            return config.CreateMapper();
        }
    }
}
