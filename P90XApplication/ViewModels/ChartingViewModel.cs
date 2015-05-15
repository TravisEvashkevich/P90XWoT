using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dae.ToolDev.Framework;
using Models;

namespace ViewModels
{
    public class ChartingViewModel:ViewModelBase
    {
        private CalendarViewModel _calendarViewModel;

        private ObservableCollection<RepsModel> _list1;
        private ObservableCollection<RepsModel> _list2;
        private ObservableCollection<string> _workoutNames;
        private ObservableCollection<List<KeyValuePair<string, int>>> _dataSourceList;

        public ObservableCollection<RepsModel> List1 { get { return _list1; } set { Set(ref _list1,value); } }
        public ObservableCollection<RepsModel> List2 { get { return _list2; } set { Set(ref _list2,value); } }

        //the holders for the list of KVP's which is what we transform the collections of RepsModels into when we hit the Update button
        public ObservableCollection<List<KeyValuePair<string, int>>> DataSourceList
        {
            get { return _dataSourceList; }
            set { Set(ref _dataSourceList, value); }
        }

        public ObservableCollection<string> WorkoutNames{get { return _workoutNames; } set{Set(ref _workoutNames,value);}} 

        public CalendarViewModel CalendarViewModel { get { return _calendarViewModel; } set{Set(ref _calendarViewModel,value);}}
        public DelegateCommand CmdUpdate { get; private set; }

        public ChartingViewModel()
        {
            _list1 = new ObservableCollection<RepsModel>();
            _list2 = new ObservableCollection<RepsModel>();
            _workoutNames = new ObservableCollection<string>();
            CmdUpdate = new DelegateCommand(UpdateCharts);
            _dataSourceList = new ObservableCollection<List<KeyValuePair<string, int>>>();
        }

        public void UpdateCharts()
        {
            if (CalendarViewModel.CompareList1 != null && CalendarViewModel.CompareList2 != null)
            {
                WorkoutNames.Clear();
                CalendarViewModel.SelectedWorkoutsCompareNames.ToList().ForEach(WorkoutNames.Add);
                List1 = CalendarViewModel.CompareList1;
                List2 = CalendarViewModel.CompareList2;
                UpdateChartingWidths();
            }
        }

        private void UpdateChartingWidths()
        {
            DataSourceList.Clear();
            var tempList = new List < KeyValuePair<string, int>>();
            foreach (var repsModel in List1)
            {
               var kvp = new KeyValuePair<string, int>(repsModel.RepName, repsModel.Reps);
                tempList.Add(kvp);
            }
            DataSourceList.Add(tempList);
            var tempList2 = new List<KeyValuePair<string, int>>();
            foreach (var rModel in List2)
            {
                var kvp = new KeyValuePair<string, int>(rModel.RepName, rModel.Reps);
                tempList2.Add(kvp);
            }
            DataSourceList.Add(tempList2);
            // SelectedWorkoutCompare[0].ToList().ForEach(CompareList1.Add);
        }

    }
}
