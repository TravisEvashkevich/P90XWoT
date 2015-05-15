using Models;
using ViewModels;
using UserControl = System.Windows.Controls.UserControl;


namespace Views
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
       private UserViewModel _userViewModel = new UserViewModel();

        public UserViewModel UserViewModel
        {
            get { return _userViewModel; }
            set {_userViewModel = value; }
        }

        public UserView()
        {
            InitializeComponent();
            DataContext = UserViewModel;

            UserComboBox.ItemsSource = UserViewModel.Users;
            ProgramComboBox.ItemsSource = UserViewModel.Programs;
        }

        private void Logon_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordTBox.Text = "";
        }

        private void AddUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordInput.Text = "";
            UserNameInput.Text = "";
        }
    }
}
