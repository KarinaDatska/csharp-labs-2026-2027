using DroneServiceCenter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Services
{
    public class FlightService
    {
        public List<FlightLog> GetAll()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.FlightLogs
                    .Include(f => f.Drone)
                    .OrderByDescending(f => f.FlightDate)
                    .ToList();
            }
        }

        public List<FlightLog> GetSignalLossIncidents()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.FlightLogs
                    .Include(f => f.Drone)
                    .Where(f => f.SignalLost || f.RthAltitudeM <= 30)
                    .OrderByDescending(f => f.FlightDate)
                    .ToList();
            }
        }

        public void Add(FlightLog log)
        {
            CheckFlight(log);

            using (var context = DbContextFactory.Create())
            {
                var drone = context.Drones
                    .Include(d => d.RepairOrders)
                    .FirstOrDefault(d => d.Id == log.DroneId);

                if (drone == null)
                {
                    throw new Exception("Дрон не знайдено.");
                }

                bool hasOpenRepairs = drone.RepairOrders.Any(r => r.ClosedAt == null);
                if (drone.Status != "У строю" || hasOpenRepairs)
                {
                    throw new Exception("Цей дрон зараз не допущений до польотів.");
                }

                context.FlightLogs.Add(log);
                context.SaveChanges();
            }
        }

        public void Update(FlightLog log)
        {
            CheckFlight(log);

            using (var context = DbContextFactory.Create())
            {
                var existing = context.FlightLogs.Find(log.Id);
                if (existing == null)
                {
                    throw new Exception("Запис не знайдено.");
                }

                existing.DroneId = log.DroneId;
                existing.FlightDate = log.FlightDate;
                existing.PilotName = log.PilotName.Trim();
                existing.Location = log.Location.Trim();
                existing.RthAltitudeM = log.RthAltitudeM;
                existing.DurationMin = log.DurationMin;
                existing.SignalLost = log.SignalLost;

                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var context = DbContextFactory.Create())
            {
                var log = context.FlightLogs.Find(id);
                if (log == null)
                {
                    throw new Exception("Запис не знайдено.");
                }

                context.FlightLogs.Remove(log);
                context.SaveChanges();
            }
        }

        private void CheckFlight(FlightLog log)
        {
            if (log.DroneId <= 0)
                throw new Exception("Оберіть дрон.");

            if (string.IsNullOrWhiteSpace(log.PilotName))
                throw new Exception("Введіть ПІБ пілота.");

            if (string.IsNullOrWhiteSpace(log.Location))
                throw new Exception("Введіть район польоту.");

            if (log.RthAltitudeM <= 0 || log.RthAltitudeM > 500)
                throw new Exception("Висота RTH має бути від 1 до 500 метрів.");

            if (log.DurationMin <= 0 || log.DurationMin > 300)
                throw new Exception("Тривалість має бути від 1 до 300 хвилин.");
        }
    }
}