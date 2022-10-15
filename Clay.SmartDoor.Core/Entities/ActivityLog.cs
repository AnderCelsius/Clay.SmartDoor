using System.ComponentModel.DataAnnotations;

namespace Clay.SmartDoor.Core.Entities
{
    public class ActivityLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Time { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ActionBy { get; set; } = string.Empty;
        public string DoorId { get; set; } = Guid.NewGuid().ToString();
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string DoorTag { get; set; } = string.Empty;

        #region Navigational Properties
        public AppUser User { get; set; }
        #endregion
    }
}