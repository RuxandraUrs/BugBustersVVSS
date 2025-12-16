using System;

namespace FitnessApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get;  set; } 
        public string PasswordHash { get; set; }
        public string Email { get;  set; } 
        public DateTime BirthDate { get; set; } 
        public string Gender { get; set; } 
        public double HeightCm { get; set; } 
        public double WeightKg { get; set; } 
        public double TargetWeightKg { get; set; } 
        public int DailyCalorieGoal { get; set; } 
        public DateTime CreatedAt { get; set; }


        public User() { }

        public User(string username, string email, double weight, double height, double targetWeight, int calorieGoal)
        {
            SetUsername(username);
            SetEmail(email);
            SetWeight(weight);

            HeightCm = height;
            TargetWeightKg = targetWeight;
            DailyCalorieGoal = calorieGoal;
            CreatedAt = DateTime.Now;
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is mandatory!");

            if(username.Length < 4)
                throw new ArgumentException("Username must be at least 4 characters long!");

            Username = username;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is mandatory!");
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Invalid email format!");
            Email = email;
        }

        public void SetWeight(double weight)
        {
            if (weight <= 30 || weight > 300)
                throw new ArgumentException("Invalid weight. Must be between 30 and 300 kg");

            WeightKg = weight;
        }
    }
}
