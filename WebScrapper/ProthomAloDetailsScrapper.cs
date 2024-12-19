using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class ProthomAloDetailsScrapper
    {
        private readonly HttpClient _client;

        public ProthomAloDetailsScrapper()
        {
            _client = new HttpClient();
            // Adding a user agent header
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
        }

        public string GetHtmlContent(string url)
        {
            HttpResponseMessage response = _client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public string ExtractWorldCategoryLink(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var worldNode = document.DocumentNode.SelectSingleNode("//li[@class='menu-item _1G1J7']//div//a[text()='politics']");
            
            if (worldNode != null)
            {
                string href = worldNode.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    return "https://bdnews24.com" + href;
                }
            }

            return null;
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
            var contentNodes = document.DocumentNode.SelectNodes("//p[not(@class='author') and not(@class='pub') and not(ancestor::div[@class='HeaderTopDate']) and not(ancestor::div[@class='pub-up print-section d-lg-flex']) and not(child::span[@class='author'])] | //span[not(@class='author') and not(@class='pub') and not(ancestor::div[@class='HeaderTopDate']) and not(ancestor::div[@class='pub-up print-section d-lg-flex'])]");

            StringBuilder fullNews = new StringBuilder();

            if (titleNode != null)
            {
                fullNews.AppendLine($"Title: {HtmlEntity.DeEntitize(titleNode.InnerText.Trim())}");
                fullNews.AppendLine();
            }

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
                    }
                }
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
