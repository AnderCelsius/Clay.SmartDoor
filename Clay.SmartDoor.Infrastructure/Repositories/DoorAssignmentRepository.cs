using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class DoorAssignmentRepository : GenericRepository<DoorAssignment>, IDoorAssignmentRepository
    {
        public DoorAssignmentRepository(SmartDoorContext context) : base(context)
        {
        }
    }
}
