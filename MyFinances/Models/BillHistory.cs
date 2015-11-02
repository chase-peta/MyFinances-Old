using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(BillHistoryMeta))]
    public partial class BillHistory
    {

    }

    public static class BillHistoryExentions
    {
        public static IEnumerable<BillHistory> GetBillHistory(this Bill bill)
        {
            IEnumerable<BillHistory> billHistory = bill.BillHistories.OrderBy(x => x.DatePaid).Reverse().ToList();
            foreach (BillHistory history in billHistory)
            {
                history.LoadBillHistory();
            }
            return billHistory;
        }

        public static BillHistory GetBillHistoryItem(this LinkToDBDataContext context, int id)
        {
            return context.BillHistories.FirstOrDefault(x => x.Id == id);
        }

        private static BillHistory LoadBillHistory(this BillHistory history)
        {
            return history;
        }
    }

    public class BillHistoryMeta
    {
        /* In Database */
        [Display(Name = "Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Amount { get; set; }

        [Display(Name = "Date Paid"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public object DatePaid { get; set; }

        [Display(Name = "Payee")]
        public object Payee { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        [Display(Name = "Issue Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public object IssueDate { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }
    }
}