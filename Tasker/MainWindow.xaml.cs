using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Tasker
{
    public partial class MainWindow : Window
    {
        #region Fields
        private string FilePath;

        private List<Task> Tasks;

        // used to check if any of the items has actually changed
        private List<Task> TasksSnapshot;

        private bool WasSorted;

        #endregion


        #region Methods

        public MainWindow()
        {
            InitializeComponent();

            FilePath = "./tasks";

            ReadFromFile();

            TasksSnapshot = Task.CopyTasks(Tasks);

            dataGrid.ItemsSource = Tasks;
            dataGrid.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
        }

        private void ReadFromFile()
        {
            string tasksFromFile = null;

            if (File.Exists(FilePath))
            {
                using (StreamReader fs = new StreamReader(FilePath))
                {
                    tasksFromFile = fs.ReadToEnd();
                }

                Tasks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Task>>(tasksFromFile);
            }
            else
            {
                Tasks = new List<Task>();
            }
        }
        

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // NOTE this is a slightly better solution than saving to file every time the event is fired (which was happening)
            for (int i = 0; i < e.RemovedCells.Count; i++)
            {
                Task changed = e.RemovedCells[i].Item as Task;
                if (changed != null && (TasksSnapshot.Any(o => o.Description == changed.Description && o.Priority == changed.Priority && o.TimeNeededHours == changed.TimeNeededHours) == false))
                {
                    SaveToFile();
                    TasksSnapshot = Task.CopyTasks(Tasks);

                    break;
                }
            }

            WasSorted = false;
        }

        private void SaveToFile()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Tasks);
            File.WriteAllText(FilePath, json);
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
            if (WasSorted) return;

            for (int i = 0; i < Tasks.Count; i++)
            {
                int newBest = FindBestTask(i);

                if (newBest != i) Swap(i, newBest);
            }

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Tasks;

            WasSorted = true;

            SaveToFile();
        }

        private int FindBestTask(int currentIndex)
        {
            for (int i = currentIndex + 1; i < Tasks.Count; i++)
            {
                Task currentTask = Tasks[currentIndex];
                Task taskToCheck = Tasks[i];
                bool newPriorityHigher = taskToCheck.Priority >= currentTask.Priority;
                bool newTimeSmaller = taskToCheck.TimeNeededHours <= currentTask.TimeNeededHours;
                bool newTimeSmallerThanPriority = !newTimeSmaller && newPriorityHigher && taskToCheck.TimeNeededHours < currentTask.TimeNeededHours + (Math.Abs(taskToCheck.Priority - currentTask.Priority) * 2);
                bool newPriorityHigherThanTime = !newPriorityHigher && newTimeSmaller && taskToCheck.Priority > currentTask.Priority - (Math.Abs(currentTask.TimeNeededHours - taskToCheck.TimeNeededHours) * 0.5);

                if (newPriorityHigher && newTimeSmaller ||
                    newTimeSmallerThanPriority ||
                    newPriorityHigherThanTime)
                    currentIndex = i;
            }

            return currentIndex;
        }

        private void Swap(int oldIndex, int newIndex)
        {
            Task tmp = Tasks[oldIndex];
            Tasks[oldIndex] = Tasks[newIndex];
            Tasks[newIndex] = tmp;
        }

        #endregion
    }
}
