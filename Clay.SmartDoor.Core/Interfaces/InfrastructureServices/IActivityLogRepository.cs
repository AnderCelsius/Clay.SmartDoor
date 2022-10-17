using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog>
    {
        IQueryable<ActivityLog> GetUserActivityLogs(string userId, DateTime fromDate, DateTime toDate);
    }
}
