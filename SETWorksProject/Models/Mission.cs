using System;
namespace SETWorksProject.Models
{
	public class Mission
	{
		public string Date { get; set; }
		public string UTCTIME { get; set; }
		public string Time { get; set; }
		public string RocketName { get; set; }
		public bool? WasSuccess { get; set; }
		public string MassRank { get; set; }
		public double? Mass { get; set; }
		public string? Pic { get; set; }


		public Mission()
		{
		}

        public Mission(string date, string time, string rocketName, bool wasSuccess, double mass)
        {
            Date = date;
            Time = time;
            RocketName = rocketName;
            WasSuccess = wasSuccess;
            Mass = mass;
        }
    }
}

