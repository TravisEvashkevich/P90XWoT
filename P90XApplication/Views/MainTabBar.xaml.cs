using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ViewModels;

namespace Views
{
    /// <summary>
    /// Interaction logic for MainTabBar.xaml
    /// </summary>
    public partial class MainTabBar : UserControl
    {
        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
            set { _mainViewModel = value; }
        }

        public MainTabBar()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();

            DataContext = _mainViewModel;

            //We take the Views that are in the tab bar and that each have their own VM
            //and set them to the ones in the viewmodel that way they can communicate and pass
            //values on to eachother.
            MainViewModel.UserViewModel = UserView.UserViewModel;
            MainViewModel.CalendarViewModel = CalendarView.CalendarViewModel;
            MainViewModel.NewWorkoutViewViewModel = WorkoutAddView.NewWorkoutViewModel;
            MainViewModel.ChartingViewModel = ChartView.ChartingViewModel;

            //Set ViewModels that need outside data here. Such as setting the needed UserViewModel in CalendarViewModel
            MainViewModel.CalendarViewModel.UserViewModel = MainViewModel.UserViewModel;
            MainViewModel.UserViewModel.NewWorkoutViewModel = MainViewModel.NewWorkoutViewViewModel;
            //and the UserViewModel in NewWorkoutVM
            MainViewModel.NewWorkoutViewViewModel.UserViewModel = MainViewModel.UserViewModel;
            MainViewModel.ChartingViewModel.CalendarViewModel = MainViewModel.CalendarViewModel;

        }


    }
}
