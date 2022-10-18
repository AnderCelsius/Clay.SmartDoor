using Clay.SmartDoor.Core.DTOs.Doors;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Models.Constants;
using System;
using System.Collections.Generic;

namespace Clay.SmartDoor.Test.Helper
{
    internal class TestDataGenerator
    {
        public const string Default_Id = "33e09d95-51d3-41ed-a2ae-faff5e711078";
        public const string Default_Door_Id = "bef094a4027a-51d3-41ed-a2ae-ba63545f";
        public const string Default_AccessGroup = "33e09d95-60c1-41ed-a2ae-faff5e711078";
        public const string Default_SecureGroup_One = "ba63545f-2c49-4954-983c-bef094a4027a";

        public static AppUser SuperAdminUser = new()
        {
            FirstName = "Obinna",
            LastName = "Asiegbulam",
            Email = "superadminuser@email.com",
            UserName = "superadminuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            AccessGroupId = Default_SecureGroup_One
        };

        public static AppUser AdminUser = new()
        {
            FirstName = "Joshua",
            LastName = "Enyi",
            Email = "adminuser@email.com",
            UserName = "adminuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            AccessGroupId = Default_SecureGroup_One
        };

        public static AppUser BasicUser = new()
        {
            FirstName = "Omowunmi",
            LastName = "Kassim",
            Email = "basicuser@email.com",
            UserName = "basicuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            AccessGroupId = Default_AccessGroup
        };

        public static AppUser InActiveUser = new()
        {
            FirstName = "Omowunmi",
            LastName = "Kassim",
            Email = "basicuser@email.com",
            UserName = "basicuser@email.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = false,
            AccessGroupId = Default_AccessGroup
        };

        public static string ActionBy = Default_Id;
        public static CreateDoorRecord RequestModel = new("Main Door", "Uno", "1st Floor");

        public static Door DefaultDoor = new()
        {
            Id = Default_Door_Id,
            NameTag = RequestModel.NameTag,
            CreatedAt = DateTime.Now,
            LastModified = DateTime.Now,
            CreatedBy = Default_Id,
            Building = RequestModel.Building,
            Floor = RequestModel.Floor
        };

        public static ActivityLog activityLog = new()
        {
            Time = DateTime.Now,
            Description = ActivityDescriptions.Door_Created,
            ActionBy = ActionBy,
            DoorId = DefaultDoor.Id,
            Building = RequestModel.Building,
            Floor = RequestModel.Floor,
            DoorTag = DefaultDoor.NameTag,
        };

        public static IEnumerable<Door> GenerateDummyDoors(List<string> doorIds)
        {
            List<Door> doors = new();
            int count = 0;
            foreach(var id in doorIds)
            {
                count++;
                doors.Add(new Door
                {
                    Id = id,
                    NameTag = $"{RequestModel.NameTag}-{count}",
                    Floor = RequestModel.Floor,
                    Building = RequestModel.Building,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = SuperAdminUser.Id
                });
            }

            return doors;
        }

    }
}
