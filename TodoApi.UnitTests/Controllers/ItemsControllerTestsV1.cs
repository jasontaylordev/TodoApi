using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.UnitTests.Controllers
{
    public class ItemsControllerTestsV1
    {
        [Fact]
        public async Task GetAll_ReturnsItems()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAll_ReturnsItems")
                .Options;

            var context = new TodoDbContext(options);
            
            context.Items.AddRange(
                Enumerable.Range(1, 10).Select(t => new Item { Description = "Item " + t })
            );

            context.SaveChanges();
            
            var controller = new ItemsController(context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var model = Assert.IsAssignableFrom<IEnumerable<Item>>(result);
            Assert.Equal(10, model.Count());
        }

        [Fact]
        public async Task Create_ReturnsNewlyCreatedTodoItem()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: "Create_ReturnsNewlyCreatedTodoItem")
                .Options;

            var context = new TodoDbContext(options);

            context.Items.AddRange(
                Enumerable.Range(1, 10).Select(t => new Item { Description = "Item " + t })
            );

            context.SaveChanges();

            var controller = new ItemsController(context);

            // Act
            var result = await controller.Create(new Item { Description = "This is a new task" });

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(11, context.Items.Count());
        }
    }
}
