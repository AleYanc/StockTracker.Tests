using StockTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTracker.Tests.Helpers
{
    internal class FakeProductList
    {
        public List<Product> GetTwentyFakeProductList(int categoryId)
        {
            List<Product> returnTwentyProducts = new List<Product>()
            {
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                },
                    new()
                {
                    Name = "Test",
                    CategoryId = categoryId
                }
            };

            return returnTwentyProducts;
        }
    }
}
