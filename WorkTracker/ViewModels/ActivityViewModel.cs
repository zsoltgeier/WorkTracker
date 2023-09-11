using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.ViewModels
{
    public class ActivityViewModel : ObservableObject
    {
        private int _id;
        private string _title;
        private string _description;
        private TimeSpan _duration;
        private string _comment;
        private DateTime _date;
        private int _employeeId;
        private EmployeeViewModel _employee;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int EmployeeId
        {
            get => _employeeId;
            set => SetProperty(ref _employeeId, value);
        }

        public EmployeeViewModel Employee
        {
            get => _employee;
            set => SetProperty(ref _employee, value);
        }
    }
}
