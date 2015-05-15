using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dae.ToolDev.Framework;

namespace ViewModels
{
    public class MainViewModel
    {
        private UserViewModel _userViewModel;
        private CalendarViewModel _calendarViewModel;
        private NewWorkoutViewModel _newWorkoutViewModel;
        private ChartingViewModel _chartingViewModel;

        public UserViewModel UserViewModel { get { return _userViewModel; } set { _userViewModel = value; }}
        public CalendarViewModel CalendarViewModel { get { return _calendarViewModel; } set { _calendarViewModel = value; } }
        public NewWorkoutViewModel NewWorkoutViewViewModel { get { return _newWorkoutViewModel; } set { _newWorkoutViewModel = value; } }
        public ChartingViewModel ChartingViewModel { get { return _chartingViewModel; } set { _chartingViewModel = value; } }

        public MainViewModel()
        {
            _userViewModel = new UserViewModel();
            _calendarViewModel = new CalendarViewModel();
            _newWorkoutViewModel = new NewWorkoutViewModel();
            _chartingViewModel = new ChartingViewModel();

        }
    }
}
