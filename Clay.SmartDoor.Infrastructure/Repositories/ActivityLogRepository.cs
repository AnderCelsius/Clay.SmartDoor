using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly SmartDoorContext _context;

        public ActivityLogRepository(SmartDoorContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<ActivityLog> GetUserActivityLogs(string userId, DateTime fromDate, DateTime toDate)
        {
            return _context.ActivityLogs.AsNoTracking()
                .Where(x => x.ActionBy == userId && x.Time >= fromDate && x.Time <= toDate)
                .OrderByDescending(x => x.Time);
        }
    }
}
