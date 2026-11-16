using DroneServiceCenter.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Services
{
    public class DroneService
    {
        public List<Drone> GetAll()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.Drones
                    .Include(d => d.RepairOrders)
                    .Include(d => d.FlightLogs)
                    .OrderBy(d => d.Id)
                    .ToList();
            }
        }

        public List<Drone> GetReadyForFlight()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.Drones
                    .Where(d => d.Status == "У строю" && !d.RepairOrders.Any(r => r.ClosedAt == null))
                    .OrderBy(d => d.PropellerHours)
                    .ToList();
            }
        }

        public void Add(Drone drone)
        {
            CheckDrone(drone);

            try
            {
                using (var context = DbContextFactory.Create())
                {
                    context.Drones.Add(drone);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                if (IsDuplicateError(ex))
                {
                    throw new Exception("Дрон з таким серійним номером вже існує в базі.");
                }
                throw;
            }
        }

        public void Update(Drone drone)
        {
            CheckDrone(drone);

            try
            {
                using (var context = DbContextFactory.Create())
                {
                    var existing = context.Drones
                        .Include(d => d.RepairOrders)
                        .FirstOrDefault(d => d.Id == drone.Id);

                    if (existing == null)
                    {
                        throw new Exception("Дрон не знайдено.");
                    }

                    bool hasOpenRepairs = existing.RepairOrders.Any(r => r.ClosedAt == null);
                    if (drone.Status == "У строю" && hasOpenRepairs)
                    {
                        throw new Exception("Дрон з відкритим ремонтом не може мати статус «У строю».");
                    }

                    existing.SerialNumber = drone.SerialNumber.Trim();
                    existing.Model = drone.Model.Trim();
                    existing.FirmwareVersion = drone.FirmwareVersion.Trim();
                    existing.Status = drone.Status;
                    existing.PropellerHours = drone.PropellerHours;
                    existing.HasCrashMarks = drone.HasCrashMarks;

                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                if (IsDuplicateError(ex))
                {
                    throw new Exception("Дрон з таким серійним номером вже існує в базі.");
                }
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = DbContextFactory.Create())
                {
                    var drone = context.Drones.Find(id);
                    if (drone == null)
                    {
                        throw new Exception("Дрон не знайдено.");
                    }

                    context.Drones.Remove(drone);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException)
            {
                throw new Exception("Неможливо видалити дрон, бо до нього прив'язані польоти або ремонти.");
            }
        }

        private void CheckDrone(Drone drone)
        {
            if (string.IsNullOrWhiteSpace(drone.SerialNumber))
                throw new Exception("Введіть серійний номер.");

            if (drone.SerialNumber.Length > 50)
                throw new Exception("Серійний номер занадто довгий (максимум 50 символів).");

            if (string.IsNullOrWhiteSpace(drone.Model))
                throw new Exception("Введіть модель дрона.");

            if (string.IsNullOrWhiteSpace(drone.FirmwareVersion))
                throw new Exception("Введіть версію прошивки.");

            if (drone.PropellerHours < 0)
                throw new Exception("Наліт гвинтів не може бути меншим за нуль.");
        }

        private bool IsDuplicateError(DbUpdateException ex)
        {
            if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                return sqlEx.Number == 2601 || sqlEx.Number == 2627;
            }
            return false;
        }
    }
}