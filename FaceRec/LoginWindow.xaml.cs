using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FaceRec.Models;

namespace FaceRec
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                if (db.Users.Local.Count() < 2)
                {
                    User ua = new User("admin", "admin", UserRole.Admin, "Admin User");
                    ua.UserData = "Admin User";
                    ua.Role = UserRole.Admin;

                    User uu = new User("user", "user", UserRole.Writer, "Writer User");
                    uu.UserData = "Writer User";
                    uu.Role = UserRole.Writer;

                    db.Users.Add(ua);
                    db.SaveChanges();
                }


                User user = db.Users.Where(u => u.Login == LoginBox.Text).FirstOrDefault();
                if (user != null && user.Password == PasswordBox.Password)
                {
                    MainWindow mw = new MainWindow(user.Login, user.Role);
                    mw.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Inncorect login or password");
                }

            }
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
    }
}
