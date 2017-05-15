namespace Tasker
{
    public class Task
    {
        private double timeNeededHours;
        private int priority;
        private string description;

        public int Priority
        {
            get
            {
                return priority;
            }

            set
            {
                if (value < 0 || value > 10) throw new System.Exception("Out of range");
                priority = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public double TimeNeededHours
        {
            get
            {
                return timeNeededHours;
            }

            set
            {
                timeNeededHours = value;
            }
        }
    }
}