namespace Clay.SmartDoor.Core.Entities
{
    public class Door : BaseEntity
    {
        public string Tag { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
    }
}
