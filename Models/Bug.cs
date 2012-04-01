using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BootstrapMVC.Models
{
	public class Bug
	{
		public long ID { get; set; }
		public string Title { get; set; }
		public string Estimate { get; set; }
		public string Priority { get; set; }
		public long Importance { get; set; }
		public string Feature { get; set; }
		public string Product { get; set; }
	}
}