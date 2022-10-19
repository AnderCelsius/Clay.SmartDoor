using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Admin
{
    public class NewUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string AccessGroupId { get; set; } = string.Empty;
    }
}
