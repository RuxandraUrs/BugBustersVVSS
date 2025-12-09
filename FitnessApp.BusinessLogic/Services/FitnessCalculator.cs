using System;
using FitnessApp.BusinessLogic.Domains;

namespace FitnessApp.BusinessLogic.Services
{
    public class FitnessCalculator
    {
        private UserDomain _userDomain { get; set; }

        ///BMI (Body Mass Index): weight(kg) / height(m)^2
        public double CalculateBMI(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            return _userDomain.CalculateBMI(username);
        }

        /// BMR (Basal Metabolic Rate) : (10 x weight) + (6.25 x height) - (5 x age) + s ; Mifflin-St Jeor formula
        public double CalculateBMR(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");

            return _userDomain.CalculateBMR(username);
        }
    }
}