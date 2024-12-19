using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class NewsContentService :INewsContentService
    {
        private readonly WebScrapperContext _context;

        public NewsContentService(WebScrapperContext context)
        {
            _context = context;
        }

        public void AddNewsContent(NewsContent newsContent)
        {
            _context.NewsContent.Add(newsContent);
            _context.SaveChanges();
        }

        public void AddNewsContents(List<NewsContent> newsContents)
        {
            _context.NewsContent.AddRange(newsContents);
            _context.SaveChanges();
        }


        public void AddNewsContentCategoryWise(NewsContentCategoryWise newsContent)
        {
            _context.NewsContentCategoryWise.Add(newsContent);
            _context.SaveChanges();
        }
        
    }
}
