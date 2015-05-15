using System.Windows.Controls;
using ViewModels;

namespace Views
{
    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class ChartView : UserControl
    {
        private ChartingViewModel _chartingViewModel = new ChartingViewModel();
        

        public ChartingViewModel ChartingViewModel
        {
            get { return _chartingViewModel; }
            set { _chartingViewModel = value; }
        }

        public ChartView()
        {
            InitializeComponent();

            DataContext = ChartingViewModel;
            
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ChartingViewModel.UpdateCharts();
            if (ChartingViewModel.DataSourceList.Count == 2)
            {
                ColumnChart1.DataContext = ChartingViewModel.DataSourceList[0];
                ColumnChart1.Title = ChartingViewModel.WorkoutNames[0];

                ColumnChart2.DataContext = ChartingViewModel.DataSourceList[1];
                ColumnChart2.Title = ChartingViewModel.WorkoutNames[1];
            }

        }

      


    }
}
