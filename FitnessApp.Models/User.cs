using System;

namespace FitnessApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } 
        public string PasswordHash { get; set; }
        public string Email { get; set; } 
        public DateTime BirthDate { get; set; } 
        public string Gender { get; set; } 
        public double HeightCm { get; set; } 
        public double WeightKg { get; set; } 
        public double TargetWeightKg { get; set; } 
        public int DailyCalorieGoal { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
