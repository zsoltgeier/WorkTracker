using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkTracker.Models;
using WorkTracker.Data;

namespace WorkTracker.ViewModels
{
    public class MainWindowViewModel : ObservableRecipient
    {
        private WorkTrackerDbContext _db;
        private ObservableCollection<EmployeeViewModel> _employees;
        private ObservableCollection<ActivityViewModel> _activities;
        private EmployeeViewModel _selectedEmployee;
        private string _employeeName;
        private string _selectedActivityTitle;
        private string _activityDescription;
        private string _activityMinutesText = "0"; // Default value
        private string _activityHoursText = "0"; // Default value
        private string _activityComment;

        public MainWindowViewModel()
        {
            // Initialize the database context and commands
            _db = new WorkTrackerDbContext();
            _employees = new ObservableCollection<EmployeeViewModel>(
                _db.Employees.Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    Name = e.Name
                }));

            _selectedEmployee = _employees.FirstOrDefault();
            AddEmployeeCommand = new RelayCommand(AddEmployee);
            LogActivityCommand = new RelayCommand(LogActivity);
            ViewActivitiesCommand = new RelayCommand(ViewActivities);
            ExportCommand = new RelayCommand(Export);
        }

        // Properties and collections for data binding
        public ObservableCollection<EmployeeViewModel> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        public ObservableCollection<ActivityViewModel> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }

        public EmployeeViewModel SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        public string EmployeeName
        {
            get => _employeeName;
            set => SetProperty(ref _employeeName, value);
        }

        public List<string> ActivityTitleOptions { get; } = new List<string> { "Fejlesztés", "Tesztelés", "Megbeszélés" };
        public string SelectedActivityTitle
        {


            get => _selectedActivityTitle;
            set => SetProperty(ref _selectedActivityTitle, value);
        }

        public string ActivityDescription
        {
            get => _activityDescription;
            set => SetProperty(ref _activityDescription, value);
        }

        public ObservableCollection<int> ActivityMinutesOptions { get; } = new ObservableCollection<int> { 0, 30 };
        public string ActivityMinutesText
        {
            get => _activityMinutesText;
            set => SetProperty(ref _activityMinutesText, value);
        }

        public ObservableCollection<int> ActivityHoursOptions { get; } = new ObservableCollection<int> { 1, 2, 4 };
        public string ActivityHoursText
        {
            get => _activityHoursText;
            set => SetProperty(ref _activityHoursText, value);
        }

        public string ActivityComment
        {
            get => _activityComment;
            set => SetProperty(ref _activityComment, value);
        }

        public RelayCommand AddEmployeeCommand { get; }
        public RelayCommand LogActivityCommand { get; }
        public RelayCommand ViewActivitiesCommand { get; }
        public RelayCommand ExportCommand { get; }

        // Add an employee to the database
        private void AddEmployee()
        {
            string name = EmployeeName?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                // Show an error message if the name is empty
                MessageBox.Show("Adja meg a dolgozó nevét.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsNameValid(name))
            {
                // Show an error message if the name contains non-text characters
                MessageBox.Show("A dolgozó neve csak szöveg karaktereket tartalmazhat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var employee = new Employee { Name = name };
            _db.Employees.Add(employee);
            _db.SaveChanges();

            var newEmployeeViewModel = new EmployeeViewModel { Id = employee.Id, Name = employee.Name };
            Employees.Add(newEmployeeViewModel);

            // Set the newly added employee as the selected employee
            SelectedEmployee = newEmployeeViewModel;

            EmployeeName = string.Empty;
        }

        // Log an activity
        private void LogActivity()
        {
            if (SelectedEmployee == null)
            {
                // Show an error message if no employee is selected
                MessageBox.Show("Válasszon ki egy dolgozót.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string title = SelectedActivityTitle?.Trim();
            string hoursText = ActivityHoursText?.Trim();
            string minutesText = ActivityMinutesText?.Trim();

            if (string.IsNullOrEmpty(title))
            {
                // Show an error message if the title is empty
                MessageBox.Show("Adja meg a végzett munka megnevezését.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(hoursText, out int hours) || hours < 0 || !int.TryParse(minutesText, out int minutes) || minutes < 0 || minutes >= 60 || (hours * 60 + minutes) <= 0 || (hours * 60 + minutes) > 720)
            {
                // Show an error message for invalid or out-of-range duration
                MessageBox.Show("Érvénytelen időtartamot adott meg.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TimeSpan duration = TimeSpan.FromMinutes(hours * 60 + minutes);

            using (var context = new WorkTrackerDbContext())
            {
                var activity = new Activity
                {
                    Title = title,
                    Description = ActivityDescription?.Trim() ?? string.Empty,
                    Duration = duration,
                    Comment = ActivityComment?.Trim() ?? string.Empty,
                    Date = DateTime.Now,
                    EmployeeId = SelectedEmployee.Id
                };

                context.Activities.Add(activity);
                context.SaveChanges();
            }
            // Reset to default values
            SelectedActivityTitle = string.Empty;
            ActivityDescription = string.Empty;
            ActivityHoursText = "0";
            ActivityMinutesText = "0";
            ActivityComment = string.Empty;
        }

        // View activities for the selected employee
        private void ViewActivities()
        {
            if (SelectedEmployee != null)
            {
                using (var context = new WorkTrackerDbContext())
                {
                    var activities = context.Activities
                        .Where(a => a.EmployeeId == SelectedEmployee.Id)
                        .OrderByDescending(a => a.Date)
                        .Select(a => new ActivityViewModel
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Description = a.Description,
                            Duration = a.Duration,
                            Comment = a.Comment,
                            Date = a.Date,
                            EmployeeId = a.EmployeeId
                        })
                        .ToList();

                    Activities = new ObservableCollection<ActivityViewModel>(activities);
                }
            }
        }

        // Export activities to a CSV file
        private void Export()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = new DateTime(today.Year, today.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = "activity_summary.csv"
            };

            try
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        var activities = _db.Activities
                            .Where(a => a.Date >= startDate && a.Date < endDate) // Filter by date range
                            .Select(a => new
                            {
                                EmployeeName = a.Employee.Name,
                                ActivityTitle = a.Title,
                                DurationHours = a.Duration.TotalHours
                            })
                            .OrderBy(a => a.EmployeeName) // Sort by EmployeeName
                            .ToList(); // Fetch filtered and sorted activities from the database

                        var summary = activities
                            .GroupBy(a => new { a.EmployeeName, a.ActivityTitle })
                            .Select(group => new
                            {
                                EmployeeName = group.Key.EmployeeName,
                                ActivityTitle = group.Key.ActivityTitle,
                                TotalHours = group.Sum(a => a.DurationHours)
                            })
                            .ToList(); // Perform Sum operation in memory

                        csv.WriteRecords(summary);
                    }

                    // Display a success message
                    MessageBox.Show("Sikeres exportálás!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Display an error message
                MessageBox.Show($"Hiba történt: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Check if a name contains only text characters and spaces
        private bool IsNameValid(string name)
        {
            // Validate that the name contains only text characters and spaces
            return !string.IsNullOrEmpty(name) && name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }
    }
}
