namespace Clay.SmartDoor.Core.DTOs.Admin
{
    public class AccessGroupResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<AssignedDoor> AssignedDoors { get; set; } = new List<AssignedDoor>();
    }

    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class AssignedDoor
    {
        public string DoorId { get; set; } = string.Empty;
        public bool Assigned { get; set; }
    }
}
