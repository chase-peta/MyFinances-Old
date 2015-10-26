using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyFinances.Models;

namespace MyFinances.Controllers
{
    public class BillsController : Controller
    {
        public ActionResult Index()
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.Bills.Where(x => x.IsActive == true).OrderBy(x => x.DueDate).ToList().Where(x => x.DueInDays <= 45));
            }
        }

        public ActionResult Edit(int id)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                return View(context.Bills.FirstOrDefault(x => x.Id == id));
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Bill collection)
        {
            using (LinkToDBDataContext context = new LinkToDBDataContext())
            {
                try
                {
                    context.SubmitChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(collection);
                }
            }
        }
	}
}