using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamcityBuildTower.Domain
{
	public class Build
	{
		public string Id { get; set; }
		public string Number { get; set; }
		public string Status { get; set; }
		public string BuildTypeId { get; set; }
		public string Href { get; set; }
		public string WebUrl { get; set; }
		public string StatusText { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime FinishDate { get; set; }
		public string State { get; set; }
		public bool Running { get; set; }

		public override string ToString()
		{
			return Number;
		}
	}
}
