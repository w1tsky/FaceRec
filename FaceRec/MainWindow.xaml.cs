using FaceRec.Models;
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
using FaceRec.UserControls;

namespace FaceRec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string login, UserRole role)
        {
            InitializeComponent();

            CurrentUser.Text = login;
            CurrentUserRole.Text = role.ToString();
 

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

        private void btnRegisterPerson_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new RegisterPerson());
        }
    }
}
