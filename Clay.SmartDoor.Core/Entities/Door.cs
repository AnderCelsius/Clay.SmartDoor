using System.ComponentModel.DataAnnotations.Schema;

namespace Clay.SmartDoor.Core.Entities
{
    public class Door : BaseEntity
    {
        [Column("Name_Tag")]
        public string NameTag { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
    }
}
