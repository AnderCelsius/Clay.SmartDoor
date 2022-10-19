namespace Clay.SmartDoor.Core.DTOs.Admin
{
    public class UpdateUserAccessGroup
    {
        public string UserId { get; set; } = string.Empty;
        public string NewAccessGroupId { get; set; } = string.Empty;
        public string OldAccessGroupId { get; set; } = string.Empty;
    }
}
