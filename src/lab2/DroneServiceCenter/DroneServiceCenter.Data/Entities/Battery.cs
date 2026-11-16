using System.ComponentModel.DataAnnotations;

namespace DroneServiceCenter.Data.Entities
{
    public class Battery
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string InventoryCode { get; set; } = string.Empty;

        public int ChargeCycles { get; set; }

        public decimal InternalResistance { get; set; }

        [Required]
        [MaxLength(20)]
        public string TrafficLightStatus { get; set; } = "Зелений";

        public int? DroneId { get; set; }
        public Drone? Drone { get; set; }
    }
}