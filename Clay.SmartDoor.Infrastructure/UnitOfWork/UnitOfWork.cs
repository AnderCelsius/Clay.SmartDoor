﻿using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Clay.SmartDoor.Infrastructure.Repositories;

namespace Clay.SmartDoor.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartDoorContext _smartDoorContext;
        private IDoorRepository _doors { get; set; } = null!;
        private IActivityLogRepository _activityLogs { get; set; } = null!;

        public UnitOfWork(SmartDoorContext smartDoorContext)
        {
            _smartDoorContext = smartDoorContext;
        }
        public IDoorRepository Doors => _doors ??= new DoorRepository(_smartDoorContext);

        public IActivityLogRepository ActivityLogs => _activityLogs ??= new ActivityLogRepository(_smartDoorContext);

        public async Task<int> SaveAsync()
        {
            return await _smartDoorContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _smartDoorContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
