using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Admin
{
    public class NewUserRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        public string GroupId { get; set; } = string.Empty;
    }
}
