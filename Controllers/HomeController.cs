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
    public class HomeController : Controller
    {
		private Bugs _bugsModel = new Bugs();

		public Bugs BugsModel
		{
			get
			{
				if (_bugsModel == null)
					_bugsModel = new Bugs();

				return _bugsModel;
			}
			set { _bugsModel = value; }
		}


		protected string SelectedSprint
		{
			get
			{
				IList<string> sprints = BugsModel.GetAllSprints();

				if (HttpContext.Session["sprint"] == null)
				{
					if (sprints != null && sprints.Count > 0)
					{
						HttpContext.Session["sprint"] = BugsModel.GetAllSprints()[0];
					}
					else
					{
						TempData["ErrorMessage"] = "Nerastas nei vienas sprintas";

						return null;
					}
				}

				ViewBag.Sprints = sprints;

				return (string)HttpContext.Session["sprint"];
			}
			set
			{
				HttpContext.Session["sprint"] = value;
			}
		}

        public ActionResult Index()
        {
			IList<string> sprints = BugsModel.GetAllSprints();
			ViewBag.Sprints = sprints;
			ViewBag.SelectedSprint = SelectedSprint;

            ViewBag.WelcomeMessage = "Sveiki atvykę į BzSprint!";
			ViewBag.BugZillaUrl = WebConfigurationManager.AppSettings["BugzillaUrl"] + "show_bug.cgi?id=";

			if (SelectedSprint != null)
			{
				IList<Bug> bugs = BugsModel.GetBugList(SelectedSprint);
				ViewBag.Bugs = bugs;

				if (bugs == null || bugs.Count == 0)
				{
					TempData["ErrorMessage"] = "Nerastas nei vienas bugas...";
				}
			}

            return View(BugsModel);
        }

        public ActionResult About()
        {
            return View();
        }

		public ActionResult MoveUp(long bugID)
		{
			string infoMessage = string.Format("Bugas #{0} pakeltas per vietą į viršų", bugID);

			TempData["InfoMessage"] = infoMessage;

			BugsModel.MoveBugUp(bugID, BugsModel.GetBugList(SelectedSprint));

			return RedirectToAction("Index");
		}

		public ActionResult MoveDown(long bugID)
		{
			string infoMessage = string.Format("Bugas #{0} pakeltas per vietą į apačią", bugID);

			TempData["InfoMessage"] = infoMessage;

			BugsModel.MoveBugDown(bugID, BugsModel.GetBugList(SelectedSprint));

			return RedirectToAction("Index");
		}

		public ActionResult ChangeSprint(string sprint)
		{
			SelectedSprint = sprint;

			TempData["InfoMessage"] = "Sprintas sėkmingai pakeistas";

			return RedirectToAction("Index");
		}

		public ActionResult NormalizeImportances()
		{
			BugsModel.NormalizeBugImportances(SelectedSprint);

			return RedirectToAction("Index");
		}

		public ActionResult ExportCSV()
		{
			IList<Bug> bugs = BugsModel.GetBugList(SelectedSprint);

			string separator = ";";

			MemoryStream output = new MemoryStream();
			StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
			
			writer.WriteLine("BugID" + separator + 
				"Title" + separator + 
				"Estimate" + separator + 
				"Priority" + separator + 
				"Importance" + separator + 
				"Feature" + separator + 
				"Product");

			foreach (Bug bug in bugs)
			{
				writer.Write(bug.ID + separator);
				writer.Write("\"" + bug.Title.Replace('\"', '\'') + "\"");
				writer.Write(separator);
				writer.Write("\"" + bug.Estimate + "\"");
				writer.Write(separator);
				writer.Write("\"" + bug.Priority + "\"");
				writer.Write(separator);
				writer.Write(bug.Importance);
				writer.Write(separator);
				writer.Write("\"" + bug.Feature + "\"");
				writer.Write(separator);
				writer.Write("\"" + bug.Product + "\"");
				writer.WriteLine();
			}
			writer.Flush();
			output.Position = 0;

			return File(output, "text/comma-separated-values", "Bugs.csv");
		}
    }
}
