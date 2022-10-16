namespace Clay.SmartDoor.Core.DTOs
{
    public class PermissionsDto
    {
        public string RoleId { get; set; } = string.Empty;
        public IList<RoleClaimDto> RoleClaims { get; set; } = new List<RoleClaimDto>();
    }
}
