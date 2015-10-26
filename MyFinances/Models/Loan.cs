using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(LoanMeta))]
    public partial class Loan
    {
        public decimal MonthlyPayment { get; set; }

        public decimal BaseMonthlyPayment { get; set; }

        public decimal Principal { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsPastDue { get; set; }

        public bool IsDueInTenDays { get; set; }

        public DateTime LastPaidDate { get; set; }

        public decimal LastPaidAmount { get; set; }

        public double DueInDays { get; set; }

        public IEnumerable<LoanHistory> LoanHistory { get; set; }

        public IEnumerable<LoanOutlook> LoanOutlook { get; set; }
    }

    public static class LoanExentions
    {
        public static IEnumerable<Loan> GetLoans(this LinkToDBDataContext context)
        {
            IEnumerable<Loan> loans = context.Loans.Where(x => x.IsActive == true).ToList();
            foreach (Loan loan in loans)
            {
                loan.BaseMonthlyPayment = CalculateMonthlyPayment(loan.LoanAmount, loan.Term, loan.InterestRate) + loan.Escrow;
                loan.MonthlyPayment = loan.BaseMonthlyPayment + loan.AddPayment;
                loan.Principal = loan.LoanAmount;

                loan.LoanHistory = loan.GetLoanHistory();

                loan.LastPaidDate = loan.LoanHistory.OrderBy(x => x.DatePaid).Reverse().Select(x => x.DatePaid).FirstOrDefault();
                loan.LastPaidAmount = loan.LoanHistory.OrderBy(x => x.DatePaid).Reverse().Select(x => x.Amount).FirstOrDefault();
                loan.DueDate = loan.LastPaidDate.AddMonths(1);
                loan.DueInDays = (loan.DueDate - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays;
                loan.IsPastDue = (loan.DueInDays < 0);
                loan.IsDueInTenDays = (loan.DueInDays <= 10 & loan.DueInDays >= 0);
            }
            return loans;
        }

        public static Loan GetLoan(this LinkToDBDataContext context, int id)
        {
            return context.Loans.FirstOrDefault(x => x.Id == id);
        }

        private static decimal CalculateMonthlyPayment(decimal loanAmount, int term, decimal interestRate)
        {
            double mp = 0.0;

            if (interestRate > 0)
            {
                double rate = (Convert.ToDouble(interestRate) / 100) / 12;
                double factor = (rate + (rate / (Math.Pow(rate + 1, term) - 1)));
                mp = Convert.ToDouble(loanAmount) * factor;
            }
            else
            {
                mp = Convert.ToDouble(loanAmount) / Convert.ToDouble(term);
            }

            return Convert.ToDecimal(Math.Ceiling(mp * 100) / 100);
        }

        public static string GetClasses(this Loan loan)
        {
            string classes = "";
            classes = (loan.IsPastDue) ? classes + "past-due " : classes;
            classes = (loan.IsDueInTenDays) ? classes + "due-soon " : classes;
            classes = classes.Trim();

            return (classes != "") ? "style='" + classes + "'" : "";
        }
    }

    public class LoanMeta
    {
        /* Not In Database */
        [Display(Name = "Monthly Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object MonthlyPayment { get; set; }

        [Display(Name = "Base Monthly Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object BaseMonthlyPayment { get; set; }

        [Display(Name = "Principal"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Principal { get; set; }

        [Display(Name = "Due Date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public object DueDate { get; set; }

        [Display(Name = "Past Due")]
        public object IsPastDue { get; set; }

        [Display(Name = "Due In 10 Days")]
        public object IsDueInTenDays { get; set; }

        [Display(Name = "Last Paid Date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public object LastPaidDate { get; set; }

        [Display(Name = "Last Paid Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LastPaidAmount { get; set; }

        [Display(Name = "Due In")]
        public object DueInDays { get; set; }

        /* In Database */
        [Display(Name = "Add Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object AddPayment { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Escrow { get; set; }

        [Display(Name = "First Payment Date"), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public object FirstPaymentDate { get; set; }

        [Display(Name = "Interest Rate"), DisplayFormat(DataFormatString = "{0:c}")]
        public object InterestRate { get; set; }

        [Display(Name = "Loan Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LoanAmount { get; set; }

        [Display(Name = "Name")]
        public object Name { get; set; }

        [Display(Name = "Term")]
        public object Term { get; set; }

        [Display(Name = "Payment Type")]
        public object PaymentTypeId { get; set; }

        [Display(Name = "Is Active")]
        public object IsActive { get; set; }

        [Display(Name = "Interest Compound Monthly")]
        public object InterestCompDaily { get; set; }

        [Display(Name = "Interest Compound Monthly")]
        public object InterestCompMonthly { get; set; }

        /* Not Used In View */
        public object Id { get; set; }

        public object Version { get; set; }

        public object CreationDate { get; set; }

        public object ModifyDate { get; set; }

        public object UserId { get; set; }
    }
}