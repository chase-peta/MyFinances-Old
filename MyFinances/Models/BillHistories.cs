using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(BillHistoriesMeta))]
    public partial class BillHistories
    {

    }

    public static class BillHistoriesExentions
    {
        public static BillHistory GetBillHistoryItem(this LinkToDBDataContext context, int id)
        {
            return context.BillHistories.FirstOrDefault(x => x.Id == id);
        }
    }

    public class BillHistoriesMeta
    {
        /* In Database */
        [Display(Name = "Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Amount { get; set; }

        [Display(Name = "Date Paid"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public object DatePaid { get; set; }

        [Display(Name = "Payee")]
        public object Payee { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        [Display(Name = "Issue Date")]
        public object IssueDate { get; set; }
    }
}