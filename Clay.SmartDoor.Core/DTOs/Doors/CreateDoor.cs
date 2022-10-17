using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.DTOs.Doors
{
    public record CreateDoorRecord(
        string NameTag,
        string Building,
        string Floor)
    {
        public Door ToDoor(DateTime createdAt, DateTime lastModified, string creatorId)
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                NameTag = NameTag,
                CreatedBy = creatorId,
                Building = Building,
                Floor = Floor,
                CreatedAt = createdAt,
                LastModified = lastModified
            };
        }
    }
}
