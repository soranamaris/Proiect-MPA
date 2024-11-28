using System.ComponentModel.DataAnnotations;

namespace Proiect_MPA.Models
{
    public class Zone
    {
        public int ID { get; set; }

        [Display(Name = "Zona")]
        public string Name { get; set; }
        // Relație cu mesele
        public ICollection<Table>? Tables { get; set; }
    }
}
