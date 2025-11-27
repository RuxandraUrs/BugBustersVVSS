using System.Collections.Generic;

namespace FitnessApp.BusinessLogic.ExternalApi
{
    public class NinjaFoodItem
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Serving_Size_G { get; set; }
        public double Protein_G { get; set; }
        public double Fat_Total_G { get; set; }
        public double Carbohydrates_Total_G { get; set; }
    }
}