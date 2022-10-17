using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Extensions
{
    public static class DoorExtension
    {
        public static DoorDetails ToDoorDetails(this Door door)
        {
            return new()
            {
                Id = door.Id,
                NameTag = door.NameTag,
                Floor = door.Floor,
                Building = door.Building
            };
        }
    }
}
