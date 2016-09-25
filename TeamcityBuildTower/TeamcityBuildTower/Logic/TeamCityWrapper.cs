using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TeamcityBuildTower.Domain;

namespace TeamcityBuildTower.Logic
{
	public class TeamCityWrapper
	{
		private readonly ILog s_log = LogManager.GetLogger(typeof(TeamCityWrapper));

		private Credential credential;
		private SerialPort port;
		private string buildConfigurationId;
		private string host;
		private int lastStatus = -1;
		public TeamCityWrapper()
		{
			s_log.Debug("Initializing the Teamcity Wrapper");

			this.credential = new Credential()
			{
				UserName = ConfigurationManager.AppSettings.Get("Username"),
				Password = ConfigurationManager.AppSettings.Get("Password")
			};
			this.port = new SerialPort(ConfigurationManager.AppSettings.Get("Comport"), 9600);
			this.buildConfigurationId = ConfigurationManager.AppSettings.Get("BuildConfigurationId");
			this.host = ConfigurationManager.AppSettings.Get("TeamcityUrl");
		}

		/// <summary>
		/// 0 = Fails
		/// 1 = Pass
		/// 2 = Running
		/// </summary>
		public void SendStatusToArduino()
		{
			s_log.Debug("calling the status arduino");
			if (IsBuildRunning())
			{
				SendMessageToCompIfNew(2);
			}
			else
			{
				var currentStatus = GetCurrentStatus();
				if (currentStatus == "SUCCESS")
				{
					SendMessageToCompIfNew(1);
				}
				else
				{
					SendMessageToCompIfNew(0);
				}
			}
		}

		private bool IsBuildRunning()
		{
			string tcUrlWithRunningStatus = String.Format("{0}/httpAuth/app/rest/builds?locator=buildType:(id:{1}),running:true,count:1&fields=count,build(status,state,running)", this.host, this.buildConfigurationId);
			var currentBuild = ProcessTeamcityRequest(tcUrlWithRunningStatus);

			return Convert.ToInt32(currentBuild.Count) > 0 ? true : false;
		}

		private string GetCurrentStatus()
		{
			string tcUrlWithRunningStatus = String.Format("{0}/httpAuth/app/rest/builds?locator=buildType:(id:{1}),count:1&fields=count,build(status,state,running)", this.host, this.buildConfigurationId);
			var currentBuild = ProcessTeamcityRequest(tcUrlWithRunningStatus);

			if (Convert.ToInt32(currentBuild.Count) > 0)
			{
				return currentBuild.Build[0].Status;
			}
			else
			{
				return "";
			}
		}

		private BuildWrapper ProcessTeamcityRequest(string url)
		{
			try
			{
				var tcResponse = "";
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "GET";
				string authInfo = this.credential.UserName + ":" + this.credential.Password;
				authInfo = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo));
				request.Headers.Add("Authorization", "Basic " + authInfo);
				request.Accept = "application/json";

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					tcResponse = reader.ReadToEnd();
				}

				return Newtonsoft.Json.JsonConvert.DeserializeObject<BuildWrapper>(tcResponse);
			}
			catch (Exception ex)
			{
				s_log.Error(ex);
				return null;
			}
		}

		private void SendMessageToCompIfNew(int message)
		{
			if (lastStatus != message)
			{
				SendMessageToComPort(message); // only send if there is new status from TC.
			}
		}

		private void SendMessageToComPort(int message)
		{
			try
			{
				s_log.Debug("trying to send the message to comp port");

				if (!port.IsOpen)
				{
					port.Open();
				}
				byte[] msg = System.Text.Encoding.UTF8.GetBytes(message.ToString());
				port.Write(msg, 0, msg.Length);

				lastStatus = message; // everything is success, then write the current status 
				s_log.Debug("message sent successfully");
			}
			catch (Exception ex)
			{
				s_log.Error(ex);
			}
		}
	}
}
