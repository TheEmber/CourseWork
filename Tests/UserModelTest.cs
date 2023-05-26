using CourseWork.Models;
using Xunit;

namespace CourseWork.Tests;
public class UserModelTest
{
    // Test hashing and verifying
    [Fact]
    public void HashTest()
    {
        const string rightPassword = "JKs^$,fhjs*&*&558852";

        User user = User.RegisterUser("fdshhjk@dsa.das", rightPassword, "name", "surname");

        // Password should be hashed and not equals to input password
        Assert.True(user.Password != rightPassword);
    }
    [Fact]
    public void HashRandomTest()
    {
        const string password = "fmodsuyguhfds";

        User user1 = User.RegisterUser("fdshhjk@dsa.das", password, "name", "surname");
        User user2 = User.RegisterUser("fdshhjk@dsa.das", password, "name", "surname");

        Assert.False(user1.Password == user2.Password);
    }
    [Fact]
    public void HashVerificationTest_wrongPassword()
    {
        const string rightPassword = "%klfdhuyu*7sjk";
        const string wrongPassword = "KdjhgvnNG4477/";

        User user = User.RegisterUser("fdshhjk@dsa.das", rightPassword, "name", "surname");

        // Hashed password verification should be failed with wrong password
        Assert.False(user.VerifyHashedPassword(wrongPassword));
    }
    [Fact]
    public void HashVerificationTest_rightPassword()
    {
        const string rightPassword = "djihg&*%$3bhjk/";

        User user = User.RegisterUser("fdshhjk@dsa.das", rightPassword, "name", "surname");

        Assert.True(user.VerifyHashedPassword(rightPassword));
    }
    [Fact]
    public void ChangePasswordTest()
    {
        const string oldPassword = "fjdshu7YGHBfty";
        const string newPassword = "fmkodsjhugy&*()OIPKLJ";

        User user = User.RegisterUser("fdshhjk@dsa.das", oldPassword, "name", "surname");

        user.ChangePassword(oldPassword, newPassword);

        // Hashed password verification should be failed when password changed
        Assert.False(user.VerifyHashedPassword(oldPassword));
        // Hashed password verification should be successful with new password after changing
        Assert.True(user.VerifyHashedPassword(newPassword));
    }
    [Fact]
    public void ChangePassword_wrongPassword()
    {
        const string oldPassword = "fdsjniy*^Gdas";
        const string newPassword = "dsaui^&Gvueqw";

        User user = User.RegisterUser("fdshhjk@dsa.das", oldPassword, "name", "surname");

        user.ChangePassword(newPassword, newPassword);

        Assert.True(user.VerifyHashedPassword(oldPassword));
        Assert.False(user.VerifyHashedPassword(newPassword));
    }
    [Fact]
    public void GuidGenerationTest()
    {
        User user1 = new("hfusdbhu@dfsa.das", "password", "name", "surname");
        User user2 = new("hfusdbhu@dfsa.das", "password", "name", "surname");

        Assert.True(user1.ID != user2.ID);
    }
    [Fact]
    public void DefaultRoleTest()
    {
        User user1 = new("hfusdbhu@dfsa.das", "password", "name", "surname");
        User user2 = new("hfusdbhu@dfsa.das", "password", "name", "surname");

        Assert.True(user1.RoleID == user2.RoleID);
        Assert.True(user1.RoleID == 1);
    }
    [Fact]
    public void AssignedGuidTest()
    {
        User user = new(Guid.Empty, "hfusdbhu@dfsa.das", "password", "name", "surname", 5, null!);

        Assert.True(user.ID == Guid.Empty);
    }
    [Fact]
    public void AssignedRoleTest()
    {
        Role role = new();
        User user = new(Guid.Empty, "dsjihuy@dsa.dsaa", "password", "name", "surname", role.ID, role);

        Assert.True(user.RoleID == user.Role.ID);
    }
    [Fact]
    public void RegisterUserTest()
    {
        const string password1 = "fpdksojiu7890";
        const string password2 = "omdisuy768YUHIYG";
        User user1 = User.RegisterUser("email", password1, "name", "surname");
        User user2 = User.RegisterUser("email", password2, "name", "surname");

        Assert.True(user1.RoleID == 1);
        Assert.True(user1.ID != user2.ID);
        Assert.True(user1.Password != user2.Password);
        Assert.True(user1.VerifyHashedPassword(password1));
        Assert.True(user2.VerifyHashedPassword(password2));
    }
    [Fact]
    public void RegisterWithRoleTest()
    {
        Role role = new()
        {
            ID = 5,
            Name = "TestRole"
        };
        User user = User.RegisterUser("email", "password", "name", "surname", role);

        Assert.True(user.Role == role);
        Assert.True(user.RoleID == role.ID);
    }
}