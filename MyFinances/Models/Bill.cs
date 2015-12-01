using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(BillMeta))]
    public partial class Bill
    {
        public DateTime LastPaidDate { get; set; }

        public decimal LastPaidAmount { get; set; }

        public bool HistoryDiffPayee { get; set; }

        public decimal AveragePaid { get; set; }

        public decimal MinPaid { get; set; }

        public decimal MaxPaid { get; set; }

        public int AverageDay { get; set; }

        public int MinYear { get; set; }

        public int MaxYear { get; set; }

        public IEnumerable<BillHistory> BillHistory { get; set; }

        public IEnumerable<BillHistoryAverage> BillHistoryAverage { get; set; }

        public bool IsPastDue { get { return DueInDays < 0 && ((DueDate - LastPaidDate).TotalDays >= 5 || (DueDate - LastPaidDate).TotalDays <= -5); } }

        public double DueInDays { get { return (DueDate - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays; } }
        
        public string DueIn
        {
            get
            {
                if (IsPastDue)
                    return "Past Due";
                else if (DueInDays < 0)
                    return "Paid";
                else if (DueInDays == 0)
                    return "Today";
                else if (DueInDays == 1)
                    return "Tomorrow";
                else
                    return DueInDays.ToString() + " Days";
            }
        }

        public string Classes
        {
            get
            {
                if (IsPastDue)
                    return "past-due";
                else if (DueInDays < 0)
                    return "paid";
                else if (DueInDays < 5)
                    return "due-soon";
                else
                    return "";
            }
        }
    }

    public static class BillExentions
    {
        public static IEnumerable<Bill> GetBills(this LinkToDBDataContext context)
        {
            IEnumerable<Bill> bills = context.Bills.Where(x => x.IsActive == true).OrderBy(x => x.DueDate).ToList();
            foreach (Bill bill in bills)
            {
                bill.LoadBill();
            }
            return bills;
        }

        public static Bill GetBill(this LinkToDBDataContext context, int id)
        {
            Bill bill = context.Bills.FirstOrDefault(x => x.Id == id).LoadBill();
            if (bill.BillHistories.Any())
            {
                List<BillHistoryAverage> avgList = new List<BillHistoryAverage>();
                for (int i = 1; i <= 12; i++)
                {
                    IEnumerable<BillHistory> perMonth = bill.BillHistory.Where(x => x.DatePaid.Month == i);
                    if (perMonth.Any())
                    {
                        BillHistoryAverage bha = new BillHistoryAverage(
                            new DateTime((i < bill.DueDate.Month) ? bill.DueDate.Year + 1 : bill.DueDate.Year, i, 1),
                            perMonth.Average(x => x.Amount),
                            perMonth.Min(x => x.Amount),
                            perMonth.Max(x => x.Amount),
                            perMonth.Average(x => x.DatePaid.Day)
                        );
                        avgList.Add(bha);
                    }
                }
                bill.BillHistoryAverage = avgList.OrderBy(x => x.Month);
            }
            return bill;
        }

        private static Bill LoadBill(this Bill bill)
        {
            if (bill.BillHistories.Any())
            {
                bill.BillHistory = bill.GetBillHistory();
                bill.LastPaidDate = bill.BillHistory.OrderBy(x => x.DatePaid).Reverse().Select(x => x.DatePaid).FirstOrDefault();
                bill.LastPaidAmount = bill.BillHistory.OrderBy(x => x.DatePaid).Reverse().Select(x => x.Amount).FirstOrDefault();
                bill.HistoryDiffPayee = bill.BillHistory.Any(x => x.Payee != bill.Payee);
                bill.AveragePaid = bill.BillHistory.Average(x => x.Amount);
                bill.MinPaid = bill.BillHistory.Min(x => x.Amount);
                bill.MaxPaid = bill.BillHistory.Max(x => x.Amount);
                bill.AverageDay = Convert.ToInt16(Math.Ceiling(bill.BillHistory.Average(x => x.DatePaid.Day)));
                bill.MinYear = bill.BillHistory.Min(x => x.DatePaid.Year);
                bill.MaxYear = bill.BillHistory.Max(x => x.DatePaid.Year);
            } else {
                bill.MinYear = DateTime.Now.Year;
                bill.MaxYear = bill.MinYear;
            }
            return bill;
        }
    }

    public class BillMeta
    {
        /* Not In Database */
        [Display(Name = "Past Due")]
        public object IsPastDue { get; set; }

        [Display(Name = "Last Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public object LastPaidDate { get; set; }

        [Display(Name = "Last Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LastPaidAmount { get; set; }

        [Display(Name = "Due In"), DisplayFormat(DataFormatString = "{0} Days")]
        public object DueInDays { get; set; }

        [Display(Name = "Overal"), DisplayFormat(DataFormatString = "{0:c}")]
        public object AveragePaid { get; set; }

        [Display(Name = "Minimum"), DisplayFormat(DataFormatString = "{0:c}")]
        public object MinPaid { get; set; }

        [Display(Name = "Maximum"), DisplayFormat(DataFormatString = "{0:c}")]
        public object MaxPaid { get; set; }

        /* In Database */
        [Display(Name = "Name"), Required]
        public object Name { get; set; }

        [Display(Name = "Due Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true),  DataType(DataType.Date)]
        public object DueDate { get; set; }

        [Display(Name = "Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Amount { get; set; }

        [Display(Name = "Payee"), Required]
        public object Payee { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        [Display(Name = "Stays Same")]
        public object StaysSame { get; set; }

        [Display(Name = "Is Active")]
        public object IsActive { get; set; }

        [Display(Name = "Issue Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public object IssueDate { get; set; }

        [Display(Name = "Shared")]
        public object Shared { get; set; }
    }
}