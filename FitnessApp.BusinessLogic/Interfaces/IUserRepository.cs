using FitnessApp.Models;

namespace FitnessApp.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        // need for login step
        User GetUserByUsername(string username);

        // need for registering a new user
        void AddUser(User user); //should add a new parameter: hashedPassword
    }
}
