using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamcityBuildTower.Domain
{
	public class BuildWrapper
	{
		public string Count { get; set; }
		public List<Build> Build { get; set; }
	}
}
