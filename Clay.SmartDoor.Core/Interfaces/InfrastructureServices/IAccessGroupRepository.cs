using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IAccessGroupRepository : IGenericRepository<AccessGroup>
    {
        Task<AccessGroup?> GetAccessGroupByNameAsync(string groupName);
        Task<AccessGroup?> GetAccessGroupByIdAsync(string id);
        IQueryable<AccessGroup> GetAccessGroupsByActiveStatusAsync(bool isActive);
    }
}