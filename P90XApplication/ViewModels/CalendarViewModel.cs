using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dae.ToolDev.Framework;
using Models;

namespace ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        //Private string to hack something together so people don't have to re enter
        //data from prior to v1.1 (since I added the details entry to the xml and it crashes
        //when trying to make things that don't have that entry in it...
        private string _versionNumber = "1.3";

        //Keeps the name of the workout from the combo box,
        //used to check against the names of the workout name list
        //to return index for putting the correct one on screen
        private string _selectedWorkoutName;
        //a private list of strings that are all the history workouts with the date stripped
        private ObservableCollection<string> _baseWorkoutNames;
        //Holds the actual repnames and reps, each gets a seperate list
        //for easier parsing and comparing later
        private List<List<RepsModel>> _workouts;
        //Holds the current workout that is selected.
        private List<RepsModel> _selectedWorkout;

        private IList<string> _workoutPaths;
        private IList<string> _workoutNames;

        private UserViewModel _userVModel;
        private User _loggedInUser;
        private List<string> _selectedWorkoutsCompareNames;
        private ObservableCollection<List<RepsModel>> _selectedWorkoutCompare;

        private ObservableCollection<RepsModel> _compareList1;
        private ObservableCollection<RepsModel> _compareList2;
        private List<string> _selectedWorkoutsCompareBaseNames;

        public DelegateCommand CmdLoad { get; private set; }
        public DelegateCommand CmdAddToCompare { get; private set; }
        public DelegateCommand CmdCompare { get; set; }
        public DelegateCommand CmdClear { get; set; }   

        public CalendarViewModel()
        {
            _loggedInUser = null;
            _workouts = new List<List<RepsModel>>();
            _selectedWorkout = new List<RepsModel>();

            _workoutPaths = new List<string>();
            _workoutNames = new List<string>();
            _selectedWorkoutsCompareNames = new List<string>(2);
            _selectedWorkoutCompare = new ObservableCollection<List<RepsModel>>();
            _baseWorkoutNames = new ObservableCollection<string>();
            _selectedWorkoutsCompareBaseNames = new List<string>();

            CmdLoad = new DelegateCommand(LoadPreviousWorkouts);
            CmdAddToCompare = new DelegateCommand(AddToCompare);
            CmdCompare = new DelegateCommand(CompareWorkouts,CanCompare);
            CmdClear = new DelegateCommand(ClearCompare);
        }

        public int CompareCount { get; set; }

        public ObservableCollection<RepsModel> CompareList1
        {
            get { return _compareList1; }
            set { Set(ref _compareList1, value); }
        }
        public ObservableCollection<RepsModel> CompareList2
        {
            get { return _compareList2; }
            set { Set(ref _compareList2, value); }
        }

        private ObservableCollection<string> BaseWorkoutNames
        {
            get { return _baseWorkoutNames; }
            set { Set(ref _baseWorkoutNames, value); }
        }

        public List<List<RepsModel>> Workouts
        {
            get { return _workouts; }
            set { Set(ref _workouts, value); }
        }

        public List<RepsModel> SelectedWorkout
        {
            get { return _selectedWorkout; }
            set { Set(ref _selectedWorkout, value); }
        }

        public UserViewModel UserViewModel
        {
            get { return _userVModel; }
            set { Set(ref _userVModel, value); }
        }

        public User LoggedInUser
        {
            get { return _userVModel.LoggedInUser; }
            set { Set(ref _loggedInUser, value); }
        }

        //List of string holding all the xml files without extension or directory info
        public IList<string> WorkoutNames
        {
            get { return _workoutNames; }
            set { Set(ref _workoutNames, value); }
        }

        public string SelectedWorkoutName
        {
            get { return _selectedWorkoutName; }
            set { Set(ref _selectedWorkoutName, value, UpdateShownWorkout); }
        }

        private List<string> SelectedWorkoutsCompareBaseNames
        {
            get { return _selectedWorkoutsCompareBaseNames; }
            set {Set(ref _selectedWorkoutsCompareBaseNames,value);}
        }

        public List<string> SelectedWorkoutsCompareNames
        {
            get { return _selectedWorkoutsCompareNames; }
            set { Set(ref _selectedWorkoutsCompareNames, value); }
        }

        public ObservableCollection<List<RepsModel>> SelectedWorkoutCompare
        {
            get { return _selectedWorkoutCompare; }
            set { Set(ref _selectedWorkoutCompare, value); }
        }

        public void LoadPreviousWorkouts()
        {
            //here we take the path of the user + the internal folder structure to get to the previous workouts. 
            //then, like the prepare calendar, we will read all the xml's in that folder and put them in the listbox

            //read the directory but to get the file names of the workouts instead.
            //This makes it so when a workout is selected, we can get the index of it and then get
            //the same index from the RepsModel list and parse that to the screen.
            if (LoggedInUser != null)
            {
                //have to clear the dummy data from the start up.
                Workouts.Clear();
                var path = string.Format(@"Data\Users\{0}\History\", LoggedInUser.Name);
                if (Directory.Exists(path) && (Directory.GetFiles(path).Length >= 0))
                {
                    String[] fileNamesWithoutExtention =
                        Directory.GetFiles(path, "*.xml")
                                 .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                                 .ToArray();

                    WorkoutNames = fileNamesWithoutExtention;

                    //load and parse the xmls
                    _workoutPaths = Directory.GetFiles(path, "*.xml");
                    
                   for (int i = 0; i < _workoutPaths.Count; i++)
                    {
                        XDocument doc = XDocument.Load(_workoutPaths[i]);

                        var workout = doc.Descendants("Workout")
                                         .Select(
                                             el =>
                                             new RepsModel((string)el.Attribute("Name"),
                                                           (int)el.Attribute("Reps"),(string)el.Attribute("Details")));

                        Workouts.Add(new List<RepsModel>(workout));
                    }

                    //SelectedWorkoutName = WorkoutNames[0];
                    BaseWorkoutNames.Clear();
                    foreach (var name in WorkoutNames)
                    {
                        //get the index of the last hyphon so we can substring from start to there
                        int index = 0;
                        index = name.LastIndexOf('-');
                       var temp = name.Substring(index+1, name.Length - (index+1));
                        BaseWorkoutNames.Add(temp);
                    }
                    
                    UpdateShownWorkout();
                }
            }
        }

        private void UpdateShownWorkout()
        {

            //go through the workoutnames list and get the position of the workout that is the 
            //same as the displayed
            int index = 0;
           // index = WorkoutNames.IndexOf(SelectedWorkoutName);
            for (int i = 0; i < WorkoutNames.Count; i++)
            {
                if (SelectedWorkoutName == WorkoutNames[i])
                {
                    index = i;
                    break;
                }
            }
            if (Workouts.Count == 0)
                SelectedWorkout = null;
            else
            {
                SelectedWorkout = Workouts[index];
            }
        }

        private void AddToCompare()
        {
            //Go through the workoutnames and workouts to retrieve the 
            //selected workout information and add to the appropriate lists
            //Also have to check and make sure that the workouts are the same type
            //that way you're not comparing apples to oranges;
            int index = 0;
            index = WorkoutNames.IndexOf(SelectedWorkoutName);

            if (CompareCount < 2)
            {
                if (CompareList1 == null && SelectedWorkoutName != null)
                {
                    //don't have to do workout type check on first list as it's the first one.
                        var oCollection = new ObservableCollection<RepsModel>(Workouts[index]);
                        CompareList1 = oCollection;
                        ++CompareCount;
                        SelectedWorkoutsCompareNames.Add(SelectedWorkoutName);
                    //Add the BASE name to this list so we can check if the workout type matches
                    //AND keep the original name for chart display
                    SelectedWorkoutsCompareBaseNames.Add(BaseWorkoutNames[index]);
                    
                }
                else if(CompareList1 != null && SelectedWorkoutName!=null)
                {
                    if (SelectedWorkoutsCompareBaseNames.Contains(BaseWorkoutNames[index]))
                    {
                        var oCollection = new ObservableCollection<RepsModel>(Workouts[index]);
                        CompareList2 = oCollection;
                        ++CompareCount;
                        SelectedWorkoutsCompareNames.Add(SelectedWorkoutName);
                    }
                }
                
                //SelectedWorkoutCompare.Add(Workouts[index]);
            }

        }

        private void CompareWorkouts()
        {
            //check to see if both compare's have something in them else just load the first one if there is something in it.
            if (SelectedWorkoutCompare[1] != null)
            {
                SelectedWorkoutCompare[0].ToList().ForEach(CompareList1.Add);
                SelectedWorkoutCompare[1].ToList().ForEach(CompareList2.Add);
            }
            else if (SelectedWorkoutCompare[0] != null)
            {
                SelectedWorkoutCompare[0].ToList().ForEach(CompareList1.Add);
            }
        }

        private void ClearCompare()
        {
            CompareList1 = null;
            CompareList2 = null;
            SelectedWorkoutsCompareNames.Clear();
            SelectedWorkoutsCompareBaseNames.Clear();
            CompareCount = 0;

        }
        
        public bool CanCompare()
        {
            return SelectedWorkoutCompare.Count >= 1;
        }
        private void UpdateCanCompare()
        {
            CmdCompare.Update();
        }

    }
}
