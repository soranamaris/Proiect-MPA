using System.ComponentModel.DataAnnotations;

namespace Proiect_MPA.Models
{
    public class Reservation
    {
        public int ID { get; set; }
        [Display(Name = "Nr CLient")]
        public int? ClientID { get; set; }
        public Client? Client { get; set; }
        [Display(Name = "Table")]
        public int? TableID { get; set; }

        public Table? Table { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime ReservationDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Ora rezervarii")]
        public DateTime ReservationTime { get; set; }

        [Range(1, 10, ErrorMessage = "Durata trebuie să fie între 1 și 10 ore.")]
        [Display(Name = "Durata (ore)")]
        public int ReservationDuration { get; set; }
    }
}
