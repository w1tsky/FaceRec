using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using FaceRec.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    /// Логика взаимодействия для FindPersonOnImage.xaml
    /// </summary>
    public partial class FindPersonOnImage : UserControl
    {


        public List<Person> Persons;
        public List<Mat> TrainedFaces = new List<Mat>();
        public List<int> FaceIDs = new List<int>();
        public List<string> PersonsNames = new List<string>();


        private Image<Bgr, Byte> Image = null;
        Image<Gray, Byte> faceResult = null;
    
        Mat grayImage = new Mat();


        EigenFaceRecognizer recognizer;


        CascadeClassifier faceCascadeClassifier = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\Haarcascades\haarcascade_frontalface_alt.xml");

        public FindPersonOnImage()
        {
            InitializeComponent();
            TrainedFaces.Clear();
            FaceIDs.Clear();
            PersonsNames.Clear();

            using (ApplicationContext db = new ApplicationContext())
            {
                InitializeComponent();
                db.Persons.Load();
                Persons = db.Persons.Local.ToList();
            }


            int FaceCount = 0;
            foreach (var person in Persons)
            {

                var img = new Bitmap(BitmapHelpers.byteArrayToImage(person.PersonPhoto));
                Image<Gray, byte> imageForTrain = img.ToImage<Gray, byte>();

                System.Drawing.Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(imageForTrain, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);

                foreach (var face in faces)
                {
                    faceResult = imageForTrain;
                    faceResult.ROI = face;

                    CvInvoke.Resize(faceResult, faceResult, new System.Drawing.Size(200, 200), 0, 0, Inter.Cubic);
                    CvInvoke.EqualizeHist(faceResult, faceResult);
                    TrainedFaces.Add(faceResult.Mat);

                }

                FaceIDs.Add(FaceCount);
                PersonsNames.Add(person.FirstName + " " + person.LastName);
                FaceCount++;
                Debug.WriteLine(FaceCount + ". " + person.FirstName + " " + person.LastName);

                double Threshold = 2000;

                if (TrainedFaces.Count > 0)
                {
                    recognizer = new EigenFaceRecognizer(FaceCount, Threshold);
                    recognizer.Train(TrainedFaces.ToArray(), FaceIDs.ToArray());
                }

                InitializeComponent();
            }
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Image = new Image<Bgr, byte>(op.FileName);
                picCapture.Source = BitmapHelpers.ToBitmapSource(Image);
            }
        }

        private void btnDetectFaces_Click(object sender, RoutedEventArgs e)
        {
            //Convert from Bgr to Gray Image
            if (picCapture.Source != null)
            {
                CvInvoke.CvtColor(Image, grayImage, ColorConversion.Bgr2Gray);
                //Enchance the image to get better result
                CvInvoke.EqualizeHist(grayImage, grayImage);

                System.Drawing.Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);

                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(Image, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 5);
                        Image<Bgr, Byte> resultImage = Image.Convert<Bgr, Byte>();
                        picCapture.Source = BitmapHelpers.ToBitmapSource(resultImage);
                    }
                }
            }
            else
            {
                MessageBox.Show("Choose photo");
            }


        }

        private void btnRecognizeFaces_Click(object sender, RoutedEventArgs e)
        {

            if (picCapture.Source != null)
            {

                CvInvoke.CvtColor(Image, grayImage, ColorConversion.Bgr2Gray);
                //Enchance the image to get better result
                CvInvoke.EqualizeHist(grayImage, grayImage);

                System.Drawing.Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);

                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(Image, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 4);

                        Image<Bgr, Byte> resultImage = Image.Convert<Bgr, Byte>();
                        resultImage.ROI = face;


                        Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                        CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                        var result = recognizer.Predict(grayFaceResult);

                        //Debug.WriteLine(result.Label + ". " + result.Distance);
                        //Here results found known faces
                        if (result.Label != -1 && result.Distance < 2000)
                        {

                            CvInvoke.PutText(Image, PersonsNames[result.Label], new System.Drawing.Point(face.X - 2, face.Y - 2),
                            FontFace.HersheyComplex, 2.0, new Bgr(System.Drawing.Color.Green).MCvScalar);
                            CvInvoke.Rectangle(Image, face, new Bgr(System.Drawing.Color.Blue).MCvScalar, 5);

                        }
                        //here results did not found any know faces
                        else
                        {
                            CvInvoke.PutText(Image, "Unknown", new System.Drawing.Point(face.X - 2, face.Y - 2),
                            FontFace.HersheyComplex, 4.0, new Bgr(System.Drawing.Color.Orange).MCvScalar);
                            CvInvoke.Rectangle(Image, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);

                        }

                        resultImage = Image.Convert<Bgr, Byte>();
                        picCapture.Source = BitmapHelpers.ToBitmapSource(resultImage);


                    }
                }
                else
                {
                    MessageBox.Show("Choose photo");
                }


            }


        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sp = new SaveFileDialog();

            sp.Title = "Save a picture";
            sp.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (sp.ShowDialog() == true)
            {



                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)picCapture.Source));
                using FileStream stream = new FileStream(sp.FileName, FileMode.Create);
                encoder.Save(stream);

            }
        }


    }
}

