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
        public string FoodName { get; private set; }
        public double Calories { get; private set; }
        public double QuantityGrams { get; private  set; }
        public MealType Type { get; set; }

        public Meal() { }

        public Meal(string name, double calories, double quantity,  MealType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Food Name is mandatory!");

            if (calories < 0)
                throw new ArgumentException("Calories cannot be negative!");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive!");
            FoodName = name;
            Calories = calories;
            QuantityGrams = quantity;
            Type = type;
        }

    }
}
