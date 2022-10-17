﻿using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.DTOs.Doors
{
    public class OpenDoor
    {
        [Required]
        public string DoorId { get; set; } = string.Empty;

        [Required]
        public string GroupId { get; set; } = string.Empty;
    }
}
