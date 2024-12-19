using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class NewsContentCategoryWise
    {
        public int Id { get; set; }
        public string FullNews { get; set; }
        public string NewsPaperName { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
