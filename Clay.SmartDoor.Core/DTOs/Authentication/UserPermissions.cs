namespace Clay.SmartDoor.Core.DTOs.Authentication
{
    public class UserPermissions
    {
        public string UserId { get; set; } = string.Empty;
        public IList<ClaimDto> Claims { get; set; } = new List<ClaimDto>();
    }
}
