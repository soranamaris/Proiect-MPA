using Microsoft.AspNetCore.Mvc.RazorPages;
using Proiect_MPA.Data;

namespace Proiect_MPA.Models
{
    public class TableSchedulesPageModel : PageModel
    {
        public List<AssignedScheduleData> AssignedScheduleDataList;

        public void PopulateAssignedScheduleData(RestaurantContext context, Table table)
        {
            var allSchedules = context.Schedule;
            var tableSchedules = new HashSet<int>(
                table.BookingSchedules.Select(bs => bs.ScheduleID));
            AssignedScheduleDataList = new List<AssignedScheduleData>();

            foreach (var schedule in allSchedules)
            {
                AssignedScheduleDataList.Add(new AssignedScheduleData
                {
                    ScheduleID = schedule.ID,
                    Name = schedule.ScheduleName,
                    Assigned = tableSchedules.Contains(schedule.ID)
                });
            }
        }

        public void UpdateTableSchedules(RestaurantContext context, string[] selectedSchedules, Table tableToUpdate)
        {
            if (selectedSchedules == null)
            {
                tableToUpdate.BookingSchedules = new List<BookingSchedule>();
                return;
            }

            var selectedSchedulesHS = new HashSet<string>(selectedSchedules);
            var tableSchedules = new HashSet<int>(
                tableToUpdate.BookingSchedules.Select(bs => bs.Schedule.ID));

            foreach (var schedule in context.Schedule)
            {
                if (selectedSchedulesHS.Contains(schedule.ID.ToString()))
                {
                    if (!tableSchedules.Contains(schedule.ID))
                    {
                        tableToUpdate.BookingSchedules.Add(
                            new BookingSchedule
                            {
                                TableID = tableToUpdate.ID,
                                ScheduleID = schedule.ID
                            });
                    }
                }
                else
                {
                    if (tableSchedules.Contains(schedule.ID))
                    {
                        BookingSchedule scheduleToRemove = tableToUpdate
                            .BookingSchedules
                            .SingleOrDefault(bs => bs.ScheduleID == schedule.ID);
                        context.Remove(scheduleToRemove);
                    }
                }
            }
        }
    }
}


