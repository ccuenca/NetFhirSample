using FHIRCamp.Schedule.BL.Models;

namespace FHIRCamp.Schedule.BL.Services
{
    public interface IScheduleService
    {
        Task<List<ScheduleModel>> GetAll(string locationId);

        Task<List<SlotModel>> GetSlotsBySchedule(string scheduleId);

        Task CreateSchedule(ScheduleModel schedule);

        Task Delete(string id);
    }
}