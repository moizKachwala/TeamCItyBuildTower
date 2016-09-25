using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TeamcityBuildTower.Domain;
using TeamcityBuildTower.Logic;

namespace TeamcityBuildTower
{
	public partial class TeamcityBuildTowerService : ServiceBase
	{
		private Timer timer = new Timer();
		private double _servicePollInterval;
		private TeamCityWrapper _teamCityWrapper;
		private readonly ILog s_log = LogManager.GetLogger(typeof(TeamCityWrapper));
		public TeamcityBuildTowerService()
		{
			InitializeComponent();
			_servicePollInterval = Convert.ToDouble(ConfigurationManager.AppSettings.Get("ServicePollInterval"));
			_teamCityWrapper = new TeamCityWrapper();
		}

		protected override void OnStart(string[] args)
		{
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Interval = _servicePollInterval;
			timer.AutoReset = true;
			timer.Enabled = true;
			timer.Start();
		}
		protected override void OnContinue()
		{
			base.OnContinue();
			timer.Start();
		}

		protected override void OnPause()
		{
			base.OnPause();
			timer.Stop();
		}

		protected override void OnStop()
		{
			timer.Stop();
		}

		void timer_Elapsed(object sender, EventArgs e)
		{
			s_log.Debug("timmer ticked");
			_teamCityWrapper.SendStatusToArduino();
		}
	}
}
