namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IUnitOfWork : IDisposable
    {
        IDoorRepository Doors { get; }
        IActivityLogRepository ActivityLogs { get; }
        Task<int> SaveAsync();
    }
}
