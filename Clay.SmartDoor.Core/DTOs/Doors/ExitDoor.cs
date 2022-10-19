using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Doors
{
    public class ExitDoor
    {
        public string DoorId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
