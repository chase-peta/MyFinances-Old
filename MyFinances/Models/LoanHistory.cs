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
        public decimal Amount { get; set; }

        public decimal Principal { get; set; }
    }

    public static class LoanHistoryExentions
    {
        public static IEnumerable<LoanHistory> GetLoanHistory(this Loan loan)
        {
            IEnumerable<LoanHistory> loanHistory = loan.LoanHistories.OrderBy(x => x.DatePaid).Reverse().ToList();
            foreach (LoanHistory history in loanHistory)
            {
                history.Amount = history.AddPayment + history.BasicPayment + history.Escrow + history.Interest;
                loan.Principal -= history.AddPayment - history.BasicPayment;
                history.Principal = loan.Principal;
            }
            return loanHistory;
        }
    }

    public class LoanHistoryMeta
    {
        /* Not In Database */
        [Display(Name = "Amount")]
        public object Amount { get; set; }

        /* In Database */
        [Display(Name = "Add Payment")]
        public object AddPayment { get; set; }

        [Display(Name = "Basic Payment")]
        public object BasicPayment { get; set; }

        [Display(Name = "Date Paid")]
        public object DatePaid { get; set; }

        [Display(Name = "Escrow")]
        public object Escrow { get; set; }

        [Display(Name = "Interest")]
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