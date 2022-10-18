using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Test.Helper
{
    internal class TestDataGenerator
    {
        private const string Default_Password = "Password@123";
        private const string Default_AccessGroup = "33e09d95-60c1-41ed-a2ae-faff5e711078";
        private const string Default_SecureGroup_One = "ba63545f-2c49-4954-983c-bef094a4027a";

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
    }
}
