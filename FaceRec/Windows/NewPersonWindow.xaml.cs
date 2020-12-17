using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceRec.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewPerson.xaml
    /// </summary>
    /// 
    public partial class NewPersonWindow : Window
    {      
        private byte[] photo;

        public NewPersonWindow()
        {
            InitializeComponent();
        }

        public string FirstName
        {
            get { return FirstNameBox.Text; }
        }

        public string LastName
        {
            get { return LastNameBox.Text; }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                BitmapImage img = new BitmapImage(new Uri(op.FileName));
                imgPhoto.Source = img;

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                using MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                photo = ms.ToArray();
            }

        }

        public byte[] ImagePhoto
        {
            get { return photo; }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }     
    }
}
