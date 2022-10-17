using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.ActivityLogs
{
    public class ActivityLogsRequest : QueryStringParameters
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}
