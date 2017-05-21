using System.Collections.Generic;

namespace Tasker
{
    public class Task
    {
        #region Fields

        private double timeNeededHours;
        private int priority;
        private string description;

        #endregion


        #region Constructors

        public Task() { }
        public Task(Task original)
        {
            this.TimeNeededHours = original.TimeNeededHours;
            this.Priority = original.Priority;
            this.Description = original.Description;
        }

        #endregion


        #region Properties

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

        #endregion


        #region Methods

        public static List<Task> CopyTasks(List<Task> tasks)
        {
            List<Task> copyList = new List<Task>();

            foreach (Task original in tasks)
                copyList.Add(new Task(original));

            return copyList;
        }

        #endregion
    }
}