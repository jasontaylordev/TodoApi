using System.Linq;
using TodoApi.Models;

namespace TodoApi.Data
{
    public static class TodoDbInitialiser
    {
        public static void Initialise(TodoDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Items.Any())
            {
                return;
            }

            var items = new[]
            {
                new Item { Description = "Do this task." },
                new Item { Description = "Do this task too." },
                new Item { Description = "Do this task also." },
                new Item { Description = "You did this task.", Done = true },
            };

            foreach (var item in items)
            {
                context.Items.Add(item);
            }

            context.SaveChanges();
        }
    }
}
