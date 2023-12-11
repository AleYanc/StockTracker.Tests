using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTracker.Controllers;
using StockTracker.Models;

namespace StockTracker.Tests.Unit_Tests
{
    [TestClass]
    public class CategoriesControllerTest : TestBase
    {
        [TestMethod]
        public async Task GetCategories()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            context.Categories.Add(new() { Name = "Test 1" });
            context.Categories.Add(new() { Name = "Test 2" });

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var controller = new CategoriesController(context2);
            var response = await controller.GetCategories();

            var genres = response.Value!;
            Assert.AreEqual(2, genres.Count());
        }

        [TestMethod]
        public async Task GetCategoryById_Error()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var controller = new CategoriesController(context);
            var response = await controller.GetCategory(1);

            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task GetCategoryById()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            context.Categories.Add(new Models.Category() { Name = "Test 1" });

            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var controller = new CategoriesController(context2);
            int id = 1;
            var response = await controller.GetCategory(id);

            var genre = response.Value!;

            Assert.AreEqual(id, genre.Id);
        }

        [TestMethod]
        public async Task PostCategory()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            Category category = new Category() { Name = "Test" };
            var controller = new CategoriesController(context);
            var response = await controller.PostCategory(category);
            var result = response.Result as CreatedAtActionResult;
            Assert.IsNotNull(result);

            var context2 = BuildContext(dbName);
            var count = await context2.Categories.CountAsync();

            Assert.AreEqual(1, count);
        }
    }
}
