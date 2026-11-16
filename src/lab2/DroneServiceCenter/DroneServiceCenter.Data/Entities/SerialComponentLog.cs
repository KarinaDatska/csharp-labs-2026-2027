using System.ComponentModel.DataAnnotations;

namespace DroneServiceCenter.Data.Entities
{
    public class SerialComponentLog
    {
        public int Id { get; set; }

        public int DroneId { get; set; }
        public Drone? Drone { get; set; }

        [Required]
        [MaxLength(50)]
        public string ComponentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OldSerialNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NewSerialNumber { get; set; } = string.Empty;

        public DateTime ReplacedAt { get; set; }
    }
}