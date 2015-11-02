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
        public ActionResult Index()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetBills().Where(x => x.IsActive == true && x.DueInDays <= 45).OrderBy(x => x.DueDate));
            }
        }

        public ActionResult Edit(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetBill(id));
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
                    return View(bill);
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
                    history.Amount = collection.Amount;
                    history.DatePaid = collection.DatePaid;
                    history.Payee = collection.Payee;
                    history.PaymentTypeId = collection.PaymentTypeId;
                    history.IssueDate = collection.IssueDate;

                    Bill bill = context.GetBill(history.BillId);
                    bill.BillHistories.Add(history);

                    if (bill.StaysSame)
                    {
                        bill.DueDate = bill.DueDate.AddMonths(1);
                    }
                    else
                    {
                        BillHistoryAverage bha = bill.BillHistoryAverage.FirstOrDefault(x => x.Month.Month == bill.DueDate.AddMonths(1).Month);
                        bill.DueDate = bha.Month;
                        bill.Amount = bha.Average;
                    }

                    context.SubmitChanges();

                    return RedirectToAction("Edit", new { id = history.BillId });
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

                    return RedirectToAction("Edit", new { id = history.BillId });
                }
                catch
                {
                    return View(history);
                }
            }
        }
	}
}