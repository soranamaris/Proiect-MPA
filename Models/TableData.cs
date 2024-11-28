namespace Proiect_MPA.Models
{
    public class TableData
    {
        public IEnumerable<Table> Tables { get; set; }
        public IEnumerable<Schedule> Schedules { get; set; }
        public IEnumerable<BookingSchedule> BookingSchedules { get; set; }
    }
}
