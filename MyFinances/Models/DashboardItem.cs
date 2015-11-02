using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinances.Models
{
    public class DashboardItem
    {
        public DashboardItem() { }

        public DashboardItem(Bill bill)
        {
            Id = bill.Id;
            Name = bill.Name;
            Date = bill.DueDate;
            Amount = bill.Amount;
            IsPastDue = bill.IsPastDue;
            IsShared = bill.Shared;
        }

        public DashboardItem(Loan loan)
        {
            Id = loan.Id;
            Name = loan.Name;
            Date = loan.DueDate;
            Amount = loan.MonthlyPayment;
            IsPastDue = loan.IsPastDue;
            IsShared = false;
        }

        public DashboardItem(LoanOutlook loanOutlook)
        {
            Date = loanOutlook.Date;
            Amount = loanOutlook.AddAmount + loanOutlook.BaseAmount + loanOutlook.EscrowAmount + loanOutlook.InterestAmount;
            IsPastDue = (Date < DateTime.Now);
            IsShared = false;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public bool IsPastDue { get; set; }

        public bool IsShared { get; set; }

        public double DueInDays { get { return (Date - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalDays; } }
    }
}