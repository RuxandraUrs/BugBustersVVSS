using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.BusinessLogic.Domains
{
    public interface IUserDomain
    {
        double CalculateBMI(string username);
        double CalculateBMR(string username);
        int CalculateRemainingCalories(string username, int consumed, int burned);
        bool WouldExceedBudget(string username, int burnedToday, int currentTotal, int newFoodCalories);
        string GetDailyProgressStatus(string username, int consumed);
    }
}