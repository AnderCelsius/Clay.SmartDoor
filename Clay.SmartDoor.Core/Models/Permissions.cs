namespace Clay.SmartDoor.Core.Models
{
    public class Permissions
    {
        public static List<string> GeneratePermissions(string module) => new()
        {
            $"Permissions.{module}.Delete",
            $"Permissions.{module}.Create",
        };

        public static List<string> GenerateAccessPermissions() => new()
        {
            Access.Create,
            Access.Grant,
            Access.Revoke
        };

        public static class User
        {
            public const string Delete = "Permissions.User.Delete";
            public const string Create = "Permissions.User.Create";
        }

        public static class Access
        {
            public const string Create = "Permissions.Access.Create";
            public const string Grant = "Permissions.Access.Grant";
            public const string Revoke = "Permissions.Access.Revoke";
        }
    }
}
