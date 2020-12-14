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
                if (db.Users.Local.Count() == 0)
                {

                    User ua = new User("admin", "admin", UserRole.Admin, "Admin User");
                    ua.UserData = "Admin User";
                    ua.Role = UserRole.Admin;


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
