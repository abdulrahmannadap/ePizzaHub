using ePizzaHub.Core.Database;
using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Impelmentations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db): base(db)
        {
            
        }
        public bool CreateUser(User user, string Role)
        {
            try
            {
                Role role = _db.Roles.Where(r => r.Name == Role).FirstOrDefault()!;
                if (role != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.Roles.Add(role);

                    _db.Users.Add(user);
                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public UserModel ValidateUser(string Email, string Password)
        {
            User user = _db.Users.Include(u=>u.Roles).Where(u => u.Email == Email).FirstOrDefault()!;
            if (user!=null)
            {
                bool isverified = BCrypt.Net.BCrypt.Verify(Password, user.Password);
                if (isverified)
                {
                    UserModel model = new UserModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = user.Roles.Select(r => r.Name).ToArray()
                    };
                    return model;
                }
            }
            return null;
        }
    }
}
