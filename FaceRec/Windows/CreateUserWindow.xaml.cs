using FaceRec.Models;
using System.Windows;


namespace FaceRec.Windows
{

    public partial class CreateUserWindow : Window
    {
        public UserRole Roles;

        public CreateUserWindow()
        {
            InitializeComponent();
        }

        public string Login
        {
            get { return LoginBox.Text; }
        }

        public string Password
        {
            get { return PasswordBox.Text; }
        }


        public string UserData
        {

            get { return UserDataBox.Text; }
        }

        public UserRole Role
        {
            get
            {
                var u = (UserRole)usersRoleList.SelectedItem;
                return u;
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    } 
}
