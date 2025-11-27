using FitnessApp.Models;
namespace FitnessApp.BusinessLogic.Interfaces
{
    public interface IAuthenticationService
    {
        void RegisterUser(User user);
        User Login(string username, string password);
    }
}
