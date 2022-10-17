namespace Clay.SmartDoor.Core.Entities
{
    public class AccessGroup : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<DoorAssignment> DoorAssignment { get; set; } = new List<DoorAssignment>();
    }
}
