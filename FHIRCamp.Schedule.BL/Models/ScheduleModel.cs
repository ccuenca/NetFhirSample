using System;

namespace FHIRCamp.Schedule.BL.Models
{
	public class ScheduleModel
	{   
        public string Id { get; set; }

        public string PractitionerId { get; set; }

        public string PractitionerName { get; set; }

        public string LocationId { get; set; }

        public string LocationName { get; set; }

        public string HealthcareServiceName { get; set; }

        public string HealthcareServiceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Comment { get; set; }

        public int StartHour { get; set; }

        public int EndHour { get; set; }

        public int Interval { get; set; }


        public ScheduleModel()
		{

		}


	}
}

