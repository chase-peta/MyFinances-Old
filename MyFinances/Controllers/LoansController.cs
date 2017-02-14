﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class LoansController : Controller
    {
        /* =========================
         * Loan Functions
         * ========================= */
        public ActionResult Index()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetLoans().OrderBy(x => x.DueDate));
            }
        }

        public ActionResult View(int id)
        {
            ViewBag.Calculate = false;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "View";
                return View(context.GetLoan(id));
            }
        }

        public ActionResult Add()
        {
            ViewBag.Calculate = false;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "Add";
                return View("View");
            }
        }

        [HttpPost]
        public ActionResult Add(Loan collection, string button)
        {
            ViewBag.Calculate = false;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                Loan loan = new Loan();

                try
                {
                    loan.CreationDate = DateTime.Now;
                    loan.ModifyDate = DateTime.Now;
                    loan.Version = 1;
                    loan.UserId = 1;
                    loan.PaymentTypeId = 4;
                    loan.IsActive = true;

                    loan.Name = collection.Name;
                    loan.FirstPaymentDate = collection.FirstPaymentDate;
                    loan.LoanAmount = collection.LoanAmount;
                    loan.InterestRate = collection.InterestRate;
                    loan.PaymentInterestRate = collection.PaymentInterestRate;
                    loan.Term = collection.Term;
                    loan.AddPayment = collection.AddPayment;
                    loan.Escrow = collection.Escrow;
                    loan.InterestCompDaily = false;
                    loan.InterestCompMonthly = true;

                    context.Loans.InsertOnSubmit(loan);

                    switch (button)
                    {
                        case "Calculate":
                            loan = loan.LoadLoan();
                            ViewBag.Calculate = true;
                            ViewBag.Action = "Add";
                            return View("View", loan);
                        case "Save":
                            context.SubmitChanges();
                            return RedirectToAction("Index");
                        default:
                            ViewBag.Action = "Add";
                            return View("View", loan);
                    }
                }
                catch
                {
                    ViewBag.Action = "Add";
                    return View("View", loan);
                }
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.Calculate = false;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.Action = "Edit";
                return View("View", context.GetLoan(id));
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Loan collection, string button)
        {
            ViewBag.Calculate = false;
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                Loan loan = context.GetLoan(id);

                try
                {
                    loan.ModifyDate = DateTime.Now;
                    loan.Version += 1;
                    loan.Name = collection.Name;
                    loan.FirstPaymentDate = collection.FirstPaymentDate;
                    loan.LoanAmount = collection.LoanAmount;
                    loan.InterestRate = collection.InterestRate;
                    loan.PaymentInterestRate = collection.PaymentInterestRate;
                    loan.Term = collection.Term;
                    loan.AddPayment = collection.AddPayment;
                    loan.Escrow = collection.Escrow;

                    switch (button)
                    {
                        case "Calculate":
                            loan = loan.LoadLoan();
                            ViewBag.Calculate = true;
                            ViewBag.Action = "Edit";
                            return View("View", loan);
                        case "Save":
                            context.SubmitChanges();
                            return RedirectToAction("Index");
                        default:
                            ViewBag.Action = "Edit";
                            return View("View", loan);
                    }
                }
                catch
                {
                    ViewBag.Action = "Edit";
                    return View("View", loan);
                }
            }
        }

        /* =========================
         * Loan History Functions
         * ========================= */
        public ActionResult AddPayment(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                Loan loan = context.GetLoan(id);
                LoanOutlook outlook = loan.LoanOutlook.FirstOrDefault();

                LoanHistory history = new LoanHistory();
                history.LoanId = loan.Id;
                history.BasicPayment = outlook.BaseAmount;
                history.AddPayment = outlook.AddAmount;
                history.Interest = outlook.InterestAmount;
                history.Escrow = outlook.EscrowAmount;
                history.DatePaid = outlook.Date;
                history.PaymentTypeId = loan.PaymentTypeId;
                history.Loan = loan;

                return View(history);
            }
        }

        [HttpPost]
        public ActionResult AddPayment(LoanHistory collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                try
                {
                    LoanHistory history = new LoanHistory();
                    history.CreationDate = DateTime.Now;
                    history.ModifyDate = DateTime.Now;
                    history.Version = 1;
                    history.LoanId = collection.Id;
                    history.DatePaid = collection.DatePaid;
                    history.BasicPayment = collection.BasicPayment;
                    history.AddPayment = collection.AddPayment;
                    history.Interest = collection.Interest;
                    history.Escrow = collection.Escrow;
                    history.PaymentTypeId = collection.PaymentTypeId;

                    Loan loan = context.GetLoan(history.LoanId);
                    loan.LoanHistories.Add(history);

                    context.SubmitChanges();

                    return RedirectToAction("View", new { id = history.LoanId });
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
                LoanHistory history = context.GetLoanHistoryItem(id);
                history.Loan = context.GetLoan(history.LoanId);
                return View(history);
            }
        }

        [HttpPost]
        public ActionResult EditPayment(LoanHistory collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                LoanHistory history = context.GetLoanHistoryItem(collection.Id);

                try
                {
                    history.ModifyDate = DateTime.Now;
                    history.Version += 1;
                    history.DatePaid = collection.DatePaid;
                    history.BasicPayment = collection.BasicPayment;
                    history.AddPayment = collection.AddPayment;
                    history.Interest = collection.Interest;
                    history.Escrow = collection.Escrow;
                    history.PaymentTypeId = collection.PaymentTypeId;

                    context.SubmitChanges();

                    return RedirectToAction("View", new { id = history.LoanId });
                }
                catch
                {
                    return View(history);
                }
            }
        }
    }
}
