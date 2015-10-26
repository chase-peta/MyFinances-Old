using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(BillMeta))]
    public partial class Bill
    {
        public void Load() {
            LastPaidDate = BillHistories.OrderBy(x => x.DatePaid).Reverse().Select(x => x.DatePaid).FirstOrDefault();
            LastPaidAmount = BillHistories.OrderBy(x => x.DatePaid).Reverse().Select(x => x.Amount).FirstOrDefault();
            HistoryDiffPayee = BillHistories.Any(x => x.Payee != Payee);

            List<BillHistoryAverage> avgList = new List<BillHistoryAverage>();

            for (int i = 1; i <= 12; i++)
            {
                avgList.Add(new BillHistoryAverage(
                    new DateTime((i < DateTime.Now.Month) ? DateTime.Now.Year + 1 : DateTime.Now.Year, i, 1),
                    BillHistories.Where(x => x.DatePaid.Month == i).Average(x => x.Amount))
                );
            }

            BillHistoryAverage = avgList;
        }

        public bool IsPastDue { get { return DueInDays < 0 && (DueDate - LastPaidDate).TotalDays >= 10; } }

        public DateTime LastPaidDate { get; set; }

        public decimal LastPaidAmount { get; set; }

        public double DueInDays { get { return (DueDate - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays; } }

        public bool HistoryDiffPayee { get; set; }

        public IEnumerable<BillHistoryAverage> BillHistoryAverage { get; set; }
    }

    public class BillMeta
    {
        /* Not In Database */
        [Display(Name = "Past Due")]
        public object IsPastDue { get; set; }

        [Display(Name = "Last Paid Date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public object LastPaidDate { get; set; }

        [Display(Name = "Last Paid Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LastPaidAmount { get; set; }

        [Display(Name = "Due In"), DisplayFormat(DataFormatString = "{0} Days")]
        public object DueInDays { get; set; }

        /* In Database */
        [Display(Name = "Name")]
        public object Name { get; set; }

        [Display(Name = "Due Date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public object DueDate { get; set; }

        [Display(Name = "Amount"), DisplayFormat(DataFormatString="{0:c}")]
        public object Amount { get; set; }

        [Display(Name = "Payee")]
        public object Payee { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        [Display(Name = "Stays Same")]
        public object StaysSame { get; set; }

        [Display(Name = "Is Active")]
        public object IsActive { get; set; }

        [Display(Name = "Issue Date"), DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true)]
        public object IssueDate { get; set; }

        [Display(Name = "Shared")]
        public object Shared { get; set; }
    }
}