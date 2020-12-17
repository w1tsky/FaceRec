using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using FaceRec.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Emgu.CV.Util;
using System.Diagnostics;
using System.IO;


namespace FaceRec
{

    public partial class FindPerson : UserControl
    {

        public List<Person> Persons;
        public List<Person> RecognizedPersons = new List<Person>();
        public List<Mat> TrainedFaces = new List<Mat>();
        public List<int> FaceIDs = new List<int>();
        public List<string> PersonsNames = new List<string>();
        

        private VideoCapture videoCapture = null;
        private Image<Bgr, Byte> currentFrame = null;
        Image<Gray, Byte> faceResult = null;

        private bool faceDetectionEnabled = false;
        private bool faceRecognationEnabled = false;

        Mat frame = new Mat();
        EigenFaceRecognizer recognizer;

       
        CascadeClassifier faceCascadeClassifier = new CascadeClassifier(Directory.GetCurrentDirectory() + @"\Haarcascades\haarcascade_frontalface_alt.xml");

        public FindPerson()
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

            }
        
            double Threshold = 2000;

            if (TrainedFaces.Count > 0)
            {
                recognizer = new EigenFaceRecognizer(FaceCount, Threshold);
                recognizer.Train(TrainedFaces.ToArray(), FaceIDs.ToArray());
            }

        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            videoCapture = new VideoCapture();
            videoCapture.ImageGrabbed += ProccesFrame;
            videoCapture.Start();

        }

        private void ProccesFrame(object sender, EventArgs e)
        {

            videoCapture.Retrieve(frame, 0);
            if (!picCapture.CheckAccess())
            {
                picCapture.Dispatcher.Invoke(new Action(delegate ()
                {
                    currentFrame = frame.ToImage<Bgr, Byte>().Resize(Convert.ToInt32(picCapture.Width), Convert.ToInt32(picCapture.Height), Inter.Cubic);
                }));

            }
            else
            {
                currentFrame = frame.ToImage<Bgr, Byte>().Resize(Convert.ToInt32(picCapture.Width), Convert.ToInt32(picCapture.Height), Inter.Cubic);
            }

            //detect faces 
            if (!picCapture.CheckAccess())
            {
                picCapture.Dispatcher.Invoke(new Action(delegate ()
                {

                    if (faceDetectionEnabled)
                    {
                        //Convert from Bgr to Gray Image
                        Mat grayImage = new Mat();
                        CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                        //Enchance the image to get better result
                        CvInvoke.EqualizeHist(grayImage, grayImage);

                        System.Drawing.Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);

                        if (faces.Length > 0)
                        {
                            foreach (var face in faces)
                            {
                                CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);

                                Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                                resultImage.ROI = face;

                                if (faceRecognationEnabled)
                                {
                                    Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                                    CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                                    var result = recognizer.Predict(grayFaceResult);



                                    Debug.WriteLine(result.Label + ". " + result.Distance);
                                    //Here results found known faces
                                    if (result.Label != -1 && result.Distance < 2000)
                                    {
                                        //faceList.Items
                                        string[] personNames = PersonsNames[result.Label].Split(new char[] { ' ' });
                                        string personFirstName = personNames[1];
                                        string personLastName = personNames[1];

                                        var recognizedPerson = new Person(personFirstName, personLastName );

                                        RecognizedPersons.Add(recognizedPerson);

                                        faceList.ItemsSource = RecognizedPersons;

                                       
                                        CvInvoke.PutText(currentFrame, PersonsNames[result.Label], new System.Drawing.Point(face.X - 2, face.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(System.Drawing.Color.Orange).MCvScalar);
                                        CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Green).MCvScalar, 2);

                                    }
                                    //here results did not found any know faces
                                    else
                                    {
                                        CvInvoke.PutText(currentFrame, "Unknown", new System.Drawing.Point(face.X - 2, face.Y - 2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(System.Drawing.Color.Orange).MCvScalar);
                                        CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);

                                    }
                                }

                            }
                        }

                    }
                }));
            }


            else
            {
                if (faceDetectionEnabled)
                {
                    //Convert from Bgr to Gray Image
                    Mat grayImage = new Mat();
                    CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                    //Enchance the image to get better result
                    CvInvoke.EqualizeHist(grayImage, grayImage);

                    System.Drawing.Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, System.Drawing.Size.Empty, System.Drawing.Size.Empty);

                    if (faces.Length > 0)
                    {
                        foreach (var face in faces)
                        {
                            CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);

                            Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                            resultImage.ROI = face;

                            if (faceRecognationEnabled)
                            {
                                Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                                CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                                var result = recognizer.Predict(grayFaceResult);

                                Debug.WriteLine(result.Label + ". " + result.Distance);
                                //Here results found known faces
                                if (result.Label != -1 && result.Distance < 2000)
                                {
                                    CvInvoke.PutText(currentFrame, PersonsNames[result.Label], new System.Drawing.Point(face.X - 2, face.Y - 2),
                                    FontFace.HersheyComplex, 1.0, new Bgr(System.Drawing.Color.Orange).MCvScalar);
                                    CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Green).MCvScalar, 2);
                                }
                                //here results did not found any know faces
                                else
                                {
                                    CvInvoke.PutText(currentFrame, "Unknown", new System.Drawing.Point(face.X - 2, face.Y - 2),
                                    FontFace.HersheyComplex, 1.0, new Bgr(System.Drawing.Color.Orange).MCvScalar);
                                    CvInvoke.Rectangle(currentFrame, face, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
                                }
                            }
                        }
                    }
                }
            }

           


            //Render the video capture into Picture Box picCapture

            if (!picCapture.CheckAccess())
            {
                picCapture.Dispatcher.Invoke(new Action(delegate ()
                {
                    picCapture.Source = BitmapHelpers.ToBitmapSource(currentFrame);
                }));

            }
            else
            {
                picCapture.Source = BitmapHelpers.ToBitmapSource(currentFrame);
            }

        }




        private void btnDetectFaces_Click(object sender, RoutedEventArgs e)
        {
            faceDetectionEnabled = true;

        }

        private void btnRecognizeFaces_Click(object sender, RoutedEventArgs e)
        {
            faceRecognationEnabled = true;
        }
       
    }
}
