using FitnessApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public string FoodName { get; set; }
        public double Calories { get; set; }
        public double QuantityGrams { get; set; }
        public MealType Type { get; set; }

        public Meal() { }

        public Meal(string name, double calories, double grams, MealType type)
        {
            FoodName = name;
            Calories = calories;
            QuantityGrams = grams;
            Type = type;
        }

    }
}
