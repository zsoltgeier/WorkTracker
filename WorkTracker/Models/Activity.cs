using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
