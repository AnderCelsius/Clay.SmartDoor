using System.ComponentModel.DataAnnotations.Schema;

namespace Clay.SmartDoor.Core.Entities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CreatedBy { get; set; } = string.Empty;
 
        [Column("Date_Created")]
        public DateTime CreatedAt { get; set; }

        [Column("Last_Modified_Date")]
        public DateTime LastModified { get; set; }
    }
}
