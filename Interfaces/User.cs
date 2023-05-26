using CourseWork.Models;

namespace CourseWork.Users;

public interface IHashable
{
    bool VerifyHashedPassword(string password);
    void ChangePassword(string oldPassword, string newPassword);
}