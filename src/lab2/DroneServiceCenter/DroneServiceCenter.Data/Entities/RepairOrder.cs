using System.ComponentModel.DataAnnotations;

namespace DroneServiceCenter.Data.Entities
{
    public class RepairOrder
    {
        public int Id { get; set; }

        public int DroneId { get; set; }
        public Drone? Drone { get; set; }

        public DateTime OpenedAt { get; set; }

        [Required]
        [MaxLength(500)]
        public string IssueDescription { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ReplacedParts { get; set; }

        public bool IsTestFlightPassed { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}