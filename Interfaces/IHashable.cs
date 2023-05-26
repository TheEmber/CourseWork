using CourseWork.Models;

namespace CourseWork.Interfaces;

public interface IHashable
{
    bool VerifyHashedPassword(string password);
    void ChangePassword(string oldPassword, string newPassword);
}