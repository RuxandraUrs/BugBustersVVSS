using FitnessApp.Models;

namespace FitnessApp.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        // need for registering a new user
        void AddUser(User user);
        // need for login step
        User GetUserByUsername(string username);

        void UpdateUserStats(User user);
        void DeleteUser(int userId);
    }
}
