using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

namespace FaceRec
{
    /// <summary>
    /// Interaction logic for FindPerson.xaml
    /// </summary>
    public partial class FindPerson : UserControl
    {
        public FindPerson()
        {
            InitializeComponent();
        }
    }
}
