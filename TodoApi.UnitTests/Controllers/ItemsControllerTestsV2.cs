using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.UnitTests.Controllers
{
    public class ItemsControllerTestsV2 : IDisposable
    {
        private readonly TodoDbContext _context;

        public ItemsControllerTestsV2()
        {
            // By supplying a new service provider for each context, 
            // we have a single database instance per test.
            var serviceProvider = new ServiceCollection()
               .AddEntityFrameworkInMemoryDatabase()
               .BuildServiceProvider();

            // Build context options
            var builder = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            // Instantiate the context
            _context = new TodoDbContext(builder.Options);

            // Seed the database
            _context.Items.AddRange(
                Enumerable.Range(1, 10).Select(t => new Item { Description = "Item " + t })
            );

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll_ReturnsItems()
        {
            // Arrange
            var controller = new ItemsController(_context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var model = Assert.IsAssignableFrom<IEnumerable<Item>>(result);
            Assert.Equal(10, model.Count());
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_GivenInvalidId()
        {
            var controller = new ItemsController(_context);

            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsItem_GivenValidId()
        {
            var controller = new ItemsController(_context);

            var result = await controller.GetById(2);

            var objectResult = Assert.IsType<ObjectResult>(result);
            var task = Assert.IsAssignableFrom<Item>(objectResult.Value);
            Assert.Equal("Item 2", task.Description);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenNullItem()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Create(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var controller = new ItemsController(_context);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.Create(new Item());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsNewlyCreatedTodoItem()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Create(new Item { Description = "This is a new task" });

            Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Update(99, new Item { Id = 1, Description = "Task 1" });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequestWhenItemIsInvalid()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Update(1, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var controller = new ItemsController(_context);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.Update(1, new Item { Id = 1, Description = null });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdIsInvalid()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Update(99, new Item { Id = 99, Description = "Task 99" });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenItemUpdated()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Update(1, new Item { Id = 1, Description = "Task 1", Done = true });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIsIsInvalid()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenItemDeleted()
        {
            var controller = new ItemsController(_context);

            var result = await controller.Delete(2);

            Assert.IsType<NoContentResult>(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
