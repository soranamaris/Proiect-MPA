namespace Proiect_MPA.Models
{
    public class BookingSchedule
    {
        public int ID { get; set; }
        public int TableID { get; set; }
        public Table Table { get; set; }

        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }
    }
}
