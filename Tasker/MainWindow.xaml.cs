using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Task> Tasks;
        public bool WasSegregated;

        public MainWindow()
        {
            InitializeComponent();

            ReadTasksFromFile();

            dataGrid.ItemsSource = Tasks;
            dataGrid.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(Tasks);
            System.IO.File.WriteAllText(@"C:/tasks.txt", json);
            WasSegregated = false;
        }

        private void ReadTasksFromFile()
        {
            string tasksFromDesktop = null;
            if (File.Exists("C:/tasks.txt"))
            {
                using (StreamReader fs = new StreamReader("C:/tasks.txt"))
                {
                    tasksFromDesktop = fs.ReadToEnd();
                }

                Tasks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Task>>(tasksFromDesktop);
            }
            else
                Tasks = new List<Task>();
        }


        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
            if (WasSegregated) return;

            for (int i = 0; i < Tasks.Count; i++)
            {
                int newBest = LoopAndFindBest(i);
                Task tmp = Tasks[i];

                Tasks[i] = Tasks[newBest];
                Tasks[newBest] = tmp;
            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Tasks;
            WasSegregated = true;
        }

        private int LoopAndFindBest(int currentIndex)
        {
            for (int i = currentIndex + 1; i < Tasks.Count; i++)
            {
                if ((Tasks[i].Priority > Tasks[currentIndex].Priority && Tasks[i].TimeNeededHours < Tasks[currentIndex].TimeNeededHours) ||
                    (Tasks[i].Priority < Tasks[currentIndex].Priority && Tasks[i].TimeNeededHours < Tasks[currentIndex].TimeNeededHours && (Tasks[currentIndex].TimeNeededHours > Tasks[i].TimeNeededHours + (Math.Abs(Tasks[i].Priority - Tasks[currentIndex].Priority) * 2))) ||
                    (Tasks[i].TimeNeededHours > Tasks[currentIndex].TimeNeededHours && Tasks[i].Priority > Tasks[currentIndex].Priority && (Tasks[currentIndex].TimeNeededHours < Tasks[i].TimeNeededHours + (Math.Abs(Tasks[i].Priority - Tasks[currentIndex].Priority) * 2))) ||
                    (Tasks[i].Priority > Tasks[currentIndex].Priority && (Tasks[i].TimeNeededHours < Tasks[currentIndex].TimeNeededHours+(Math.Abs(Tasks[currentIndex].TimeNeededHours - Tasks[i].TimeNeededHours)*2))))
                    currentIndex = i;
            }

            return currentIndex;
        }
    }
}
