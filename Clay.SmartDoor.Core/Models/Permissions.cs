﻿namespace Clay.SmartDoor.Core.Models
{
    public class Permissions
    {
        public static List<string> GeneratePermissions(string module) => new()
        {
            $"Permissions.{module}.All",
            $"Permissions.{module}.General",
            $"Permissions.{module}.Store",
            $"Permissions.{module}.Create",
        };

        public static class Door
        {
            public const string All = "Permissions.Door.All";
            public const string General = "Permissions.Door.General";
            public const string Store = "Permissions.Door.Store";
            public const string Create = "Permissions.Door.Create";
        }
    }
}
