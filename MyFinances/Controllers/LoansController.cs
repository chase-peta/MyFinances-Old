using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class LoansController : Controller
    {
        public ActionResult Index()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetLoans());
            }
        }

        public ActionResult Edit(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetLoan(id));
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Loan collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
