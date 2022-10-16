using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>, IActivityLogRepository
    {
        private readonly SmartDoorContext _context;

        public ActivityLogRepository(SmartDoorContext context) : base(context)
        {
            _context = context;
        }
    }
}
