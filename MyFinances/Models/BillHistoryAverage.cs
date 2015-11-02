using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    public class BillHistoryAverage
    {
        public BillHistoryAverage()
        {

        }

        public BillHistoryAverage(DateTime month, decimal average, decimal minimum, decimal maximum, double averageDay)
        {
            Month = new DateTime(month.Year, month.Month, Convert.ToInt16(Math.Ceiling(averageDay)));
            Average = average;
            Minimum = minimum;
            Maximum = maximum;
        }

        [Display(Name = "Month"), DisplayFormat(DataFormatString = "{0:MMMM - dd}")]
        public DateTime Month { get; set; }

        [Display(Name = "Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Average { get; set; }

        [Display(Name = "Minimum"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Minimum { get; set; }

        [Display(Name = "Maximum"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Maximum { get; set; }
    }
}