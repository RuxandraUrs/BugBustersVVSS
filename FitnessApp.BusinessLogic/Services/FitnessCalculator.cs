using System;

namespace FitnessApp.BusinessLogic.Services
{
    public class FitnessCalculator
    {

        ///BMI (Body Mass Index): weight(kg) / height(m)^2
        
        public double CalculateBMI(double weightKg, double heightCm)
        {
            if (heightCm <= 0 || weightKg <= 0) return 0;

            double heightM = heightCm / 100.0;
            double bmi = weightKg / (heightM * heightM);

            return Math.Round(bmi, 2); 
        }

        /// BMR (Basal Metabolic Rate) : (10 x weight) + (6.25 x height) - (5 x age) + s ; Mifflin-St Jeor formula

        public double CalculateBMR(double weightKg, double heightCm, int age, string gender)
        {
            if (weightKg <= 0 || heightCm <= 0 || age <= 0) return 0;

            double bmr = (10 * weightKg) + (6.25 * heightCm) - (5 * age);

            if (gender != null && gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
            {
                bmr += 5;
            }
            else
            {
                bmr -= 161;
            }

            return Math.Round(bmr, 0); 
        }
    }
}