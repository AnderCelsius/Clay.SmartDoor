namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IUnitOfWork : IDisposable
    {
        IDoorRepository Doors { get; }
        IActivityLogRepository ActivityLogs { get; }
        IAccessGroupRepository AccessGroups { get; }
        IDoorAssignmentRepository DoorAssignments { get; }
        Task<int> SaveAsync();
    }
}
