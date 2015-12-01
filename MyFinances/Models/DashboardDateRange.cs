using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    public class DashboardDateRange
    {
        public DashboardDateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        [Display(Name = "Start Date"), DisplayFormat(DataFormatString = "{0:MMMM dd}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date"), DisplayFormat(DataFormatString = "{0:dd}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Total"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Total
        {
            get
            {
                decimal total = Convert.ToDecimal(0.0);
                foreach (DashboardItem item in Items)
                {
                    total += item.Amount;
                }
                return total;
            }
        }

        [Display(Name = "To Pay"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal LeftToPay
        {
            get
            {
                decimal total = Convert.ToDecimal(0.0);
                foreach (DashboardItem item in Items)
                {
                    if (!item.IsPaid)
                    {
                        total += item.Amount;
                    }
                }
                return total;
            }
        }

        public IEnumerable<DashboardItem> Items { get; set; }
    }
}