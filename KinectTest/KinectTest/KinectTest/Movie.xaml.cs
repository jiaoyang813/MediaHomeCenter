using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for Movie.xaml
    /// </summary>
    public partial class Movie : Page
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensor _sensor;
        private string movieName;
        const int skeletoncount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletoncount];
        private bool IsLeftHandWave = false;


        public Movie()
        {
            InitializeComponent();
            
            // initialize the sensor chooser and UI
            if (sensorChooser == null)
            {
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser = Generics.GlobalKinectSensorChooser;
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;

                // Bind the sensor chooser's current sensor to the KinectRegion
                var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
                BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
            }
        }

        public Movie(string s) : this()
        {
            this.movieName = s;
           
        }
        
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

 

        private void movieButton1_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            //_sensor.Stop();
            Player newplayer = new Player("Frozen");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton2_Click(object sender, RoutedEventArgs e)
        {

            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("Gravity");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton3_Click(object sender, RoutedEventArgs e)
        {

            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("JackRyan");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton4_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("Linsanity");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton5_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("Rio2");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton8_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("EndersGame");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton7_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("Vikingdom");
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton6_Click(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -= _sensor_AllFramesReady;
            Player newplayer = new Player("TheHobbit");
            this.NavigationService.Navigate(newplayer);
        }

  
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //_sensor = Generics.GlobalKinectSensorChooser.Kinect;
            if (_sensor != null)
            {
                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                    _sensor.Start();
                }
            }
            else
            {
                // at least one sensor
                if (KinectSensor.KinectSensors.Count > 0)
                {
                    _sensor = KinectSensor.KinectSensors[0];
                    if (_sensor.Status == KinectStatus.Connected)
                    {
                        _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                        _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                        _sensor.SkeletonStream.Enable();
                        _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                        _sensor.Start();
                        // MessageBox.Show("Started");

                    }
                }
            }
        }

        public struct HeadPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (HeadPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of HeadPoint

        public struct LeftHandPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (LeftHandPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }
            
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of LeftHand

        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
     
            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
                return;

            GetCameraPoint(first, e);
          //  throw new NotImplementedException();
        }

        private void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null || _sensor == null)
                    return;

                // DepthImagePoint headDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);

               
                //left hand point information
                DepthImagePoint LeftHandDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                LeftHandPoint newLeftHandPoint = new LeftHandPoint()
                {
                    X = LeftHandDepthPoint.X,
                    Y = LeftHandDepthPoint.Y,
                    Z = LeftHandDepthPoint.Depth,
                    T = DateTime.Now
                };

           
                DepthImagePoint HeadDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                HeadPoint newHeadPoint = new HeadPoint()
                {
                    X = HeadDepthPoint.X,
                    Y = HeadDepthPoint.Y,
                    Z = HeadDepthPoint.Depth,
                    T = DateTime.Now
                };

                //user should stand in the right place before eveything start
                // the two if condition requires the user to stand in front of Kinect in a box area
                if (newHeadPoint.Z < 1700 || newHeadPoint.Z > 2000)
                {
                    StatusLabel.Visibility = System.Windows.Visibility.Hidden;
                    StatusLabel.Content = "";
                    return;
                }

                StatusLabel.Visibility = System.Windows.Visibility.Visible;
                StatusLabel.Content = "Control Mode(1.7m~2m): " + newHeadPoint.Z / 1000 + "m";
                // left hand wave to quit
                if (newLeftHandPoint.Y < newHeadPoint.Y)
                {
                    // MessageBox.Show("Left wave");
                    LeftHandWave(newLeftHandPoint, newHeadPoint);
                }
                else
                {
                    IsLeftHandWave = false;
                }

               
            }// end of using statement
        }

        // end of GetCamera point
             private Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
             {
                 using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
                 {
                     if (skeletonFrameData == null)
                         return null;

                     skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                     //get the first tracked skeleton
                     Skeleton first = (from s in allSkeletons
                                       where s.TrackingState == SkeletonTrackingState.Tracked
                                       select s).FirstOrDefault();

                     return first;
                 }
                
             }

             //a very simple version of left hand wave
             private void LeftHandWave(LeftHandPoint newLeftHandPoint, HeadPoint newHeadPoint)
             {
                 //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
                 if (!IsLeftHandWave && newHeadPoint.X - newLeftHandPoint.X > 200)
                 //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
                 {
                     IsLeftHandWave = true;
                     (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.RelativeOrAbsolute);
                        
                 }
                
                
                 
             }


        
    }
}
