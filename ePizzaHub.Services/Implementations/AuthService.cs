using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Impelmentations;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementations
{
    public class AuthService : Service<User>, IAuthService
    {
        IUserRepository _userRepo
        {
            get
            {
              return  _repo as UserRepository;
            }
        }
        public AuthService(IUserRepository userRepo) : base(userRepo)
        {
           
        }

        public bool CreateUser(User user, string Role)
        {
           return _userRepo.CreateUser(user, Role);
        }

        public UserModel ValidateUser(string Email, string Password)
        {
            return _userRepo.ValidateUser(Email, Password);
        }
    }
}
