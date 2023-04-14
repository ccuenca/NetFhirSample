using System;

namespace FHIRCamp.Schedule.BL.Models
{
	public class SlotModel
	{
        public string Id { get; set; }

        public string Status { get; set; }

		public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        public SlotModel()
		{
		}
	}
}

