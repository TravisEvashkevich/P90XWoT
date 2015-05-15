using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dae.ToolDev.Framework;
using Models;

namespace ViewModels
{
    public class NewWorkoutViewModel : ViewModelBase
    {
        #region Fields
        //Keeps the name of the workout from the combo box,
        //used to check against the names of the workout name list
        //to return index for putting the correct one on screen
        private string _selectedComboBoxWorkout;
        private string _selectedWorkoutName;

        //Holds the actual repnames and reps, each gets a seperate list
        //for easier parsing and comparing later
        private readonly List<List<RepsModel>> _workouts;
        private List<RepsModel> _selectedWorkout;

        //
        private IList<string> _workoutPaths;
        private ObservableCollection<string> _workoutNames;

        //Variables for the new workouts that are added (holder for names and for the workouts themselves)
        private ObservableCollection<string> _workoutNamesToBeSaved;
        private ObservableCollection<List<RepsModel>> _workoutsToBeSaved;

        //Variables for the prevoius workout
        private ObservableCollection<List<RepsModel>> _previousWorkouts;
        private List<RepsModel> _selectedPreviousWorkout;

        private UserViewModel _userViewModel;
        private string _statusMessage;


        #endregion

        #region Properties

        public UserViewModel UserViewModel
        {
            get { return _userViewModel; }
            set { Set(ref _userViewModel, value); }
        }

        public List<List<RepsModel>> Workouts
        {
            get { return _workouts; }
            private set { /*Set(ref _workouts, value);*/ }
        }

        public List<RepsModel> SelectedWorkout
        {
            get { return _selectedWorkout; }
            set { Set(ref _selectedWorkout, value); }
        }

        //List of string holding all the xml files without extension or directory info
        public ObservableCollection<string> WorkoutNames
        {
            get { return _workoutNames; }
            set {Set(ref _workoutNames,value);}
        }

        public string SelectedComboBoxWorkout
        {
            get { return _selectedComboBoxWorkout; }
            set { Set(ref _selectedComboBoxWorkout, value); }
        }

        public ObservableCollection<string> WorkoutNamesToBeSaved
        {
            get { return _workoutNamesToBeSaved; }
            set { Set(ref _workoutNamesToBeSaved, value); }
        }
        public ObservableCollection<List<RepsModel>> WorkoutsToBeSaved
        {
            get { return _workoutsToBeSaved; }
            set { Set(ref _workoutsToBeSaved, value); }
        }

        public ObservableCollection<List<RepsModel>> PreviousWorkouts
        {
            get { return _previousWorkouts; }
            set { Set(ref _previousWorkouts, value); }
        }

        public List<RepsModel> SelectedPreviousWorkout { get { return _selectedPreviousWorkout; } set { Set(ref _selectedPreviousWorkout, value); } }

        public string SelectedWorkoutName
        {
            get { return _selectedWorkoutName; }
            set { Set(ref _selectedWorkoutName, value, UpdateShownWorkout); }
        }

        public bool CanAct { get; set; }

        public string StatusMessage { get { return _statusMessage; } set { Set(ref _statusMessage, value); } }

        #endregion

        #region Commands
        public DelegateCommand CmdAddWorkout { get; private set; }
        public DelegateCommand CmdRemoveWorkout { get; private set; }
        public DelegateCommand CmdSave { get; private set; }
        public DelegateCommand CmdSaveAndClear { get; private set; }
        #endregion

        public NewWorkoutViewModel()
        {
            _workouts = new List<List<RepsModel>>();
            _selectedWorkout = new List<RepsModel>();

            _workoutPaths = new List<string>();
            _workoutNames = new ObservableCollection<string>();
            _workoutNamesToBeSaved = new ObservableCollection<string>();
            _workoutsToBeSaved = new ObservableCollection<List<RepsModel>>();
            _previousWorkouts = new ObservableCollection<List<RepsModel>>();
            _selectedPreviousWorkout = new List<RepsModel>();

            CmdAddWorkout = new DelegateCommand(AddNewWorkoutInstance);
            CmdRemoveWorkout = new DelegateCommand(RemoveWorkout);
            CmdSave = new DelegateCommand(SaveWorkouts);
            CmdSaveAndClear = new DelegateCommand(SaveAndClearWorkouts);
            WorkoutNames.Add("");
            SelectedComboBoxWorkout = WorkoutNames[0];
        }

        //Load all the xml files into xdocs so they can be parsed.
        public void LoadWorkoutBase()
        {
            //read the same directory but to get the file names of the workouts instead.
            //This makes it so when a workout is selected, we can get the index of it and then get
            //the same index from the RepsModel list and parse that to the screen.

            //Version 1.3 Upgrade, we go with the program that the user has selected when they started
            //so if they want p90x, we only show p90x and not the others.
            if(_userViewModel.LoggedInUser != null)
            {
                string program = _userViewModel.LoggedInUser.Program;

                String[] fileNamesWithoutExtention =
                    Directory.GetFiles(@"Data\Workouts\" + program, "*.xml")
                        .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                        .ToArray();
                WorkoutNames = new ObservableCollection<string>(fileNamesWithoutExtention);

                //load and parse the xmls
                _workoutPaths = Directory.GetFiles(@"Data\Workouts\" + program, "*.xml");
                //set the combobox to the first workout.
                SelectedComboBoxWorkout = WorkoutNames[0];

                foreach (string name in _workoutPaths)
                {
                    XDocument doc = XDocument.Load(name);

                    var workout = doc.Descendants("Workout")
                        .Select(el => new RepsModel((string)el.Attribute("Name"), (int)el.Attribute("Reps")));

                    _workouts.Add(new List<RepsModel>(workout));
                }
            }
        }

        public void AddNewWorkoutInstance()
        {
            UpdateCanAct();
            if (CanAct)
            {
                //Do a quick check to make sure there is no double entering of workouts as it causes crashes and weird behaviour
                if (WorkoutNamesToBeSaved.Any(name => SelectedComboBoxWorkout == name))
                {
                    return;
                }
                //go through the workoutnames list and get the position of the workout that is the 
                //same as the displayed
                int index = 0;

                index = WorkoutNames.IndexOf(SelectedComboBoxWorkout);

                //add the workout to the list.
                WorkoutsToBeSaved.Add(Workouts[index]);
                //add the name to the list 
                string newName = SelectedComboBoxWorkout;
                WorkoutNamesToBeSaved.Add(newName);

                //We also load the last instance of the workout so the user can see their last results
                //as they fill in the new workout (helps to motivate people)
                LoadLastWorkout();
                SelectedWorkoutName = SelectedComboBoxWorkout;
            }
        }

        //probably a bad hack but hey, makes the program work as intended.
        private void ZeroWorkouts()
        {
            foreach (var repsModel in Workouts.SelectMany(workout => workout))
            {
                repsModel.Reps = 0;
                repsModel.Details = "";
            }
        }

        private void ZeroWorkouts(int index)
        {
            foreach (var repsModel in Workouts[index])
            {
                repsModel.Reps = 0;
                repsModel.Details = "";
            }
        }
        private void LoadLastWorkout()
        {
            UpdateCanAct();
            if (CanAct)
            {
                //check to see if user is logged in
                if (UserViewModel.LoggedInUser != null)
                {
                    //check to see if the file exists
                    var path = string.Format(@"Data\Users\{0}\Last\{1}.xml",
                                             UserViewModel.LoggedInUser.Name, SelectedComboBoxWorkout);
                    bool exists = File.Exists(path);
                    if (exists)
                    {
                        //Load the xml of the previous workout
                        var doc = XDocument.Load(path);

                        var workout = doc.Descendants("Workout")
                                         .Select(
                                             el =>
                                             new RepsModel((string)el.Attribute("Name"), (int)el.Attribute("Reps"), (string)el.Attribute("Details")));

                        PreviousWorkouts.Add(new List<RepsModel>(workout));
                        SelectedPreviousWorkout = PreviousWorkouts.Last();
                    }
                }
            }
        }

        public void RemoveWorkout()
        {
            UpdateCanAct();
            if (CanAct)
            {
                if (WorkoutNamesToBeSaved.Count > 0)
                {
                    int index = 0;
                    index = WorkoutNamesToBeSaved.IndexOf(SelectedWorkoutName);
                    if (WorkoutNamesToBeSaved.Count <= 1)
                    {
                        SelectedWorkoutName = null;
                        SelectedWorkout = null;
                        SelectedPreviousWorkout = null;
                    }
                    else if (index == WorkoutNamesToBeSaved.Count - 1)
                    {
                        SelectedWorkoutName = WorkoutNamesToBeSaved[index - 1];
                    }
                    else
                    {
                        SelectedWorkoutName = WorkoutNamesToBeSaved[index + 1];
                    }
                    //remove the workouts from the workoutsToBeSaved as well as the Previous that way everything displays correctly
                    WorkoutNamesToBeSaved.RemoveAt(index);
                    WorkoutsToBeSaved.RemoveAt(index);
                    if (PreviousWorkouts.Count > 0)
                    {
                        PreviousWorkouts.RemoveAt(index);
                    }
                    //Mid as well zero it everytime I remove the workout.
                    ZeroWorkouts(index);
                }

                StatusMessage = "Selected Workout Removed";
            }
        }

        public void SaveWorkouts()
        {
            UpdateCanAct();
            if (CanAct)
            {
                string history = string.Format(@"Data\Users\{0}\History\",
                                                UserViewModel.LoggedInUser.Name);
                if (!Directory.Exists(history))
                    Directory.CreateDirectory(history);
                //Save the workout to the History Folder first.
                if (Directory.Exists(history))
                {
                    DateTime now = DateTime.Now;
                    //if History doesn't exist, make it.
                    //Directory.CreateDirectory(Path);
                    //then we will save the xml in History as well as update the corresponding file in Last (more or less just overwrite it)
                    int i = 0;
                    foreach (var workout in WorkoutsToBeSaved)
                    {

                        SelectedWorkout = workout;
                        var workoutName = WorkoutNamesToBeSaved[i];
                        var element = new XElement(@workoutName.Replace(" ", ""),
                            from e in workout
                            select
                                 new XElement("Workout",
                                              new XAttribute("Name", e.RepName),
                                              new XAttribute("Reps", e.Reps),
                                              new XAttribute("Details", e.Details)));

                        var doc = new XDocument(new XElement(element));
                        //Construct the full path with workout name and date but remember to remove the / from DateTime and just replace with -
                        var path = history + now.ToShortDateString().Replace("/", "-") + "-" + WorkoutNamesToBeSaved[i] + ".xml";
                        doc.Save(path);
                        ++i;
                    }
                }

                string last = string.Format(@"Data\Users\{0}\Last\",
                                                UserViewModel.LoggedInUser.Name);
                if (!Directory.Exists(last))
                    Directory.CreateDirectory(last);
                //Save the workoutout to the Last folder (just overwrite the currently made Workout if there is one there.)
                if (Directory.Exists(history))
                {
                    //if History doesn't exist, make it.
                    //Directory.CreateDirectory(Path);
                    //then we will save the xml in History as well as update the corresponding file in Last (more or less just overwrite it)
                    int i = 0;
                    foreach (var workout in WorkoutsToBeSaved)
                    {

                        SelectedWorkout = workout;
                        var workoutName = WorkoutNamesToBeSaved[i];
                        var element = new XElement(@workoutName.Replace(" ", ""),
                            from e in workout
                            select
                                 new XElement("Workout",
                                              new XAttribute("Name", e.RepName),
                                              new XAttribute("Reps", e.Reps),
                                              new XAttribute("Details", e.Details)));

                        var doc = new XDocument(new XElement(element));
                        //Construct the full path with workout name and date but remember to remove the / from DateTime and just replace with -
                        var path = last + WorkoutNamesToBeSaved[i] + ".xml";
                        doc.Save(path);
                        ++i;
                    }
                }
                StatusMessage = "All Workouts Saved!";
            }
        }

        public void SaveAndClearWorkouts()
        {
            SaveWorkouts();

            SelectedWorkout = null;
            SelectedPreviousWorkout = null;
            WorkoutNamesToBeSaved.Clear();
            WorkoutsToBeSaved.Clear();
            PreviousWorkouts.Clear();

            ZeroWorkouts();
            StatusMessage = "All Workouts Saved and Cleared for New Ones!";

        }

        private void UpdateShownWorkout()
        {

            //go through the workoutnames list and get the position of the workout that is the 
            //same as the displayed
            int index = 0;
            index = WorkoutNamesToBeSaved.IndexOf(SelectedWorkoutName);
            if (index >= 0)
            {
                SelectedWorkout = WorkoutsToBeSaved[index];

                if (PreviousWorkouts.Count > 0)
                    SelectedPreviousWorkout = PreviousWorkouts[index];
            }
            else
            {
                SelectedWorkoutName = null;
            }

        }

        private void UpdateCanAct()
        {
            if (UserViewModel.LoggedInUser != null)
                CanAct = true;
            else
            {
                CanAct = false;
            }
        }

    }
}
    

