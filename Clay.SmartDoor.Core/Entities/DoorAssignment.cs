﻿namespace Clay.SmartDoor.Core.Entities
{
    public class DoorAssignment : BaseEntity
    {
        public string DoorId { get; set; } = string.Empty;
        public string GroupId { get; set;} = string.Empty;
        public AccessGroup AccessGroup { get; set; } = null!;
    }
}
