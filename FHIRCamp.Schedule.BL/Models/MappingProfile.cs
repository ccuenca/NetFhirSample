using System;
using fhirModel = Hl7.Fhir.Model;
using AutoMapper;

namespace FHIRCamp.Schedule.BL.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var server = "https://server.fire.ly/";

            //Schedule
            CreateMap<fhirModel.Schedule, ScheduleModel>()
                .ForMember(dest => dest.Id, m => m.MapFrom(src => src.Id))
                .ForMember(dest => dest.LocationName, m => m.MapFrom(src => src.Actor[0].Display))
                .ForMember(dest => dest.LocationId, m => m.MapFrom(src => src.Actor[0].Reference.Replace($"{server}Location/", string.Empty)))
                .ForMember(dest => dest.PractitionerName, m => m.MapFrom(src => src.Actor[1].Display))
                .ForMember(dest => dest.PractitionerId, m => m.MapFrom(src => src.Actor[1].Reference.Replace($"{server}Practitioner/", string.Empty)))              
                .ForMember(dest => dest.HealthcareServiceName, m => m.MapFrom(src => src.Actor[2].Display))
                .ForMember(dest => dest.HealthcareServiceId, m => m.MapFrom(src => src.Actor[2].Reference.Replace($"{server}HealthcareService/", string.Empty)))
                .ForMember(dest => dest.StartDate, m => m.MapFrom(src => DateTime.Parse(src.PlanningHorizon.Start)))
                .ForMember(dest => dest.EndDate, m => m.MapFrom(src => DateTime.Parse(src.PlanningHorizon.End)))
                .ForMember(dest => dest.Comment, m => m.MapFrom(src => src.Comment))
                .ReverseMap();

            //Slot
            CreateMap<fhirModel.Slot, SlotModel>()
                .ForMember(dest => dest.Id, m => m.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, m => m.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Start, m => m.MapFrom(src => src.Start))
                .ForMember(dest => dest.End, m => m.MapFrom(src => src.End));

        }
    }
}

