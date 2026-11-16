using DroneServiceCenter.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Services
{
    public class RepairService
    {
        public List<RepairOrder> GetAll()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.RepairOrders
                    .Include(r => r.Drone)
                    .OrderByDescending(r => r.OpenedAt)
                    .ToList();
            }
        }

        public List<RepairOrder> GetOpenOrders()
        {
            using (var context = DbContextFactory.Create())
            {
                return context.RepairOrders
                    .Include(r => r.Drone)
                    .Where(r => r.ClosedAt == null)
                    .OrderBy(r => r.OpenedAt)
                    .ToList();
            }
        }

        public void Add(RepairOrder order)
        {
            CheckOrder(order);

            using (var context = DbContextFactory.Create())
            {
                var drone = context.Drones.Find(order.DroneId);
                if (drone == null)
                {
                    throw new Exception("Вибраний дрон не знайдено.");
                }

                if (order.ClosedAt == null)
                {
                    drone.Status = "На ремонті";
                }

                context.RepairOrders.Add(order);
                context.SaveChanges();
            }
        }

        public void Update(RepairOrder order)
        {
            CheckOrder(order);

            using (var context = DbContextFactory.Create())
            {
                var existing = context.RepairOrders.Find(order.Id);
                if (existing == null)
                {
                    throw new Exception("Наряд не знайдено.");
                }

                existing.DroneId = order.DroneId;
                existing.OpenedAt = order.OpenedAt;
                existing.IssueDescription = order.IssueDescription.Trim();
                existing.ReplacedParts = order.ReplacedParts;
                existing.IsTestFlightPassed = order.IsTestFlightPassed;
                existing.ClosedAt = order.ClosedAt;

                var drone = context.Drones
                    .Include(d => d.RepairOrders)
                    .FirstOrDefault(d => d.Id == order.DroneId);

                if (drone != null)
                {
                    bool hasOtherOpen = drone.RepairOrders.Any(r => r.Id != order.Id && r.ClosedAt == null);
                    if (order.ClosedAt == null || hasOtherOpen)
                    {
                        drone.Status = "На ремонті";
                    }
                    else if (drone.Status == "На ремонті")
                    {
                        drone.Status = "У строю";
                    }
                }

                context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var context = DbContextFactory.Create())
            {
                var order = context.RepairOrders.Find(id);
                if (order == null)
                {
                    throw new Exception("Наряд не знайдено.");
                }

                context.RepairOrders.Remove(order);
                context.SaveChanges();
            }
        }

        private void CheckOrder(RepairOrder order)
        {
            if (order.DroneId <= 0)
                throw new Exception("Оберіть дрон.");

            if (string.IsNullOrWhiteSpace(order.IssueDescription))
                throw new Exception("Опишіть несправність.");

            if (order.ClosedAt != null && !order.IsTestFlightPassed)
                throw new Exception("Не можна закрити наряд без контрольного обльоту.");

            if (order.ClosedAt != null && order.ClosedAt < order.OpenedAt)
                throw new Exception("Дата закриття не може бути меншою за дату відкриття.");
        }
    }
}