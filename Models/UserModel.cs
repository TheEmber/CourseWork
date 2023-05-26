using CourseWork.Interfaces;
using System.Security.Cryptography;

namespace CourseWork.Models;

public class User : IHashable
{
    public Guid ID { get; set; }
    public String Email { get; set; }
    public String Password { get; set; }
    public String Name { get; set; }
    public String Surname { get; set; }
    public int RoleID { get; set; }
    public Role? Role { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
    // Retrieve existed user from database to verify password.
    public User(Guid id, string email, string password, string name, string surname, int roleid, Role role)
    {
        ID = id;
        Email = email;
        Password = password;
        Name = name;
        Surname = surname;
        RoleID = roleid;
        Role = role;
    }
    public User(string email, string password, string name, string surname)
    {
        ID = Guid.NewGuid();
        Email = email;
        Password = password;
        Name = name;
        Surname = surname;
        RoleID = 1;
    }
    // Register new user with new ID and hash passwords.
    public static User RegisterUser(string email, string password, string name, string surname)
    {
        return new User(Guid.NewGuid(), email, HashPassword(password), name, surname, 1, null!);
    }
    public static User RegisterUser(string email, string password, string name, string surname, Role role)
    {
        return new User(Guid.NewGuid(), email, HashPassword(password), name, surname, role.ID, role);
    }
    private static string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        using (Rfc2898DeriveBytes bytes = new(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }
        byte[] dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        return Convert.ToBase64String(dst);
    }

    public bool VerifyHashedPassword(string password)
    {
        byte[] buffer4;
        if (this.Password == null)
        {
            return false;
        }
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        byte[] src = Convert.FromBase64String(this.Password);
        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }
        byte[] dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        byte[] buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        using (Rfc2898DeriveBytes bytes = new(password, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }
        return ByteArraysEqual(buffer3, buffer4);
    }

    private static bool ByteArraysEqual(byte[] array1, byte[] array2)
    {
        if (array1 == array2)
            return true;
        if (array1 == null || array2 == null)
            return false;
        if (array1.Length != array2.Length)
            return false;

        foreach (byte b in array1)
        {
            bool found = false;
            foreach (byte c in array2)
            {
                if(b == c)
                {
                    found = true;
                    break;
                }
            }
            if(!found)
                return false;
        }
        return true;
    }
    public void ChangePassword(string oldPassword, string newPassword)
    {
        if(this.VerifyHashedPassword(oldPassword))
            this.Password = HashPassword(newPassword);
    }
}

public class Role
{
    public int ID { get; set; }
    public string Name { get; set; }
    public List<User>? Users { get; set; }
}