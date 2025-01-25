
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebScrapper;

Console.WriteLine("Hello, World!");




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
               services.AddScoped<DailyStarDetailsScrapper>();
           })
           .Build();


try
{
    string url = "https://bdnews24.com";
    var scraper = host.Services.GetRequiredService<BdNewsDetailsScrapper>();
    string mainPageHtml = scraper.GetHtmlContent(url);

    List<string> categoryLinks = scraper.ExtractWorldCategoryLink(mainPageHtml);

    string output = "";
    var newsContentService = host.Services.GetRequiredService<INewsContentService>();
    foreach (var categoryLink in categoryLinks)
    {
        var x = categoryLink.Split("/");
        var categoryName = x[3];
        output = "";
        output += "Category: " + categoryName + Environment.NewLine + Environment.NewLine;
        List<string> newsLinks = scraper.ExtractNewsLinks(categoryLink);

        foreach (var newsLink in newsLinks)
        {
            string fullNews = scraper.ExtractFullNewsWithoutDate(newsLink, categoryName);
            output += fullNews + Environment.NewLine + Environment.NewLine;
        }

        var newsContentCategory = new NewsContentCategoryWise
        {
            Category = categoryName,
            FullNews = output,
            NewsPaperName = "BdNews24",
            CreatedDate = DateTime.Now
        };

        newsContentService.AddNewsContentCategoryWise(newsContentCategory);
        string filePath = Path.Combine(directoryPath, $"bdnews24_content_{categoryName}_{DateTime.Now:yyyyMMdd_HHmm}.txt");

        scraper.WriteTextToFile(output, filePath);
        Console.WriteLine($"Web scraping completed for {x[3]}. Content written to " + filePath);
    }


}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}

try
{
    string url = "https://www.thedailystar.net/";
    var scraper = host.Services.GetRequiredService<DailyStarDetailsScrapper>();
    string mainPageHtml = scraper.GetHtmlContent(url);

    List<string> categoryLinks = scraper.ExtractCategoryLink(mainPageHtml);

    string output = "";
    var newsContentService = host.Services.GetRequiredService<INewsContentService>();
    foreach (var categoryLink in categoryLinks)
    {
        var x = categoryLink.Split("/");
        var categoryName = x[3];
        output = "";
        output += "Category: " + categoryName + Environment.NewLine + Environment.NewLine;
        List<string> newsLinks = scraper.ExtractNewsLinks(categoryLink);

        foreach (var newsLink in newsLinks)
        {
            string fullNews = scraper.ExtractFullNewsWithoutDate(newsLink, categoryName);
            output += fullNews + Environment.NewLine + Environment.NewLine;
        }

        var newsContentCategory = new NewsContentCategoryWise
        {
            Category = categoryName,
            FullNews = output,
            NewsPaperName = "BdNews24",
            CreatedDate = DateTime.Now
        };

        newsContentService.AddNewsContentCategoryWise(newsContentCategory);
        string filePath = Path.Combine(directoryPath, $"bdnews24_content_{categoryName}_{DateTime.Now:yyyyMMdd_HHmm}.txt");

        scraper.WriteTextToFile(output, filePath);
        Console.WriteLine($"Web scraping completed for {x[3]}. Content written to " + filePath);
    }


}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}




