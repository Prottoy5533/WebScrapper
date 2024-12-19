using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper
{
    public class NewsContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string NewsPaperName { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
