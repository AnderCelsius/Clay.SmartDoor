using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clay.SmartDoor.Test.Helper
{
    internal static class MockHelpers
    {
        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(List<TRole> roles) where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            var roleValidators = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>()
            };
            var mgr = new Mock<RoleManager<TRole>>(store.Object, roleValidators, MockLookupNormalizer(),
                new IdentityErrorDescriber(), null);

            mgr.Setup(x => x.CreateAsync(It.IsAny<TRole>()))
                .ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => ls.Add(x));

            return mgr;
        }

        private static ILookupNormalizer MockLookupNormalizer()
        {
            var normalizerFunc = new Func<string, string>(i =>
            {
                return i == null ? null! : Convert.ToBase64String(Encoding.UTF8.GetBytes(i)).ToUpperInvariant();
            });
            var lookupNormalizer = new Mock<ILookupNormalizer>();
            lookupNormalizer.Setup(i => i.NormalizeName(It.IsAny<string>())).Returns(normalizerFunc);
            lookupNormalizer.Setup(i => i.NormalizeEmail(It.IsAny<string>())).Returns(normalizerFunc);
            return lookupNormalizer.Object;
        }
    }
}
