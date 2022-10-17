using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    public class AccessGroupRepository : GenericRepository<AccessGroup>, IAccessGroupRepository
    {
        public AccessGroupRepository(SmartDoorContext context) 
            : base(context)
        {
        }
    }
}
