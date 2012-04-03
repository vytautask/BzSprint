using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace BootstrapMVC.Models
{
	public class Options
	{
		public String BugSelectQuery
		{
			get
			{
				return File.ReadAllText(HttpContext.Current.Server.MapPath("~/Models/Queries/bugs_select_query.sql"), Encoding.UTF8);
			}
			set
			{
				File.WriteAllText(HttpContext.Current.Server.MapPath("~/Models/Queries/bugs_select_query.sql"), value, Encoding.UTF8);
			}
		}
	}
}