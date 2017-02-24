using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class BillsController : Controller
    {
        /* =========================
         * Bill Functions
         * ========================= */
        public ActionResult Index(bool showInactive = false)
        {
            ViewBag.ShowInactive = showInactive;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetBills(!showInactive).OrderBy(x => x.DueDate));
            }
        }

        public ActionResult View(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "View";
                return View(context.GetBill(id));
            }
        }

        public ActionResult Add()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "Add";
                return View("View");
            }
        }

        [HttpPost]
        public ActionResult Add(Bill collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                try
                {
                    Bill bill = new Bill();
                    bill.CreationDate = DateTime.Now;
                    bill.ModifyDate = DateTime.Now;
                    bill.Version = 1;
                    bill.UserId = 1;
                    bill.PaymentTypeId = 4;
                    bill.IsActive = true;

                    bill.Amount = collection.Amount;
                    bill.DueDate = collection.DueDate;
                    bill.IssueDate = collection.IssueDate;
                    bill.Name = collection.Name;
                    bill.Payee = collection.Payee;
                    bill.Shared = collection.Shared;
                    bill.StaysSame = collection.StaysSame;

                    context.Bills.InsertOnSubmit(bill);
                    context.SubmitChanges();
                    
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Action = "Add";
                    return View("View", collection);
                }
            }
        }

        public ActionResult Edit(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "Edit";
                return View("View", context.GetBill(id));
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Bill collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                Bill bill = context.GetBill(id);

                try
                {
                    bill.ModifyDate = DateTime.Now;
                    bill.Version += 1;
                    bill.Name = collection.Name;
                    bill.Payee = collection.Payee;
                    bill.DueDate = collection.DueDate;
                    bill.Amount = collection.Amount;
                    bill.IssueDate = collection.IssueDate;
                    bill.StaysSame = collection.StaysSame;
                    bill.Shared = collection.Shared;

                    context.SubmitChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Action = "Edit";
                    return View("View", bill);
                }
            }
        }

        /* =========================
         * Bill History Functions
         * ========================= */
        public ActionResult AddPayment(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                Bill bill = context.GetBill(id);

                BillHistory history = new BillHistory();
                history.Amount = bill.Amount;
                history.DatePaid = bill.DueDate;
                history.Payee = bill.Payee;
                history.PaymentTypeId = bill.PaymentTypeId;
                history.IssueDate = bill.IssueDate;
                history.Bill = bill;

                return View(history);
            }
        }

        [HttpPost]
        public ActionResult AddPayment(BillHistory collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                try
                {
                    BillHistory history = new BillHistory();
                    history.CreationDate = DateTime.Now;
                    history.ModifyDate = DateTime.Now;
                    history.Version = 1;
                    history.BillId = collection.Id;

                    Bill bill = context.GetBill(history.BillId);
                    history.Bill = bill;

                    history.Amount = collection.Amount;
                    history.DatePaid = collection.DatePaid;
                    history.Payee = collection.Payee;
                    history.PaymentTypeId = collection.PaymentTypeId;
                    history.IssueDate = collection.IssueDate;
                    
                    bill.BillHistories.Add(history);

                    if (bill.StaysSame || bill.BillHistoryAverage == null)
                    {
                        bill.DueDate = bill.DueDate.AddMonths(1);
                    }
                    else
                    {
                        IEnumerable<BillHistoryAverage> bha = bill.BillHistoryAverage.Where(x => x.Month.Month == bill.DueDate.AddMonths(1).Month);
                        if (bha.Any())
                        {
                            bill.DueDate = bha.FirstOrDefault().Month;
                            bill.Amount = bha.FirstOrDefault().Average;
                        }
                        else
                        {
                            bill.DueDate = bill.DueDate.AddMonths(1);
                        }
                    }

                    context.SubmitChanges();

                    return RedirectToAction("View", new { id = history.BillId });
                }
                catch
                {
                    return View(collection);
                }
            }
        }

        public ActionResult EditPayment(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                BillHistory history = context.GetBillHistoryItem(id);
                history.Bill = context.GetBill(history.BillId);
                return View(history);
            }
        }

        [HttpPost]
        public ActionResult EditPayment(BillHistory collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                BillHistory history = context.GetBillHistoryItem(collection.Id);

                try
                {
                    history.ModifyDate = DateTime.Now;
                    history.Version += 1;
                    history.DatePaid = collection.DatePaid;
                    history.Amount = collection.Amount;
                    history.Payee = collection.Payee;
                    history.IssueDate = collection.IssueDate;
                    
                    context.SubmitChanges();

                    return RedirectToAction("View", new { id = history.BillId });
                }
                catch
                {
                    return View(history);
                }
            }
        }

        public ActionResult Paid (int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                BillHistory history = new BillHistory();
                history.CreationDate = DateTime.Now;
                history.ModifyDate = DateTime.Now;
                history.Version = 1;
                history.BillId = id;

                Bill bill = context.GetBill(id);

                history.Amount = bill.Amount;
                history.DatePaid = bill.DueDate;
                history.Payee = bill.Payee;
                history.PaymentTypeId = bill.PaymentTypeId;
                history.IssueDate = bill.IssueDate;
                history.Bill = bill;

                if (bill.StaysSame || bill.BillHistoryAverage == null)
                {
                    bill.DueDate = bill.DueDate.AddMonths(1);
                }
                else
                {
                    DateTime useDate = bill.DueDate;
                    IEnumerable<BillHistoryAverage> bha = bill.BillHistoryAverage.Where(x => x.Month.Month == useDate.AddMonths(1).Month);
                    if (bha.Any())
                    {
                        bill.DueDate = bha.FirstOrDefault().Month;
                        bill.Amount = bha.FirstOrDefault().Average;
                    }
                    else
                    {
                        bill.DueDate = bill.DueDate.AddMonths(1);
                    }
                }

                context.SubmitChanges();
                return RedirectToAction("Index", "Home", null);
            }
        }
	}
}