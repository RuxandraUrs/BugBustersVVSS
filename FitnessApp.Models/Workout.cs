using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public int LogId { get; set; }
        public string ActivityName { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }

        public Workout(string name, int duration, int calories)
        {
            ActivityName = name;
            DurationMinutes = duration;
            CaloriesBurned = calories;
        }
    }
}
