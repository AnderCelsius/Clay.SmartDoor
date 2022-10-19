using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Doors
{
    public class DoorAccessRequest
    {
        [Required]
        public string DoorId { get; set; } = string.Empty;

        [Required]
        public string AccessGroupId { get; set; } = string.Empty;
    }
}
