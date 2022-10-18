using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IDoorRepository : IGenericRepository<Door>
    {
        Task<Door?> GetDoorAsync(string doorId);
        IQueryable<Door> GetAllDoorsAsync();
    }
}
