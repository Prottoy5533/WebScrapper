using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public interface INewsContentService
    {
        void AddNewsContent(NewsContent newsContent);
        void AddNewsContents(List<NewsContent> newsContents);
        void AddNewsContentCategoryWise(NewsContentCategoryWise newsContent);
    }
}
