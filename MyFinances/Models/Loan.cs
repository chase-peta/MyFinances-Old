using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    [MetadataType(typeof(LoanMeta))]
    public partial class Loan
    {
        public decimal Principal { get; set; }

        public DateTime LastPaidDate { get; set; }

        public decimal LastPaidAmount { get; set; }

        public IEnumerable<LoanHistory> LoanHistory { get; set; }

        public IEnumerable<LoanOutlook> LoanOutlook { get; set; }

        public DateTime DueDate { get { return LastPaidDate.AddMonths(1); } }

        public bool IsPastDue { get { return DueInDays < 0 && ((DueDate - LastPaidDate).TotalDays >= 5 || (DueDate - LastPaidDate).TotalDays <= -5); } }

        public int HistoryMinYear { get; set; }

        public int HistoryMaxYear { get; set; }

        public int OutlookMinYear { get; set; }

        public int OutlookMaxYear { get; set; }

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

        public int PaymentsRemaining { get; set; }

        public decimal MonthlyPayment
        {
            get
            {
                return BasePayment + Escrow + AddPayment;
            }
        }

        public decimal BasePayment
        {
            get
            {
                double mp = 0.0;

                if (PaymentInterestRate > 0 || InterestRate > 0)
                {
                    //double rate = 0.001805; // Car Loan
                    //double rate = 0.006663; // Person Loan
                    double rate = 0.0;
                    if (PaymentInterestRate > 0)
                    {
                        rate = (Convert.ToDouble(PaymentInterestRate) / 100) / 12;
                    }
                    else
                    {
                        rate = (Convert.ToDouble(InterestRate) / 100) / 12;
                    }
                    double factor = (rate + (rate / (Math.Pow(rate + 1, Term) - 1)));
                    mp = Convert.ToDouble(LoanAmount) * factor;
                }
                else
                {
                    mp = Convert.ToDouble(LoanAmount) / Convert.ToDouble(Term);
                }
                if (mp > 0)
                {
                    return Convert.ToDecimal(Math.Ceiling(mp * 100) / 100);
                }
                else
                {
                    return 0.0M;
                }
            }
        }

        //Histoory of Loan totals and averages
        public int HistoryNumberOfPayments { get; set; }

        public decimal HistoryPaymentTotal { get; set; }

        public decimal HistoryPaymentAverage { get; set; }

        public decimal HistoryAdditionalTotal { get; set; }

        public decimal HistoryAdditionalAverage { get; set; }

        public decimal HistoryEscrowTotal { get; set; }

        public decimal HistoryEscrowAverage { get; set; }

        public decimal HistoryInterestTotal { get; set; }

        public decimal HistoryInterestAverage { get; set; }

        //Outlook of Loan totals and averages
        public int OutlookNumberOfPayments { get; set; }

        public decimal OutlookPaymentTotal { get; set; }

        public decimal OutlookPaymentAverage { get; set; }

        public decimal OutlookAdditionalTotal { get; set; }

        public decimal OutlookAdditionalAverage { get; set; }

        public decimal OutlookEscrowTotal { get; set; }

        public decimal OutlookEscrowAverage { get; set; }

        public decimal OutlookInterestTotal { get; set; }

        public decimal OutlookInterestAverage { get; set; }

        //Life of Loan totals and averages
        public int LifeNumberOfPayments { get; set; }

        public decimal LifePaymentTotal { get; set; }

        public decimal LifePaymentAverage { get; set; }

        public decimal LifeAdditionalTotal { get; set; }

        public decimal LifeAdditionalAverage { get; set; }

        public decimal LifeEscrowTotal { get; set; }

        public decimal LifeEscrowAverage { get; set; }

        public decimal LifeInterestTotal { get; set; }

        public decimal LifeInterestAverage { get; set; }
    }

    public static class LoanExentions
    {
        public static IEnumerable<Loan> GetLoans(this LinkToDBDataContext context, bool checkIsActive = true)
        {
            IEnumerable<Loan> loans = (checkIsActive) ? context.Loans.Where(x => x.IsActive == true).ToList() : context.Loans.ToList();
            foreach (Loan loan in loans)
            {
                loan.LoadLoan();
            }
            return loans;
        }

        public static Loan GetLoan(this LinkToDBDataContext context, int id)
        {
            Loan loan = context.Loans.FirstOrDefault(x => x.Id == id).LoadLoan();
            return loan;
        }

        public static Loan LoadLoan(this Loan loan)
        {
            loan.Principal = loan.LoanAmount;
            loan.LastPaidDate = loan.FirstPaymentDate;
            loan.LoanHistory = loan.GetLoanHistory();

            LoanHistory lastHistory = loan.LoanHistory.OrderBy(x => x.DatePaid).Reverse().FirstOrDefault();
            if (lastHistory != null)
            {
                loan.LastPaidDate = lastHistory.DatePaid;
                loan.LastPaidAmount = lastHistory.Payment;
                loan.Principal = lastHistory.Principal;
            }
            if (loan.LoanHistory.Count() > 0)
            {
                loan.HistoryMinYear = loan.LoanHistory.Min(x => x.DatePaid.Year);
                loan.HistoryMaxYear = loan.LoanHistory.Max(x => x.DatePaid.Year);

                loan.HistoryNumberOfPayments = loan.LoanHistory.Count();
                loan.HistoryPaymentTotal = loan.LoanHistory.Sum(x => x.Payment);
                loan.HistoryAdditionalTotal = loan.LoanHistory.Sum(x => x.AddPayment);
                loan.HistoryEscrowTotal = loan.LoanHistory.Sum(x => x.Escrow);
                loan.HistoryInterestTotal = loan.LoanHistory.Sum(x => x.Interest);

                loan.HistoryPaymentAverage = loan.LoanHistory.Average(x => x.Payment);
                loan.HistoryAdditionalAverage = loan.LoanHistory.Average(x => x.AddPayment);
                loan.HistoryEscrowAverage = loan.LoanHistory.Average(x => x.Escrow);
                loan.HistoryInterestAverage = loan.LoanHistory.Average(x => x.Interest);
            }

            if (loan.IsActive)
            {
                loan.LoanOutlook = loan.GetLoanOutlook();
                loan.PaymentsRemaining = loan.LoanOutlook.Count();
                loan.OutlookMinYear = loan.LoanOutlook.Min(x => x.Date.Year);
                loan.OutlookMaxYear = loan.LoanOutlook.Max(x => x.Date.Year);

                loan.OutlookNumberOfPayments = loan.LoanOutlook.Count();
                loan.OutlookPaymentTotal = loan.LoanOutlook.Sum(x => x.Payment);
                loan.OutlookAdditionalTotal = loan.LoanOutlook.Sum(x => x.AddAmount);
                loan.OutlookEscrowTotal = loan.LoanOutlook.Sum(x => x.EscrowAmount);
                loan.OutlookInterestTotal = loan.LoanOutlook.Sum(x => x.InterestAmount);

                loan.OutlookPaymentAverage = loan.LoanOutlook.Average(x => x.Payment);
                loan.OutlookAdditionalAverage = loan.LoanOutlook.Average(x => x.AddAmount);
                loan.OutlookEscrowAverage = loan.LoanOutlook.Average(x => x.EscrowAmount);
                loan.OutlookInterestAverage = loan.LoanOutlook.Average(x => x.InterestAmount);

                loan.LifeNumberOfPayments = loan.HistoryNumberOfPayments + loan.OutlookNumberOfPayments;
                loan.LifePaymentTotal = loan.HistoryPaymentTotal + loan.OutlookPaymentTotal;
                loan.LifeAdditionalTotal = loan.HistoryAdditionalTotal + loan.OutlookAdditionalTotal;
                loan.LifeEscrowTotal = loan.HistoryEscrowTotal + loan.OutlookEscrowTotal;
                loan.LifeInterestTotal = loan.HistoryInterestTotal + loan.OutlookInterestTotal;

                loan.LifePaymentAverage = loan.LifePaymentTotal / loan.LifeNumberOfPayments;
                loan.LifeAdditionalAverage = loan.LifeAdditionalTotal / loan.LifeNumberOfPayments;
                loan.LifeEscrowAverage = loan.LifeEscrowTotal / loan.LifeNumberOfPayments;
                loan.LifeInterestAverage = loan.LifeInterestTotal / loan.LifeNumberOfPayments;
            }

            return loan;
        }

        public static IEnumerable<LoanOutlook> GetLoanOutlook(this Loan loan)
        {
            List<LoanOutlook> outlook = new List<LoanOutlook>();
            double principal = Convert.ToDouble(loan.Principal);
            DateTime date = loan.LastPaidDate;

            double inte = Convert.ToDouble(loan.InterestRate);
            if (loan.InterestCompMonthly)
            {
                inte = (inte / 100 / 12);
            }
            else if (loan.InterestCompDaily)
            {
                inte = (inte / 100 / (DateTime.IsLeapYear(loan.LastPaidDate.Year) ? 366 : 365));
            }

            while (principal > 0.00)
            {
                if (loan.InterestCompMonthly)
                {
                    date = date.AddMonths(1);
                    double interest = Math.Round(inte * Convert.ToDouble(principal), 2);

                    double baseAmount = Convert.ToDouble(loan.BasePayment) - interest;
                    baseAmount = (baseAmount > principal) ? principal : baseAmount;
                    principal -= baseAmount;

                    double add = Convert.ToDouble(loan.AddPayment);
                    add = (principal - add > 0) ? add : principal;
                    principal -= add;

                    if (principal <= 10)
                    {
                        add += principal;
                        principal = 0.00;
                    }

                    LoanOutlook item = new LoanOutlook(date, Convert.ToDecimal(interest), Convert.ToDecimal(baseAmount), Convert.ToDecimal(add), loan.Escrow, Convert.ToDecimal(principal));
                    outlook.Add(item);
                }
                else
                {
                    DateTime lastDate = date;
                    date = date.AddMonths(1);
                    date = new DateTime(date.Year, date.Month, loan.FirstPaymentDate.Day);
                    if (date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        date = date.AddDays(2);
                    }
                    else if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        date = date.AddDays(1);
                    }
                    double interest = 0.0;
                    if (lastDate.Year == date.Year)
                    {
                        interest = inte * Convert.ToDouble(principal) * ((date - lastDate).TotalDays);
                    }
                    else
                    {
                        interest = inte * Convert.ToDouble(principal) * ((new DateTime(lastDate.Year, 12, 31) - lastDate).TotalDays + 0.5);
                        inte = (Convert.ToDouble(loan.InterestRate) / 100 / (DateTime.IsLeapYear(date.Year) ? 366 : 365));
                        interest += inte * Convert.ToDouble(principal) * ((date - new DateTime(date.Year, 1, 1)).TotalDays + 0.5);
                    }
                    interest = Math.Floor(interest * 100) / 100;

                    double baseAmount = Convert.ToDouble(loan.BasePayment) - interest;
                    baseAmount = (baseAmount > principal) ? principal : baseAmount;
                    principal -= baseAmount;

                    double add = Convert.ToDouble(loan.AddPayment);
                    add = (principal - add > 0) ? add : principal;
                    principal -= add;

                    if (principal <= 10)
                    {
                        add += principal;
                        principal = 0.00;
                    }

                    LoanOutlook item = new LoanOutlook(date, Convert.ToDecimal(interest), Convert.ToDecimal(baseAmount), Convert.ToDecimal(add), loan.Escrow, Convert.ToDecimal(principal));
                    outlook.Add(item);
                }
            }

            return outlook.OrderBy(x => x.Date);
        }
    }

    public class LoanMeta
    {
        /* Not In Database */
        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object MonthlyPayment { get; set; }

        [Display(Name = "Base Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object BasePayment { get; set; }

        [Display(Name = "Principal"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Principal { get; set; }

        [Display(Name = "Due Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public object DueDate { get; set; }

        [Display(Name = "Past Due")]
        public object IsPastDue { get; set; }

        [Display(Name = "Last Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public object LastPaidDate { get; set; }

        [Display(Name = "Last Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LastPaidAmount { get; set; }

        [Display(Name = "Due In"), DisplayFormat(DataFormatString = "{0} Days")]
        public object DueInDays { get; set; }

        [Display(Name = "Remaining"), DisplayFormat(DataFormatString = "{0} Months")]
        public object PaymentsRemaining { get; set; }

        //History Totals and Averages
        [Display(Name = "Payments")]
        public object HistoryNumberOfPayments { get; set; }

        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryPaymentTotal { get; set; }

        [Display(Name = "Payment Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryPaymentAverage { get; set; }

        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryAdditionalTotal { get; set; }

        [Display(Name = "Additional Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryAdditionalAverage { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryEscrowTotal { get; set; }

        [Display(Name = "Escrow Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryEscrowAverage { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryInterestTotal { get; set; }

        [Display(Name = "Interest Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object HistoryInterestAverage { get; set; }

        //Outlook Totals and Averages
        [Display(Name = "Payments")]
        public object OutlookNumberOfPayments { get; set; }

        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookPaymentTotal { get; set; }

        [Display(Name = "Payment Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookPaymentAverage { get; set; }

        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookAdditionalTotal { get; set; }

        [Display(Name = "Additional Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookAdditionalAverage { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookEscrowTotal { get; set; }

        [Display(Name = "Escrow Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookEscrowAverage { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookInterestTotal { get; set; }

        [Display(Name = "Interest Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object OutlookInterestAverage { get; set; }

        //Life Totals and Averages
        [Display(Name = "Payments")]
        public object LifeNumberOfPayments { get; set; }

        [Display(Name = "Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifePaymentTotal { get; set; }

        [Display(Name = "Payment Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifePaymentAverage { get; set; }

        [Display(Name = "Additional"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeAdditionalTotal { get; set; }

        [Display(Name = "Additional Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeAdditionalAverage { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeEscrowTotal { get; set; }

        [Display(Name = "Escrow Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeEscrowAverage { get; set; }

        [Display(Name = "Interest"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeInterestTotal { get; set; }

        [Display(Name = "Interest Average"), DisplayFormat(DataFormatString = "{0:c}")]
        public object LifeInterestAverage { get; set; }

        /* In Database */
        [Display(Name = "Add Payment"), DisplayFormat(DataFormatString = "{0:c}")]
        public object AddPayment { get; set; }

        [Display(Name = "Escrow"), DisplayFormat(DataFormatString = "{0:c}")]
        public object Escrow { get; set; }

        [Display(Name = "First Payment Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
        public object FirstPaymentDate { get; set; }

        [Display(Name = "Interest Rate"), DisplayFormat(DataFormatString = "{0:0.0##}", ApplyFormatInEditMode = true)]
        public object InterestRate { get; set; }

        [Display(Name = "Payment Interest Rate"), DisplayFormat(DataFormatString = "{0:0.0##}", ApplyFormatInEditMode = true)]
        public object PaymentInterestRate { get; set; }

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

        [Display(Name = "Interest Compound Daily")]
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