using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult RenderMenu()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                ViewBag.bills = context.GetBillsForMenu();
                ViewBag.loans = context.GetLoansForMenu();
                
                return PartialView("Menu");
            }
        }
    }
}