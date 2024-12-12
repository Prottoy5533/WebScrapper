// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using WebScrapper;

Console.WriteLine("Hello, World!");


string url = "https://bdnews24.com";
string directoryPath = @"D:\WebScrapperProject\Output";
string filePath = Path.Combine(directoryPath, $"bdnews24_content_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

//try
//{
//    var scraper = new BDNewsScrapper();
//    string htmlContent = scraper.GetHtmlContent(url);
//    string extractedText = scraper.ExtractTextFromHtml(htmlContent);
//    scraper.WriteTextToFile(extractedText, filePath);

//    Console.WriteLine("Web scraping completed. Content written to " + filePath);
//}
//catch (Exception ex)
//{
//    Console.WriteLine("An error occurred: " + ex.Message);
//}

try
{
    var scraper = new DetailsScrapper();
    string mainPageHtml = scraper.GetHtmlContent(url);
    string worldCategoryLink = scraper.ExtractWorldCategoryLink(mainPageHtml);

    if (worldCategoryLink != null)
    {
        string output = "Category: World" + Environment.NewLine + Environment.NewLine;
        List<string> newsLinks = scraper.ExtractNewsLinks(worldCategoryLink);

        foreach (var newsLink in newsLinks)
        {
            string fullNews = scraper.ExtractFullNews(newsLink);
            output += fullNews + Environment.NewLine + Environment.NewLine;
        }

        scraper.WriteTextToFile(output, filePath);
        Console.WriteLine("Web scraping completed. Content written to " + filePath);
    }
    else
    {
        Console.WriteLine("World category link not found.");
    }
}
catch (Exception ex)
{
    Console.WriteLine("An error occurred: " + ex.Message);
}