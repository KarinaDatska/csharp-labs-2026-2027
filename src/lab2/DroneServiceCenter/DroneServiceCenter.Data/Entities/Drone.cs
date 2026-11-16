using System.ComponentModel.DataAnnotations;

namespace DroneServiceCenter.Data.Entities
{
    public class Drone
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string FirmwareVersion { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "У строю";

        public int PropellerHours { get; set; }

        public bool HasCrashMarks { get; set; }

        public ICollection<Battery> Batteries { get; set; } = new List<Battery>();
        public ICollection<FlightLog> FlightLogs { get; set; } = new List<FlightLog>();
        public ICollection<RepairOrder> RepairOrders { get; set; } = new List<RepairOrder>();
        public ICollection<SerialComponentLog> SerialComponentLogs { get; set; } = new List<SerialComponentLog>();
    }
}