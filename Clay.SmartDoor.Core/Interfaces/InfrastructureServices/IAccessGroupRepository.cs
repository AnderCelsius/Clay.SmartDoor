using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IAccessGroupRepository : IGenericRepository<AccessGroup>
    {
        Task<AccessGroup?> GetAccessGroupsAsync(string groupName);
        IQueryable<AccessGroup> GetAccessGroupsByActiveStatusAsync(bool isActive);
    }
}