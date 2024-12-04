using System.ComponentModel.DataAnnotations;

namespace Proiect_MPA.Models
{
    public class Table
    {
        public int ID { get; set; }

        [Display(Name = "Locuri disponibile")]
        [Range(1, 15, ErrorMessage = "Numarul de locuri trebuie sa fie intre 1 si 15")]

        public int Seats { get; set; }

        // Cheie străină pentru Waiter
        [Display(Name = "Chelner Responsabil")]
        public int? WaiterID { get; set; }
        public Waiter? Waiter { get; set; }

        // Cheie străină pentru Zone
        [Display(Name = "Zona")]
        public int? ZoneID { get; set; }
        public Zone? Zone { get; set; }

        // Programul pentru rezervări
        [Display(Name = "Disponibilitate (ore)")]

        public int? ReservationID { get; set; }
        public Reservation? Reservation { get; set; }

        public ICollection<BookingSchedule>? BookingSchedules { get; set; }
    }
}
