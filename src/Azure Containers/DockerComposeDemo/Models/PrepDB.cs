using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DockerComposeDemo.Models
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<ColorContext>());
            }
        }
        public static void SeedData(ColorContext context)
        {
            System.Console.WriteLine("Appying migrations...");

            context.Database.Migrate();

            if (!context.ColorItems.Any())
            {
                System.Console.WriteLine("Adding data - seeding...");
                context.ColorItems.AddRange(
                    new Color { ColorName = "Red" },
                    new Color { ColorName = "Orange" },
                    new Color { ColorName = "Yellow" },
                    new Color { ColorName = "Green" },
                    new Color { ColorName = "Blue" }
                    );
                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("Already have data - not seeding");
            }
        }
    }
}
