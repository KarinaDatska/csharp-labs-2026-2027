using System.ComponentModel.DataAnnotations;

namespace DroneServiceCenter.Data.Entities
{
    public class FlightLog
    {
        public int Id { get; set; }

        public int DroneId { get; set; }
        public Drone? Drone { get; set; }

        public DateTime FlightDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string PilotName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        public int RthAltitudeM { get; set; }

        public int DurationMin { get; set; }

        public bool SignalLost { get; set; }
    }
}