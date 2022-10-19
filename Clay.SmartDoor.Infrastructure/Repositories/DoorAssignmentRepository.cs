using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class DoorAssignmentRepository : GenericRepository<DoorAssignment>, IDoorAssignmentRepository
    {
        private readonly SmartDoorContext _context;

        public DoorAssignmentRepository(SmartDoorContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DoorAssignment?> GetDoorAssignmentAsync(string doorId, string accessGroupId)
        {
            return await _context.DoorAssignment.AsNoTracking()
                .SingleOrDefaultAsync(da => da.DoorId == doorId && da.AccessGroupId == accessGroupId && da.Assigned == true);
        }
    }
}
