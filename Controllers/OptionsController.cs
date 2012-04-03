using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BootstrapMVC.Models;
using System.Web.Configuration;
using System.IO;
using System.Text;

namespace BootstrapMVC.Controllers
{
    public class OptionsController : Controller
    {
        public ActionResult Index()
        {
			Options model = new Options();

            return View(model);
        }

		public ActionResult Update(Options model)
		{
			TempData["InfoMessage"] = "Options saved";

			return RedirectToAction("Index");
		}
    }
}
