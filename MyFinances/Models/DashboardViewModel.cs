using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinances.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<DashboardDateRange> DateRanges { get; set; }

        public int CurrentYear { get; set; }

        public int StartYear { get; set; }

        public int EndYear { get; set; }
    }
}