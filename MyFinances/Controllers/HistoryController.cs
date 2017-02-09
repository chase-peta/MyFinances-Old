using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class HistoryController : Controller
    {
        public ActionResult Index(int year = 0)
        {
            year = (year == 0) ? DateTime.Now.Year : year;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                IEnumerable<BillHistory> billHistories = context.GetAllBillHistory(year);
                IEnumerable<LoanHistory> loanHistories = context.GetAllLoanHistory(year);

                DateTime billStartDate = billHistories.Min(x => x.DatePaid);
                DateTime loanStartDate = loanHistories.Min(x => x.DatePaid);
                int startMonth = (billStartDate < loanStartDate) ? billStartDate.Month : loanStartDate.Month;

                DateTime billEndDate = billHistories.Max(x => x.DatePaid);
                DateTime loanEndDate = loanHistories.Max(x => x.DatePaid);
                int endMonth = (billEndDate > loanEndDate) ? billEndDate.Month : loanEndDate.Month;

                List<DashboardDateRange> dateRanges = new List<DashboardDateRange>();
                for (var month = startMonth; month <= endMonth; month++)
                {
                    dateRanges.Add(new DashboardDateRange(
                        new DateTime(year, month, 1),
                        new DateTime(year, month, DateTime.DaysInMonth(year, month))
                    ));
                }

                foreach (DashboardDateRange range in dateRanges)
                {
                    range.Items = billHistories.Where(x => x.DatePaid >= range.StartDate && x.DatePaid <= range.EndDate).Select(x => new DashboardItem(x)).ToList();
                    range.Items = range.Items.Concat(loanHistories.Where(x => x.DatePaid >= range.StartDate && x.DatePaid <= range.EndDate).Select(x => new DashboardItem(x)).ToList());
                    range.Items = range.Items.OrderBy(x => x.Date);
                }

                DashboardViewModel viewModel = new DashboardViewModel();
                viewModel.DateRanges = dateRanges.OrderBy(x => x.StartDate).Reverse();
                viewModel.CurrentYear = year;

                int yearBill = context.BillHistories.Max(x => x.DatePaid).Year;
                int yearLoan = context.LoanHistories.Max(x => x.DatePaid).Year;
                viewModel.EndYear = (yearBill > yearLoan) ? yearBill : yearLoan;

                yearBill = context.BillHistories.Min(x => x.DatePaid).Year;
                yearLoan = context.LoanHistories.Min(x => x.DatePaid).Year;
                viewModel.StartYear = (yearBill < yearLoan) ? yearBill : yearLoan;

                return View(viewModel);
            }
        }
    }
}