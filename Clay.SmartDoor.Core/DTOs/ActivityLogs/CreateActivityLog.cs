using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.DTOs.ActivityLogs
{
    public record CreateActivityLog(
        string Description,
        string DoorTag,
        string Building,
        string Floor)
    {
        public ActivityLog ToActivitylog(string userId, DateTime createdAt)
        {
            return new()
            {
                Time = createdAt,
                Description = Description,
                DoorTag = DoorTag,  
                Building = Building,
                Floor = Floor,
                ActionBy = userId
            };
        }
    }
}
