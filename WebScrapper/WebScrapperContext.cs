

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebScrapper
{
    public class WebScrapperContext : DbContext
    {
        public WebScrapperContext(DbContextOptions<WebScrapperContext> options) : base(options)
        {
        }
        public DbSet<NewsContent> NewsContent { get; set; }
        public DbSet<NewsContentCategoryWise> NewsContentCategoryWise { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                

                var connectionString = "Server=ABID\\SQLEXPRESS;Database=webscrapper;Trusted_Connection=True;Encrypt=False;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
