using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinances.Models
{
    public class DashboardDateRange
    {
        public DashboardDateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<DashboardItem> Items { get; set; }
    }
}