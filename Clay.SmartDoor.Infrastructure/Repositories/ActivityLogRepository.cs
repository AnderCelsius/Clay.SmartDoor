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
                .Where(x => x.ActionBy == userId &&
                EF.Functions.DateDiffDay(fromDate, x.Time) >= 0 && EF.Functions.DateDiffDay(x.Time, toDate) >= 0)
                .OrderByDescending(x => x.Time);
        }
    }
}
