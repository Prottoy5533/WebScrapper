
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebScrapper;

Console.WriteLine("Hello, World!");



string url = "https://bdnews24.com";
//string url = "https://www.prothomalo.com/";
string directoryPath = @"D:\WebScrapper\Output";
//string filePath = Path.Combine(directoryPath, $"bdnews24_content_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
var host = Host.CreateDefaultBuilder(args)
           .ConfigureServices((context, services) =>
           {
               services.AddDbContext<WebScrapperContext>();

               // Register NewsContentService with DI as a singleton or scoped as needed
               services.AddScoped<INewsContentService, NewsContentService>();
               services.AddScoped<BdNewsDetailsScrapper>();
           })
           .Build();


try
{
    var scraper = host.Services.GetRequiredService<BdNewsDetailsScrapper>();
    string mainPageHtml = scraper.GetHtmlContent(url);

    List<string> categoryLinks = scraper.ExtractWorldCategoryLink(mainPageHtml);

    string output = "";

    foreach (var categoryLink in categoryLinks)
    {

        output = "";
        output += "Category: " + categoryLink + Environment.NewLine + Environment.NewLine;
        List<string> newsLinks = scraper.ExtractNewsLinks(categoryLink);

        foreach (var newsLink in newsLinks)
        {

            string fullNews = scraper.ExtractFullNewsWithoutDate(newsLink);
            output += fullNews + Environment.NewLine + Environment.NewLine;
        }
        var x = categoryLink.Split("/");
        string filePath = Path.Combine(directoryPath, $"bdnews24_content_{x[3]}_{DateTime.Now:yyyyMMdd_HHmm}.txt");

        scraper.WriteTextToFile(output, filePath);
        Console.WriteLine($"Web scraping completed for {x[3]}. Content written to " + filePath);
    }


}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}




