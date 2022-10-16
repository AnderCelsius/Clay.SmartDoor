using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Dtos
{
    public record CreateDoorRecord(
        string NameTag,
        string CreatorId,
        string Building,
        string Floor)
    {
        public Door ToDoor(DateTime createdAt, DateTime lastModified)
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                NameTag = NameTag,
                CreatorBy = CreatorId,
                Building = Building,
                Floor = Floor,
                CreatedAt = createdAt,
                LastModified = lastModified
            };
        }
    }
}
