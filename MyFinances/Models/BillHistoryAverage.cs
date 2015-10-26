using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    public class BillHistoryAverage
    {
        public BillHistoryAverage(DateTime month, decimal average)
        {
            Month = month;
            Average = average;
        }

        [Display(Name = "Month"), DisplayFormat(DataFormatString = "{0:MMMM}")]
        public DateTime Month { get; set; }

        [Display(Name = "Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Average { get; set; }
    }
}