using DroneServiceCenter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.Migrate();

            if (context.Drones.Any())
            {
                return;
            }

            var drone1 = new Drone
            {
                SerialNumber = "DJI-M350-001",
                Model = "DJI Matrice 350 RTK",
                FirmwareVersion = "v07.01.1003",
                Status = "У строю",
                PropellerHours = 42,
                HasCrashMarks = false
            };

            var drone2 = new Drone
            {
                SerialNumber = "DJI-M3T-002",
                Model = "DJI Mavic 3 Thermal",
                FirmwareVersion = "v01.00.0900",
                Status = "На ремонті",
                PropellerHours = 85,
                HasCrashMarks = true
            };

            var drone3 = new Drone
            {
                SerialNumber = "AUT-EVO2-003",
                Model = "Autel EVO II Dual 640T",
                FirmwareVersion = "v2.7.35",
                Status = "У строю",
                PropellerHours = 19,
                HasCrashMarks = false
            };

            var drone4 = new Drone
            {
                SerialNumber = "DJI-FPV-004",
                Model = "DJI Avata 2 Pro",
                FirmwareVersion = "v01.00.0400",
                Status = "Списаний",
                PropellerHours = 140,
                HasCrashMarks = true
            };

            context.Drones.AddRange(drone1, drone2, drone3, drone4);
            context.SaveChanges();

            context.Batteries.AddRange(
                new Battery
                {
                    InventoryCode = "BAT-TB65-101",
                    ChargeCycles = 38,
                    InternalResistance = 11.4m,
                    TrafficLightStatus = "Зелений",
                    DroneId = drone1.Id
                },
                new Battery
                {
                    InventoryCode = "BAT-TB65-102",
                    ChargeCycles = 115,
                    InternalResistance = 18.9m,
                    TrafficLightStatus = "Жовтий",
                    DroneId = drone1.Id
                },
                new Battery
                {
                    InventoryCode = "BAT-M3-201",
                    ChargeCycles = 210,
                    InternalResistance = 27.5m,
                    TrafficLightStatus = "Червоний",
                    DroneId = null
                },
                new Battery
                {
                    InventoryCode = "BAT-AUT-301",
                    ChargeCycles = 24,
                    InternalResistance = 10.2m,
                    TrafficLightStatus = "Зелений",
                    DroneId = drone3.Id
                }
            );

            context.FlightLogs.AddRange(
                new FlightLog
                {
                    DroneId = drone1.Id,
                    FlightDate = DateTime.Now.AddDays(-5),
                    PilotName = "Коваленко О.В.",
                    Location = "Промзона Північна (огляд ЛЕП)",
                    RthAltitudeM = 80,
                    DurationMin = 34,
                    SignalLost = false
                },
                new FlightLog
                {
                    DroneId = drone2.Id,
                    FlightDate = DateTime.Now.AddDays(-3),
                    PilotName = "Бондаренко І.М.",
                    Location = "Центральний район (щільна забудова)",
                    RthAltitudeM = 30,
                    DurationMin = 12,
                    SignalLost = true
                },
                new FlightLog
                {
                    DroneId = drone3.Id,
                    FlightDate = DateTime.Now.AddDays(-1),
                    PilotName = "Шевченко Т.Г.",
                    Location = "Полігон Випробувальний-2",
                    RthAltitudeM = 60,
                    DurationMin = 28,
                    SignalLost = false
                },
                new FlightLog
                {
                    DroneId = drone1.Id,
                    FlightDate = DateTime.Now.AddHours(-6),
                    PilotName = "Коваленко О.В.",
                    Location = "Мостовий перехід №4",
                    RthAltitudeM = 95,
                    DurationMin = 41,
                    SignalLost = false
                }
            );

            context.RepairOrders.AddRange(
                new RepairOrder
                {
                    DroneId = drone2.Id,
                    OpenedAt = DateTime.Now.AddDays(-2),
                    IssueDescription = "Пошкодження переднього правого променя та калібрування підвісу після жорсткої посадки.",
                    ReplacedParts = "Промінь карбоновий правий, демпфер підвісу",
                    IsTestFlightPassed = false,
                    ClosedAt = null
                },
                new RepairOrder
                {
                    DroneId = drone1.Id,
                    OpenedAt = DateTime.Now.AddDays(-12),
                    IssueDescription = "Планова заміна комплекту гвинтів та перевірка підшипників двигунів.",
                    ReplacedParts = "Комплект гвинтів 2110s (4 шт.)",
                    IsTestFlightPassed = true,
                    ClosedAt = DateTime.Now.AddDays(-10)
                }
            );

            context.SerialComponentLogs.AddRange(
                new SerialComponentLog
                {
                    DroneId = drone2.Id,
                    ComponentType = "Підвіс камери H20T",
                    OldSerialNumber = "GMB-88412-OLD",
                    NewSerialNumber = "GMB-99104-NEW",
                    ReplacedAt = DateTime.Now.AddDays(-1)
                }
            );

            context.SaveChanges();
        }
    }
}