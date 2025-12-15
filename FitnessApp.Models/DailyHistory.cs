using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    public class DailyHistory
    {
        public DateTime LogDate { get; set; }
        public double TotalCaloriesIn { get; set; } 
        public double TotalCaloriesBurned { get; set; } 
        public double CalorieDifference { get; set; }
    }
}
