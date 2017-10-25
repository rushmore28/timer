using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        private CancellationTokenSource _cancellationTokenSource;
        private IList<MaintenanceWindow> _maintenanceWindows;

        public static void Main(string[] args)
        {
            var program = new Program();
            program.SetupMaintenanceWindow();
            program.RunCodeAt(program.GetNextDate());
            Console.ReadLine();
        }

        private void SetupMaintenanceWindow()
        {
            _maintenanceWindows = new List<MaintenanceWindow>
            {
                new MaintenanceWindow
                {
                    Day = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8,25,0),
                    EndTime = new TimeSpan(8,26,0)
                },
                new MaintenanceWindow
                {
                    Day = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8,22,0),
                    EndTime = new TimeSpan(8,24,0)
                }
            };
        }

        private void RunCodeAt(DateTime date)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var dateNow = DateTime.Now;
            TimeSpan ts;
            if (date > dateNow)
                ts = date - dateNow;
            else
            {
                date = GetNextDate();
                ts = date - dateNow;
            }

            //waits certan time and run the code, in meantime you can cancel the task at anty time
            Task.Delay(ts).ContinueWith((x) =>
            {
                Console.WriteLine($"Code run at {DateTime.Now}");
                Thread.Sleep(120000);
                RunCodeAt(GetNextDate());
            }, _cancellationTokenSource.Token);
        }

        private DateTime GetNextDate()
        {
            var date = DateTime.Now;

            var nextStartDateTimes = new List<DateTime>();
            var nextEndDateTimes = new List<DateTime>();

            foreach (var window in _maintenanceWindows.OrderBy(x => x.Day).ThenBy(x => x.StartTime))
            {
                nextStartDateTimes.Add(GetStartDateTime(window, date));
                nextEndDateTimes.Add(GetEndDateTime(window, date));
            }

            var nextStartDateTime = nextStartDateTimes.OrderBy(x => x).First();
            var nextEndDateTime = nextEndDateTimes.OrderBy(x => x).First();

            return nextStartDateTime < nextEndDateTime ? nextStartDateTime : nextEndDateTime;
        }

        public DateTime GetStartDateTime(MaintenanceWindow window, DateTime date)
        {
            var startTime = date.GetNextWeekday(window.Day);

            if (window.StartTime < date.TimeOfDay)
            {
                startTime = date.AddDays(1).GetNextWeekday(window.Day);
            }

            startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day);
            startTime = startTime.Add(window.StartTime);
            return startTime;
        }

        public DateTime GetEndDateTime(MaintenanceWindow window, DateTime date)
        {
            var endTime = date.GetNextWeekday(window.Day);

            if (window.EndTime < date.TimeOfDay)
            {
                endTime = date.AddDays(1).GetNextWeekday(window.Day);
            }

            endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day);
            endTime = endTime.Add(window.EndTime);
            return endTime;
        }


        public class MaintenanceWindow
        {
            public DayOfWeek Day { get; set; }   
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
        }
    }
}
