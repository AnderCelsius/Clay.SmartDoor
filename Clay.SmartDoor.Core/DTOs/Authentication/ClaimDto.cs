namespace Clay.SmartDoor.Core.DTOs.Authentication
{
    public class ClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool Selected { get; set; } 
    }
}
