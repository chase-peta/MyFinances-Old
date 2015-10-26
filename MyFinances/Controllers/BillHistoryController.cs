using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class BillHistoryController : Controller
    {
        public ActionResult Index(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.Bills.FirstOrDefault(x => x.Id == id).BillHistories);
            }
        }

        public ActionResult Edit(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.GetBillHistoryItem(id));
            }
        }
	}
}