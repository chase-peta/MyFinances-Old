using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(LoanHistoryMeta))]
    public partial class LoanHistory
    {
        public LoanHistory(Loan loan)
        {
            LoanOutlook outlook = loan.LoanOutlook.FirstOrDefault();
            this.LoanId = loan.Id;
            this.BasicPayment = outlook.BaseAmount;
            this.AddPayment = outlook.AddAmount;
            this.Interest = outlook.InterestAmount;
            this.Escrow = outlook.EscrowAmount;
            this.DatePaid = outlook.Date;
            this.Principal = outlook.Principal;
            this.PaymentTypeId = loan.PaymentTypeId;
            this.Loan = loan;
        }

        public decimal Payment { get { return BasicPayment + AddPayment + Interest + Escrow; } }

        public decimal Principal { get; set; }
    }

    public static class LoanHistoryExentions
    {
        public static IEnumerable<LoanHistory> GetLoanHistory(this Loan loan)
        {
            IEnumerable<LoanHistory> loanHistory = loan.LoanHistories.OrderBy(x => x.DatePaid).ToList();
            foreach (LoanHistory history in loanHistory)
            {
                loan.Principal -= (history.AddPayment + history.BasicPayment);
                history.Principal = loan.Principal;
            }
            return loanHistory;
        }

        public static IEnumerable<LoanHistory> GetAllLoanHistory (this LinkToDBDataContext context, int year = 0)
        {
            if (year > 0)
            {
                return context.LoanHistories.Where(x => x.DatePaid.Year == year).OrderBy(x => x.DatePaid).ToList();
            }
            else
            {
                return context.LoanHistories.OrderBy(x => x.DatePaid).ToList();
            }
        }

        public static LoanHistory GetLoanHistoryItem (this LinkToDBDataContext context, int id)
        {
            return context.LoanHistories.FirstOrDefault(x => x.Id == id);
        }
    }

    public class LoanHistoryMeta
    {
        /* Not In Database */
        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Payment { get; set; }

        [Display(Name = "Principal"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Principal { get; set; }

        /* In Database */
        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public object AddPayment { get; set; }

        [Display(Name = "Base"), DisplayFormat(DataFormatString = "{0:c}")]
        public object BasicPayment { get; set; }

        [Display(Name = "Date Paid"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public object DatePaid { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Escrow { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Interest { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }

        public object LoanId { get; set; }
    }
}