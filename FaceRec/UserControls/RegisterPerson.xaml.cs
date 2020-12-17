using FaceRec.Models;
using FaceRec.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace FaceRec.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Register_Person.xaml
    /// </summary>
    public partial class RegisterPerson : UserControl
    {
        public List<Person> Persons;

        public RegisterPerson()
        {
            InitializeComponent();

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Persons.Load();
                Persons = db.Persons.Local.ToList();
                PersonList.ItemsSource = Persons;
            }
        }

        private void btnRemovePerson_Click(object sender, RoutedEventArgs e)
        {

            using (ApplicationContext db = new ApplicationContext())
            {
                var u = (Person)PersonList.SelectedItem;

                if (u != null)
                {
                    db.Persons.Remove(u);
                    db.Persons.Load();
                    Persons = db.Persons.Local.ToList();
                    PersonList.ItemsSource = Persons;
                    db.SaveChanges();

                    MessageBox.Show("Пользователь удален");
                }
                else
                {
                    MessageBox.Show("Добавьте пользователя");
                }

            }

        }


        private void PersonList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var p = (Person)PersonList.SelectedItem;
                if (p != null)
                {

                    Person person = db.Persons.Find(p.Id);
                    PersonDetails.DataContext = person;
                    PhotoImage.Source = LoadImage(person.PersonPhoto);
                }
            }
        }

        private void addPerson_Click(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                NewPersonWindow registerPersonWindow = new NewPersonWindow();
                registerPersonWindow.DataContext = this;
                if (registerPersonWindow.ShowDialog() == true)
                {

                    string firstName = registerPersonWindow.FirstName;
                    string secondName = registerPersonWindow.LastName;
                    byte[] imagePhoto = registerPersonWindow.ImagePhoto;

                    db.Persons.Add(new Person(firstName, secondName, imagePhoto));
                    db.SaveChanges();
                    db.Persons.Load();
                    Persons = db.Persons.Local.ToList();
                    PersonList.ItemsSource = Persons;

                }
            }

        }

        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}
