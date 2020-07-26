using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fnews_Web.ViewModels
{
    public class NewsTagView
    {
        public int TagID { get; set; }
        public String TagName { get; set; }
        public bool Checked { get; set; }
    }
}
