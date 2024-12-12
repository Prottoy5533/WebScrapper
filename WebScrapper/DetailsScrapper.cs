using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class DetailsScrapper
    {
        private readonly HttpClient _client;

        public DetailsScrapper()
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

            var worldNode = document.DocumentNode.SelectSingleNode("//nav//ul//li//a[text()='World']");
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

            var newsNodes = document.DocumentNode.SelectNodes("//div[@class='Cat-lead-wrapper']/a[@href] | //div[@class='rm-container align-items-stretch']/a[@href]");
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

        //public string ExtractFullNews(string newsUrl)
        //{
        //    string htmlContent = GetHtmlContent(newsUrl);
        //    HtmlDocument document = new HtmlDocument();
        //    document.LoadHtml(htmlContent);

        //    var titleNode = document.DocumentNode.SelectSingleNode("//div[@class='d-flex live']//h1");
        //    var contentNodes = document.DocumentNode.SelectNodes("//div[@class='text-wrapper']/p");

        //    string fullNews = "";

        //    if (titleNode != null)
        //    {
        //        fullNews += $"Title: {titleNode.InnerText.Trim()}" + Environment.NewLine;
        //    }

        //    if (contentNodes != null)
        //    {
        //        foreach (var node in contentNodes)
        //        {
        //            fullNews += node.InnerText.Trim() + Environment.NewLine + Environment.NewLine;
        //        }
        //    }

        //    return fullNews;
        //}

        public string ExtractFullNews(string newsUrl)
        {
            string htmlContent = GetHtmlContent(newsUrl);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var titleNode = document.DocumentNode.SelectSingleNode("//div[@class='d-flex live']//h1");
            var contentNodes = document.DocumentNode.SelectNodes("//p | //span");

            string fullNews = "";

            if (titleNode != null)
            {
                fullNews += $"Title: {titleNode.InnerText.Trim()}" + Environment.NewLine + Environment.NewLine;
            }

            if (contentNodes != null)
            {
                foreach (var node in contentNodes)
                {
                    fullNews += node.InnerText.Trim() + Environment.NewLine + Environment.NewLine;
                }
            }

            return fullNews;
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
