using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class BdNewsDetailsScrapper
    {
        private readonly HttpClient _client;
        private readonly INewsContentService _newsContentService;

        public BdNewsDetailsScrapper(INewsContentService newsContentService)
        {
            _client = new HttpClient();
            
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
            _newsContentService = newsContentService;
        }
     

        public string GetHtmlContent(string url)
        {
            HttpResponseMessage response = _client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public List<string> ExtractWorldCategoryLink(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            //var worldNode = document.DocumentNode.SelectSingleNode("//nav//ul//li//a[text()='World']");
            var categoryNodes = document.DocumentNode.SelectNodes("//nav//ul//li//a[@href]");
            //var categoryNodes = document.DocumentNode.SelectNodes("//nav//ul//li//a[text()='World']");
            var categoryLinks = new List<string>();
            //if (worldNode != null)
            //{
            //    string href = worldNode.GetAttributeValue("href", string.Empty);
            //    if (!string.IsNullOrEmpty(href))
            //    {
            //        return "https://bdnews24.com" + href;
            //    }
            //}

            if (categoryNodes != null)
            {
                foreach (var node in categoryNodes)
                {
                    string href = node.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(href) && !href.Contains("archive") && !href.Contains("search") && !href.Contains("hello.bdnews24.com"))
                    {
                        categoryLinks.Add("https://bdnews24.com" + href);
                    }
                }
            }

            return categoryLinks;
        }

        public List<string> ExtractNewsLinks(string categoryUrl)
        {
            string htmlContent = GetHtmlContent(categoryUrl);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var newsNodes = document.DocumentNode.SelectNodes("//div[@class='Cat-lead-wrapper']/a[@href] | //div[@class='Cat-list']/a[@href] | //div[@class='rm-container align-items-stretch']/a[@href] | //div[@class='tab-news']/a[@href]");

            var newsLinks = new List<string>();

            if (newsNodes != null)
            {
                foreach (var node in newsNodes)
                {
                    string href = node.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(href))
                    {
                        newsLinks.Add(href.StartsWith("http") ? href : "https://bdnews24.com" + href);
                    }
                }
            }

            return newsLinks;
        }



        public string ExtractFullNewsWithoutDate(string newsUrl)
        {
            string htmlContent = GetHtmlContent(newsUrl);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            
            var titleNode = document.DocumentNode.SelectSingleNode("//div[@class='d-flex live']//h1");
            var contentNodes = document.DocumentNode.SelectNodes("//p[not(@class='author') and not(@class='pub') and not(ancestor::div[@class='Brief']) and not(ancestor::div[@class='CopyRight']) and not(ancestor::div[@class='HeaderTopDate']) and not(ancestor::div[@class='pub-up print-section d-lg-flex']) and not(child::span[@class='author'])] | //span[not(@class='author') and not(@class='pub') and not(ancestor::div[@class='HeaderTopDate']) and not(ancestor::div[@class='pub-up print-section d-lg-flex'])]");

            StringBuilder fullNews = new StringBuilder();

            if (titleNode != null)
            {
                fullNews.AppendLine($"Title: {HtmlEntity.DeEntitize(titleNode.InnerText.Trim())}");
                fullNews.AppendLine();
            }
            var content = "";
            if (contentNodes != null)
            {
                foreach (var node in contentNodes)
                {
                    if (node.InnerText.Contains("+"))
                    {

                      continue;
                    }
                        
                    string innerText = HtmlEntity.DeEntitize(node.InnerText.Trim());
                    if (!string.IsNullOrEmpty(innerText) && !string.IsNullOrWhiteSpace(innerText))
                    {
                        fullNews.AppendLine(innerText);
                        fullNews.AppendLine();
                        content += innerText;
                    }
                }

                var newsContent = new NewsContent();
                newsContent.Title = titleNode.InnerText.Trim();
                newsContent.Content = content;
                newsContent.Category = "Test";
                newsContent.NewsPaperName = "BDNews24";
                newsContent.CreatedDate = DateTime.Now;

                _newsContentService.AddNewsContent(newsContent);



            }

            return fullNews.ToString();
        }

        public void WriteTextToFile(string text, string filePath)
        {
            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, text);
        }
    }
}
