namespace Clay.SmartDoor.Core.DTOs.ActivityLogs
{
    public class ActivityLogsRequest : QueryStringParameters
    {
        public string UserId { get; set; } = string.Empty;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
