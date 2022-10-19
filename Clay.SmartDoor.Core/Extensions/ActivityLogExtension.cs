using Clay.SmartDoor.Core.DTOs.ActivityLogs;
using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Extensions
{
    public static class ActivityLogExtension
    {
        public static ActivityLogDetails ToActivityLogDetails(this ActivityLog activityLog)
        {
            return new()
            {
                DoorNameTag = activityLog.DoorTag,
                Description = activityLog.Description,
                ActivityTime = activityLog.Time,
                Building = activityLog.Building,
                Floor = activityLog.Floor
            };
        }
    }
}
