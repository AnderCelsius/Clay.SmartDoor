using Clay.SmartDoor.Core.DTOs.Admin;
using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Extensions
{
    public static class AccessGroupExtension
    {
        public static AccessGroupResponse ToAccessGroupResponse(this AccessGroup accessGroup)
        {
            return new()
            {
                Id = accessGroup.Id,
                Name = accessGroup.Name,
                IsActive = accessGroup.IsActive,
                CreatedAt = accessGroup.CreatedAt,
                LastModified = accessGroup.LastModified,
                Users = accessGroup.Users.Select(x => new User
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedDate,
                    LastModified = x.LastModified,
                }).ToList(),
                AssignedDoors = accessGroup.DoorAssignment.Select( x => new AssignedDoor
                {
                    DoorId = x.DoorId,
                    Assigned = x.Assigned
                }).ToList()
            };
        }
    }
}
