using System;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using FHIRCamp.Schedule.BL.Models;
using fhirModel = Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using AutoMapper;
using Hl7.Fhir.Model;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Hl7.Fhir.Rest;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FHIRCamp.Schedule.BL.Services
{
    public class ScheduleService : IScheduleService
    {
        private HttpClient _httpClient;
        private IMapper _mapper;

        public ScheduleService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async System.Threading.Tasks.Task CreateSchedule(ScheduleModel schedule)
        {
            //Create schedule
            fhirModel.Schedule _schedule = new fhirModel.Schedule();

            var startDate = schedule.StartDate;
            var endDate = schedule.EndDate;

            startDate = startDate.AddHours(schedule.StartHour);
            endDate = endDate.AddHours(schedule.EndHour);
            endDate = endDate.AddMinutes(59);
            
            _schedule.ServiceCategory.Add(new CodeableConcept("http://terminology.hl7.org/CodeSystem/service-category", "17", "General Practice", ""));
            _schedule.Identifier.Add(new Identifier("http://acmesalud.com.co/scheduleid", Guid.NewGuid().ToString()));
            _schedule.Specialty.Add(new CodeableConcept("http://snomed.info/sct", "394802001", "medicina general", ""));
            _schedule.Actor.Add(new ResourceReference($"Location/{schedule.LocationId}", schedule.LocationName));
            _schedule.Actor.Add(new ResourceReference($"Practitioner/{schedule.PractitionerId}", schedule.PractitionerName));
            _schedule.Actor.Add(new ResourceReference($"HealthcareService/{schedule.HealthcareServiceId}", schedule.HealthcareServiceName));
            _schedule.PlanningHorizon = new Period(new FhirDateTime(schedule.StartDate), new FhirDateTime( schedule.EndDate));            
            _schedule.Comment = schedule.Comment;
            _schedule.Active = true;

            var json = _schedule.ToJson();
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/Schedule", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            FhirJsonSerializer serializer = new FhirJsonSerializer();

            var options = new JsonSerializerOptions().ForFhir(typeof(fhirModel.Schedule).Assembly);

            var newSchedule = System.Text.Json.JsonSerializer.Deserialize<fhirModel.Schedule>(content, options);

            var dateStatus = DateTime.Compare(startDate, endDate);

            // Create Slots
            while ( dateStatus < 0 ) {

                fhirModel.Slot _slot = new fhirModel.Slot();

                _slot.Status = Slot.SlotStatus.Free;
                _slot.Start = startDate;
                startDate = startDate.AddMinutes(schedule.Interval);
                _slot.End = startDate;

                _slot.Schedule = new ResourceReference($"/Schedule/{newSchedule?.Id}");

                var jsonSlot = _slot.ToJson();
                var jsonSlotContent = new StringContent(jsonSlot, Encoding.UTF8, "application/json");
                var responseSlot = await _httpClient.PostAsync("/Slot", jsonSlotContent);
                var contentSlot = await response.Content.ReadAsStringAsync();

                if (!responseSlot.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }

                dateStatus = DateTime.Compare(startDate, endDate);
            }
        }

        public async Task<List<ScheduleModel>> GetAll(string locationId)
        {
            var response = await _httpClient.GetAsync($"/Schedule?actor=Location/{locationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var parser = new FhirJsonParser();

            var schedulesBundle = await parser.ParseAsync<Hl7.Fhir.Model.Bundle>(content);

            var schedules = new List<ScheduleModel>();

            if (schedulesBundle != null)
            {
                foreach (var r in schedulesBundle.Entry)
                {
                    if (r.Resource is fhirModel.Schedule)
                    {
                        var scheduleObject = (fhirModel.Schedule)r.Resource;

                        if (scheduleObject.Actor.Count > 0)
                        {
                            var s = _mapper.Map<fhirModel.Schedule, ScheduleModel>(scheduleObject);
                            schedules.Add(s);
                        }
                    }
                }
            }

            return schedules;
        }

        public async Task<List<SlotModel>> GetSlotsBySchedule(string scheduleId)
        {
            var response = await _httpClient.GetAsync($"/Slot?schedule={scheduleId}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var parser = new FhirJsonParser();

            var slotsBundle = await parser.ParseAsync<Hl7.Fhir.Model.Bundle>(content);

            var slots = new List<SlotModel>();

            if (slotsBundle != null)
            {
                foreach (var r in slotsBundle.Entry)
                {
                    if (r.Resource is fhirModel.Slot)
                    {
                        var scheduleObject = (fhirModel.Slot)r.Resource;

                        var s = _mapper.Map<fhirModel.Slot, SlotModel>(scheduleObject);
                        slots.Add(s);

                    }
                }
            }

            return slots;
        }

        public async System.Threading.Tasks.Task Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"/Schedule/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
        }
    }
}