using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class AccessGroupRepository : GenericRepository<AccessGroup>, IAccessGroupRepository
    {
        private readonly SmartDoorContext _context;

        public AccessGroupRepository(SmartDoorContext context) 
            : base(context)
        {
            _context = context;
        }

        public async Task<AccessGroup?> GetAccessGroupsAsync(string groupName)
        {
            return await _context.AccessGroups.AsNoTracking().SingleOrDefaultAsync(a => a.Name == groupName);
        }

        public IQueryable<AccessGroup> GetAccessGroupsByActiveStatusAsync(bool isActive)
        {
            return _context.AccessGroups.AsNoTracking().Where(a => a.IsActive == isActive);
        }
    }
}
