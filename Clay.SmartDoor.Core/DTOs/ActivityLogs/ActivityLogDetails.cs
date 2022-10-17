namespace Clay.SmartDoor.Core.DTOs.ActivityLogs
{
    public class ActivityLogDetails
    {
        public DateTime ActivityTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DoorNameTag { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
    }
}
