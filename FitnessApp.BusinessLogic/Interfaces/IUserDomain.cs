using FitnessApp.Models;

namespace FitnessApp.BusinessLogic.Domains
{
    public interface IUserDomain
    {
        User GetUserByUsername(string username);
        void AddUser(User user);
        //void UpdateUser(User user);
        //void DeleteUser(User user);
        void RegisterUser(User user);
        User Login(string username, string password);

        double CalculateBMI(string username);
        double CalculateBMR(string username);
        int CalculateRemainingCalories(string username, int consumed, int burned);
        bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories);
        string GetDailyProgressStatus(string username, int consumed);
    }
}