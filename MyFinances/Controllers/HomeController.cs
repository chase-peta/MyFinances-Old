using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                DashboardViewModel viewModel = new DashboardViewModel();
                IEnumerable<Bill> bills = context.GetBills();
                IEnumerable<Loan> loans = context.GetLoans();

                DateTime startDate = DateTime.Now;
                foreach (Bill bill in bills) { startDate = (bill.DueDate <= startDate) ? bill.DueDate : startDate; }
                foreach (Loan loan in loans) { startDate = (loan.DueDate <= startDate) ? loan.DueDate : startDate; }
                if (startDate.Day > 1)
                {
                    //startDate = startDate.AddMonths(-1);
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                }

                viewModel.DateRanges = InitiateDateRanges(startDate);

                foreach (DashboardDateRange range in viewModel.DateRanges)
                {
                    List<DashboardItem> items = new List<DashboardItem>();

                    foreach (Bill bill in bills) {
                        if (bill.DueDate >= range.StartDate && bill.DueDate <= range.EndDate) {
                            items.Add(new DashboardItem(bill));
                        }
                        if (bill.StaysSame || !bill.BillHistories.Any())
                        {
                            for (int i = 1; i <= 3; i++)
                            {
                                DateTime date = bill.DueDate.AddMonths(i);
                                if (date >= range.StartDate && date <= range.EndDate)
                                {
                                    DashboardItem item = new DashboardItem(bill);
                                    item.Date = date;
                                    items.Add(item);
                                }
                            }
                        }
                        else
                        {
                            Bill difBill = context.GetBill(bill.Id);
                            foreach (BillHistoryAverage bha in difBill.BillHistoryAverage)
                            {
                                if (bha.Month >= range.StartDate && bha.Month <= range.EndDate && bha.Month.Month != bill.DueDate.Month)
                                {
                                    DashboardItem item = new DashboardItem(bha);
                                    item.Id = bill.Id;
                                    item.Name = bill.Name;
                                    items.Add(item);
                                }
                            }
                        }

                        if (bill.BillHistories.Any())
                        {
                            foreach (BillHistory history in bill.BillHistory)
                            {
                                if (history.DatePaid >= range.StartDate && history.DatePaid <= range.EndDate)
                                {
                                    items.Add(new DashboardItem(history));
                                }
                            }
                        }
                    }

                    foreach (Loan loan in loans)
                    {
                        foreach (LoanOutlook outlook in loan.LoanOutlook)
                        {
                            if (outlook.Date >= range.StartDate && outlook.Date <= range.EndDate)
                            {
                                DashboardItem dbItem = new DashboardItem(outlook);
                                dbItem.Name = loan.Name;
                                dbItem.Id = loan.Id;
                                items.Add(dbItem);
                            }
                        }
                        foreach (LoanHistory history in loan.LoanHistory)
                        {
                            if (history.DatePaid >= range.StartDate && history.DatePaid <= range.EndDate)
                            {
                                items.Add(new DashboardItem(history));
                            }
                        }
                    }

                    range.Items = items.OrderBy(x => x.Date);
                }

                return View(viewModel);
            }
        }

        private DateTime GetEndDate(DateTime startDate)
        {
            return (startDate.Day == 1) ?
                    new DateTime(startDate.Year, startDate.Month, 14) :
                    new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
        }

        private IEnumerable<DashboardDateRange> InitiateDateRanges(DateTime startDate)
        {
            startDate = (startDate.Day > 1 && startDate.Day < 15) ?
                    new DateTime(startDate.Year, startDate.Month, 1) :
                    (startDate.Day > 15) ? new DateTime(startDate.Year, startDate.Month, 15) : startDate;
            DateTime endDate = GetEndDate(startDate);

            List<DashboardDateRange> dateRanges = new List<DashboardDateRange>();
            for (int i = 1; i <= 6; i++)
            {
                dateRanges.Add(new DashboardDateRange(
                    startDate,
                    endDate
                ));

                startDate = endDate.AddDays(1);
                endDate = GetEndDate(startDate);
            }
            return dateRanges.OrderBy(x => x.StartDate);
        }
	}
}