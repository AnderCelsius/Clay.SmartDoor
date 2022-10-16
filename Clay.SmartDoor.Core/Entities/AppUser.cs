using Microsoft.AspNetCore.Identity;

namespace Clay.SmartDoor.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        #region Navigational Properties
        public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        #endregion
    }
}
