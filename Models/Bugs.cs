using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data.Odbc;

namespace BootstrapMVC.Models
{
	public class Bugs
	{
		public Bugs()
		{

		}

		public IList<Bug> GetBugList(string sprint)
		{
			List<Bug> bugs = new List<Bug>();

			using (OdbcConnection con = new OdbcConnection(WebConfigurationManager.ConnectionStrings["BugsDB"].ConnectionString))
			{
				con.Open();

				using (OdbcCommand com = new OdbcCommand("SELECT bug_id, short_desc, cf_duration, priority, CAST(cf_scrum_importance AS SIGNED INT), "
					+ "COALESCE(cf_octopusfeature, cf_foxusfeature) as feature, products.name "
					+ " FROM bugs inner join products on products.id = bugs.product_id "
					+ "WHERE product_id IN (" + WebConfigurationManager.AppSettings["ProductIDs"] 
					+ ") AND bug_status IN ('ASSIGNED', 'REOPENED') and cf_scrum_sprint = ? ORDER BY CAST(cf_scrum_importance AS SIGNED INT) desc, priority", con))
				{
					com.Parameters.AddWithValue("@sprintName", sprint);

					using (OdbcDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							Bug bug = new Bug();
							bug.ID = reader.GetInt64(0);
							bug.Title = reader.GetString(1);
							bug.Estimate = reader.GetString(2);
							bug.Priority = reader.GetString(3);

							if (!reader.IsDBNull(4))
								bug.Importance = reader.GetInt64(4);

							bug.Feature = reader.GetString(5);
							bug.Product = reader.GetString(6);

							bugs.Add(bug);
						}
					}
				}

				con.Close();
			}

			return bugs;
		}

		public IList<string> GetAllSprints()
		{
			List<string> sprints = new List<string>();

			using (OdbcConnection con = new OdbcConnection(WebConfigurationManager.ConnectionStrings["BugsDB"].ConnectionString))
			{
				con.Open();

				using (OdbcCommand com = new OdbcCommand("SELECT value FROM cf_scrum_sprint", con))
				{
					using (OdbcDataReader reader = com.ExecuteReader())
					{
						while (reader.Read())
						{
							sprints.Add(reader.GetString(0));
						}
					}
				}

				con.Close();
			}

			return sprints;
		}

		public void UpdateBug(Bug bug)
		{
			using (OdbcConnection con = new OdbcConnection(WebConfigurationManager.ConnectionStrings["BugsDB"].ConnectionString))
			{
				con.Open();

				UpdateBug(bug, con);

				con.Close();
			}
		}

		private void UpdateBug(Bug bug, OdbcConnection con)
		{
			using (OdbcCommand com = new OdbcCommand("UPDATE bugs set cf_scrum_importance = ? WHERE bug_id = ?", con))
			{
				com.Parameters.AddWithValue("@importance", bug.Importance);
				com.Parameters.AddWithValue("@bugID", bug.ID);

				com.ExecuteNonQuery();
			}

			bug = null;
			con = null;
		}

		public void BulkUpdateBugs(IList<Bug> bugs)
		{
			using (OdbcConnection con = new OdbcConnection(WebConfigurationManager.ConnectionStrings["BugsDB"].ConnectionString))
			{
				con.Open();

				foreach (Bug bug in bugs)
				{
					UpdateBug(bug, con);
				}

				con.Close();
			}

			bugs = null;
		}

		public void MoveBugUp(long bugID, IList<Bug> buglist)
		{
			if(buglist != null)
			{
				int bugCount = buglist.Count;
				for (int i = 0; i < bugCount; i++)
				{
					if (buglist[i].ID == bugID)
					{
						if (i > 0)
						{
							long tmp = buglist[i].Importance;
							buglist[i].Importance = buglist[i - 1].Importance;

							buglist[i - 1].Importance = tmp;

							UpdateBug(buglist[i]);
							UpdateBug(buglist[i - 1]);
						}

						break;
					}
				}
			}
		}

		public void MoveBugDown(long bugID, IList<Bug> buglist)
		{
			if (buglist != null)
			{
				int bugCount = buglist.Count;
				for (int i = 0; i < bugCount; i++)
				{
					if (buglist[i].ID == bugID)
					{
						if (i + 1 < bugCount)
						{
							long tmp = buglist[i].Importance;
							buglist[i].Importance = buglist[i + 1].Importance;
							buglist[i + 1].Importance = tmp;

							UpdateBug(buglist[i]);
							UpdateBug(buglist[i + 1]);
						}

						break;
					}
				}
			}
		}

		public void NormalizeBugImportances(string sprint)
		{
			IList<Bug> bugs = GetBugList(sprint);

			if (bugs != null && bugs.Count > 0)
			{
				long importance = 0;
				for (int i = bugs.Count - 1; i > -1; i--)
				{
					bugs[i].Importance = importance;
					importance += 10;
				}

				BulkUpdateBugs(bugs);
			}
		}
	}
}