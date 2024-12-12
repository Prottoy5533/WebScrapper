using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class BDNewsScrapper
    {
        private readonly HttpClient _client;

        public BDNewsScrapper()
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

        public string ExtractTextFromHtml(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // Extracting titles and corresponding briefs
            var descNodes = document.DocumentNode.SelectNodes("//div[@class='Desc']");

            if (descNodes == null)
            {
                throw new Exception("Unable to find elements with class 'Desc'.");
            }

            string text = "";
            foreach (var descNode in descNodes)
            {
                var titleNode = descNode.SelectSingleNode(".//h2[@class='Title']");
                var briefNode = descNode.SelectSingleNode(".//div[@class='Brief']/p");

                if (titleNode != null && briefNode != null)
                {
                    text += $"Title: {titleNode.InnerText.Trim()}" + Environment.NewLine;
                    text += $"Brief: {briefNode.InnerText.Trim()}" + Environment.NewLine;
                    text += Environment.NewLine;
                }
            }

            return text;
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
