using FitnessApp.Models;
namespace FitnessApp.BusinessLoic.Interfaces
{
    public interface IAuthenticationService
    {
        void RegisterUser(User user);
        User Login(string username, string password);
    }
}
