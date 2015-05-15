using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models;
using Dae.ToolDev.Framework;


namespace ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        ///A viewmodel that will load an xml file at a hardcoded spot on the disk (relative to the project)
        /// has "password" protection, very very low level as it keeps it with the user names
        /// and when a user is removed then the folder attached to it is also removed.
        /// <summary>
        /// Privates
        /// </summary>
        private User _selectedUser;
        private ObservableCollection<User> _users;
        private string _xmlPath = @"Data\Users\UserList.xml";
        private string _loggedInStatus = "Not Logged In";
        private bool _canAddNewUser;
        private bool _canRemoveUser;
        private bool _notLoggedIn = true;
        private User _loggedInUser;
        private List<string> _programs;

        private NewWorkoutViewModel _newWorkoutViewModel;

        ///
        /// Properties
        /// 

        public NewWorkoutViewModel NewWorkoutViewModel
        {
            get { return _newWorkoutViewModel; }
            set {Set(ref _newWorkoutViewModel, value);}
        }

        public ObservableCollection<User> Users
        {
            get { return _users; }

        }

        public bool CanAddNewUser
        {
            get
            {
                return _canAddNewUser;
            }
            private set { Set(ref _canAddNewUser, value); }
        }

        public bool NotLoggedIn
        {
            get { return _notLoggedIn; }
            set { Set(ref _notLoggedIn, value); }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                Set(ref _selectedUser, value, OnSelectedUserChanged);
            }
        }

        public List<string> Programs { get { return _programs; }
            private set { }
        }

        public string SelectedProgram { get; set; }

        public User LoggedInUser { get { return _loggedInUser; } set { Set(ref _loggedInUser, value); } }

        public string NameOfNewUser { get; set; }

        public string Password { get; set; }
        public string EnteredPassword { get; set; }

        public string LoggedInStatus
        {
            get { return _loggedInStatus; }
            set { Set(ref _loggedInStatus, value); }
        }

        public bool CanRemoveUser
        {
            get { return _canRemoveUser; }
            set { Set(ref _canRemoveUser, value); }
        }

        /// <summary>
        /// Publics
        /// </summary>
        public DelegateCommand CmdNewUser { get; private set; }
        public DelegateCommand CmdRemoveUser { get; private set; }
        public DelegateCommand CmdSaveUsers { get; private set; }
        public DelegateCommand CmdLogin { get; private set; }
        public DelegateCommand CmdLogoff { get; private set; }

        public UserViewModel()
        {
            CmdNewUser = new DelegateCommand(AddNewUser);
            CmdRemoveUser = new DelegateCommand(RemoveUser, CanRemoveSelectedUser);
            CmdSaveUsers = new DelegateCommand(SaveNewUsers);
            CmdLogin = new DelegateCommand(LoginUser);
            CmdLogoff = new DelegateCommand(LogoffUsers);

            VersionCheck();
            LoadUsers();
            _users.CollectionChanged += HandleUserCollectionChange;

            _programs = new List<string>();
            _programs.Add("P90X");
            _programs.Add("P90X2");
            _programs.Add("P90X3");
        }

        private void VersionCheck()
        {
            if(File.Exists(_xmlPath))
            {
                //Read the users xml file in and store in the collection
                XDocument doc = XDocument.Load(_xmlPath);

                var users =
                    doc.Descendants("User")
                        .Select(el => new User((string)el.Attribute("Name"), (string)el.Attribute("Password"), (string)el.Attribute("Program")));
                var tempList = new ObservableCollection<User>();
                foreach (var user in users)
                {
                    if (user.Program == null)
                    {
                        //V 1.3 needed to check for program addition
                        user.Program = "P90X";
                        tempList.Add(user);
                    }
                }
                
                if(tempList.Count != 0)
                {
                    XDocument saveDoc = new XDocument();
                    saveDoc.Add(new XElement("Users"));
                    foreach (var user in tempList)
                    {
                        var element = new XElement("User", new XAttribute("Name", user.Name),
                            new XAttribute("Password", user.Password),
                            new XAttribute("Program", user.Program)); 
                        saveDoc.Element("Users").Add(element);
                    }

                    saveDoc.Save(_xmlPath);
                }

            }
        }


        /// ------------------------------------
        /// Methods
        /// ------------------------------------'

        private void LoadUsers()
        {
            if (!File.Exists(_xmlPath))
            {
                //To avoid trouble later, make a dummy xml file. and then "Load" from it
                var dummyDoc = new XDocument(new XElement("Users"));
                dummyDoc.Save(_xmlPath);

                XDocument doc = XDocument.Load(_xmlPath);
                var users =
                    doc.Descendants("User")
                       .Select(el => new User((string)el.Attribute("Name"), (string)el.Attribute("Password"), (string)el.Attribute("Program")));

                _users = new ObservableCollection<User>(users);
            }
            else
            {
                //Read the users xml file in and store in the collection
                XDocument doc = XDocument.Load(_xmlPath);

                var users =
                    doc.Descendants("User")
                       .Select(el => new User((string)el.Attribute("Name"), (string)el.Attribute("Password"), (string)el.Attribute("Program")));


                _users = new ObservableCollection<User>(users);
            }
        }

        public void AddNewUser()
        {
            //check to see if the user.xml file exists already, else make it then add the new user to it.
            //this should also clear up people having to remake profiles when installing a new version...
            if (!File.Exists(_xmlPath))
            {
                //To avoid trouble later, make a dummy xml file. and then "Load" from it
                var dummyDoc = new XDocument(new XElement("Users"));
                dummyDoc.Save(_xmlPath);

                XDocument doc = XDocument.Load(_xmlPath);
                var users =
                    doc.Descendants("User")
                       .Select(el => new User((string)el.Attribute("Name"), (string)el.Attribute("Password"), (string)el.Attribute("Program")));

                _users = new ObservableCollection<User>(users);
            }
            //Put new user in the list after the current if there is none or at the end
            //as long as there is a name AND password else do nothing.
            if (NameOfNewUser != null && Password != null)
            {
                //Check to make sure there is no double entering of names as that causes problems
                foreach (var user1 in Users)
                {

                    if(NameOfNewUser.ToLower() == user1.Name.ToLower())
                    {return;}
                }
                var user = new User(NameOfNewUser, Password, SelectedProgram);
                int index = SelectedUser == null ? Users.Count : Users.IndexOf(SelectedUser) + 1;
                Users.Insert(index, user);
                SelectedUser = Users[index];
                SaveNewUsers();

                //Add the new user to the userXML
                var doc = XDocument.Load(_xmlPath);

                var newElement = new XElement("User", new XAttribute("Name", user.Name),
                                                      new XAttribute("Password", user.Password),
                                                      new XAttribute("Program", user.Program));
                doc.Element("Users").Add(newElement);

                doc.Save(_xmlPath);
                //make the folder structures for the new user.
                //this will hold all of the workouts done
                string dummyPath = string.Format(@"Data\Users\{0}\History\",NameOfNewUser);
                Directory.CreateDirectory(dummyPath);

                //this will hold ONLY the very last of each workout that way it can be produced 
                //for comparison when making a new workout later
                dummyPath = string.Format(@"Data\Users\{0}\Last\", NameOfNewUser);
                Directory.CreateDirectory(dummyPath);

                //We will make another xml that contains data about the user such as the program they are planning to follow
                //the weight, height, food plan, days completed = 0 and will be updated when doing the Calendar load previous
                //so it stays accurate each time. (this will be day amount in the history not overall days since start)
                //and such like that and we will be able to integrate it later for meal planning as well.

            }
            //reset the name and pass to null for next user check.
            NameOfNewUser = null;
            Password = null;
        }

        public void SaveNewUsers()
        {
            foreach (var user in Users)
            {
                user.Save();
            }
        }

        public void RemoveUser()
        {
            if (CanRemoveSelectedUser())
            {
                //check to make sure there is a user selected and that it isn't our filler user data.
                if (SelectedUser != null && SelectedUser.Name != " " && SelectedUser.Password!= " ")
                {
                    int index = Users.IndexOf(SelectedUser);

                    //remove them from the XML document containing all the users 
                    var doc = XDocument.Load(_xmlPath);
                    //check the elements of the document and find an attribute with Name that equals the selected user and then remove that whole attribute.
                    doc.Root.Elements()
                       .Where(e => e.Attribute("Name").Value.Equals(SelectedUser.Name))
                       .Select(e => e)
                       .Single()
                       .Remove();
                    doc.Save(_xmlPath);

                    //Delete the folder structure as well.
                        Directory.Delete(string.Format(@"Data\Users\{0}", SelectedUser.Name), true);

                    // Next remove the current User.
                    Users.RemoveAt(index);
                }
            }
        }

        public bool ComparePasswords()
        {
            if (SelectedUser == null)
                return false;
            if (EnteredPassword != null && SelectedUser.Password != null)
            {
                if (EnteredPassword == SelectedUser.Password)
                    return true;
                else
                    return false;
            }
            else return false;
        }

        public void LoginUser()
        {
            if (ComparePasswords())
            {
                NotLoggedIn = false;
                LoggedInStatus = string.Format("Currently Logged in as {0}", SelectedUser.Name);
                LoggedInUser = SelectedUser;
                NewWorkoutViewModel.LoadWorkoutBase();
            }

        }

        public void LogoffUsers()
        {
            if (NotLoggedIn)
            {
                //do nothing
            }
            else
            {
                NotLoggedIn = true;
                LoggedInStatus = "Not Logged In.";
                LoggedInUser = null;
            }
            
        }

        private void HandleUserCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateCanAddNewUser();
        }

        private void UpdateCanAddNewUser()
        {
            // we won't let them add a new user without both a name and a password
            CanAddNewUser = NameOfNewUser != null && Password != null;
        }

        private void OnSelectedUserChanged()
        {
            UpdateCanRemoveSelectedUser();
        }

        public bool CanRemoveSelectedUser()
        {
            //we always have our "filler" user data so we make sure to not delete that so there should always
            return Users != null && Users.Count >= 1 && LoggedInUser == null;
        }

        private void UpdateCanRemoveSelectedUser()
        {
            CmdRemoveUser.Update();
        }
    }
}
