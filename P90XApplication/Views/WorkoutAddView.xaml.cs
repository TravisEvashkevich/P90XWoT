using System;
using System.Collections.Generic;
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
    /// Interaction logic for WorkoutAddView.xaml
    /// </summary>
    public partial class WorkoutAddView : UserControl
    {
        private NewWorkoutViewModel _newWorkoutViewModel = new NewWorkoutViewModel();

        public NewWorkoutViewModel NewWorkoutViewModel
        {
            get { return _newWorkoutViewModel; }
            set { _newWorkoutViewModel = value; }
        }

        public WorkoutAddView()
        {
            InitializeComponent();
            DataContext = NewWorkoutViewModel;

            WorkoutComboBox.ItemsSource = NewWorkoutViewModel.WorkoutNames;
        }

    }
}
