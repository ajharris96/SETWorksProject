using System;
namespace SETWorksProject.Models
{
	public class PayloadDeserialization
	{
        public class Dragon
        {
            public string capsule { get; set; }
            public double? mass_returned_kg { get; set; }
            public double? mass_returned_lbs { get; set; }
            public int? flight_time_sec { get; set; }
            public object manifest { get; set; }
            public bool? water_landing { get; set; }
            public bool? land_landing { get; set; }
        }

        public class Payload
        {
            public Dragon dragon { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public bool reused { get; set; }
            public string launch { get; set; }
            public List<string> customers { get; set; }
            public List<int> norad_ids { get; set; }
            public List<string> nationalities { get; set; }
            public List<string> manufacturers { get; set; }
            public double? mass_kg { get; set; }
            public double? mass_lbs { get; set; }
            public string orbit { get; set; }
            public string reference_system { get; set; }
            public string regime { get; set; }
            public object longitude { get; set; }
            public double? semi_major_axis_km { get; set; }
            public double? eccentricity { get; set; }
            public double? periapsis_km { get; set; }
            public double? apoapsis_km { get; set; }
            public double? inclination_deg { get; set; }
            public double? period_min { get; set; }
            public object lifespan_years { get; set; }
            public DateTime? epoch { get; set; }
            public double? mean_motion { get; set; }
            public double? raan { get; set; }
            public double? arg_of_pericenter { get; set; }
            public double? mean_anomaly { get; set; }
            public string id { get; set; }
        }

        public PayloadDeserialization()
		{
		}
	}
}

