using FaceRec.Models;
using System;
using System.Windows;
using System.Windows.Input;
using FaceRec.UserControls;

namespace FaceRec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserRole userRole;
        public string Login;

        public MainWindow(string login, UserRole role)
        {
            InitializeComponent();

            CurrentUser.Text = login;
            CurrentUserRole.Text = role.ToString();
            userRole = role;
            Login = login;

        }

        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void FindPerson_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new FindPerson());
        }

        private void btnFindPersonOnImage_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new FindPersonOnImage());
        }

        private void btnRegisterPerson_Click(object sender, RoutedEventArgs e)
        {
            if (userRole == UserRole.Admin)
            {
                mainFrame.Navigate(new RegisterPerson());
            }
            else if (userRole == UserRole.Writer)
                mainFrame.Navigate(new RegisterPerson());
            else
            {
                MessageBox.Show("Вы не можете управлять личностями");
            }
           
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            if (userRole == UserRole.Admin)
            {
                mainFrame.Navigate(new ManageUsers());
            }
            else
            {
                MessageBox.Show("Вы не администратор");
            }
        }

        private void LogoutUsers_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Close();
        }

       

        
    }
}
