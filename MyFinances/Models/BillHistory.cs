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
            return bill.BillHistories.OrderBy(x => x.DatePaid).Reverse().ToList();
        }

        public static IEnumerable<BillHistory> GetAllBillHistory(this LinkToDBDataContext context, int year = 0)
        {
            if (year > 0)
            {
                return context.BillHistories.Where(x => x.DatePaid.Year == year).OrderBy(x => x.DatePaid).ToList();
            } else
            {
                return context.BillHistories.OrderBy(x => x.DatePaid).ToList();
            }
        }

        public static BillHistory GetBillHistoryItem(this LinkToDBDataContext context, int id)
        {
            return context.BillHistories.FirstOrDefault(x => x.Id == id);
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