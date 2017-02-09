using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinances.Models
{
    public class DashboardItem
    {
        public DashboardItem () { }

        public DashboardItem (Bill bill)
        {
            Id = bill.Id;
            Name = bill.Name;
            Date = bill.DueDate;
            Amount = bill.Amount;
            IsShared = bill.Shared;
            Type = "Bill";
            IsPaid = false;
        }

        public DashboardItem (BillHistory history)
        {
            Id = history.BillId;
            Name = history.Bill.Name;
            Date = history.DatePaid;
            Amount = history.Amount;
            IsShared = history.Bill.Shared;
            Type = "Bill";
            IsPaid = true;
        }

        public DashboardItem (BillHistoryAverage bha)
        {
            Date = bha.Month;
            Amount = bha.Average;
            IsShared = false;
            Type = "Bill";
            IsPaid = false;
        }

        public DashboardItem (LoanHistory history)
        {
            Id = history.LoanId;
            Name = history.Loan.Name;
            Date = history.DatePaid;
            Amount = history.Payment;
            IsShared = false;
            Type = "Loan";
            IsPaid = true;
        }

        public DashboardItem (LoanOutlook loanOutlook)
        {
            Date = loanOutlook.Date;
            Amount = loanOutlook.AddAmount + loanOutlook.BaseAmount + loanOutlook.EscrowAmount + loanOutlook.InterestAmount;
            IsShared = false;
            Type = "Loan";
            IsPaid = false;
        }

        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Date"), DisplayFormat(DataFormatString = "{0:MMM. dd}")]
        public DateTime Date { get; set; }

        [Display(Name = "Amount"), DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Amount { get; set; }

        [Display(Name = "Due In"), DisplayFormat(DataFormatString = "{0} Days")]
        public double DueInDays { get { return (Date - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays; } }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }

        [Display(Name = "Due In")]
        public string DueIn
        {
            get
            {
                if (IsPaid || (DueInDays < 0 && !IsPastDue))
                    return "Paid";
                else if (IsPastDue)
                    return "Past Due";
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
                if (IsPaid || (DueInDays < 0 && !IsPastDue))
                    return "paid";
                else if (IsPastDue)
                    return "past-due";
                else if (DueInDays < 5)
                    return "due-soon";
                else
                    return "";
            }
        }

        public bool IsPastDue { get { return DueInDays < 0; } }

        public bool IsShared { get; set; }

        public string Type { get; set; }
    }
}