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
        public string ActivityName { get; private set; }
        public int DurationMinutes { get; private set; }
        public int CaloriesBurned { get; private set; }


        Workout() { }

        public Workout(string activityName, int duration, int calories)
        {
            if (string.IsNullOrWhiteSpace(activityName))
                throw new ArgumentException("Activity name is required.");

            if (duration <= 0)
                throw new ArgumentException("Training duration must be positive.");

            if (duration > 480) 
                throw new ArgumentException("Duration is suspiciously high (> 8 hours).");

            if (calories < 0)
                throw new ArgumentException("Calories burned cannot be negative.");

            ActivityName = activityName;
            DurationMinutes = duration;
            CaloriesBurned = calories;
        }
    }
}
