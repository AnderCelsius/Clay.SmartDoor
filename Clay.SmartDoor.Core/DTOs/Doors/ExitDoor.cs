using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Doors
{
    public class ExitDoor
    {
        [Required]
        public string DoorId { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
