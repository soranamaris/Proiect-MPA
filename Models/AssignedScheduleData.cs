using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_MPA.Models
{
    [NotMapped]
    public class AssignedScheduleData
    {
        public int ScheduleID { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
