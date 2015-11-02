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
                viewModel.DateRanges = InitiateDateRanges();

                IEnumerable<Bill> bills = context.GetBills();
                IEnumerable<Loan> loans = context.GetLoans();

                foreach (DashboardDateRange range in viewModel.DateRanges)
                {
                    List<DashboardItem> items = new List<DashboardItem>();

                    IEnumerable<Bill> billList = bills.Where(x => x.DueDate >= range.StartDate && x.DueDate <= range.EndDate);
                    foreach (Bill bill in billList) { items.Add(new DashboardItem(bill)); }

                    IEnumerable<Loan> loanList = loans.Where(x => x.DueDate >= range.StartDate && x.DueDate <= range.EndDate);
                    foreach (Loan loan in loanList) { items.Add(new DashboardItem(loan)); }

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

        private IEnumerable<DashboardDateRange> InitiateDateRanges()
        {
            DateTime startDate = DateTime.Now;
            startDate = (startDate.Day > 1 && startDate.Day < 15) ?
                new DateTime(startDate.Year, startDate.Month, 1) :
                (startDate.Day > 15) ? new DateTime(startDate.Year, startDate.Month, 15) : startDate;
            DateTime endDate = GetEndDate(startDate);

            List<DashboardDateRange> dateRanges = new List<DashboardDateRange>();
            for (int i = 1; i <= 2; i++)
            {
                dateRanges.Add(new DashboardDateRange(
                    startDate,
                    endDate
                ));

                dateRanges.Add(new DashboardDateRange(
                    startDate.AddMonths(1),
                    endDate.AddMonths(1)
                ));

                startDate = endDate.AddDays(1);
                endDate = GetEndDate(startDate);
            }
            return dateRanges.OrderBy(x => x.StartDate);
        }
	}
}