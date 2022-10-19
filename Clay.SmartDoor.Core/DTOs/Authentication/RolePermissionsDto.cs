namespace Clay.SmartDoor.Core.DTOs.Authentication
{
    public class RolePermissionsDto
    {
        public string RoleId { get; set; } = string.Empty;
        public IList<ClaimDto> RoleClaims { get; set; } = new List<ClaimDto>();
    }
}
