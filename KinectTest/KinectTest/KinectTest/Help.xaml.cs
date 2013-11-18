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
    public partial class Help : Page
    {
        KinectSensor _sensor;
        public KinectSensorChooser sensorChooser;

        bool playOrNot = true;
        bool closing = false;
        const int skeletoncount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletoncount];
        //private List<RightHandPoint> RightHandPointsList;
        private List<LeftHandPoint> LeftHandPointsList = new List<LeftHandPoint>();
        private int LeftWave = 0;
        private bool LeftWaveStart = false;
        private bool IsLeftHandWave = false;

        private int VolumeState = 0;
        private bool IsVolumeStart = false;
        private bool VolumeStart = false;
        private double PreviousVolumeValue = 10;// set as default volume

        //private int RotateState = 0;
        //private bool RotateStart = false;

        private bool IsRightSwipeStart = false;
        private bool IsLeftSwipeStart = false;

        private string pageName;
        System.Threading.Timer timer;


        public Help()
        {
            InitializeComponent();

            // initialize the sensor chooser and UI
            if (sensorChooser == null)
            {
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser = Generics.GlobalKinectSensorChooser;
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            }
        }

        public Help(string s)
            : this()
        {
            this.pageName = s;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
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

        //define a struct to store points; (X,Y,Z,T)
        public struct RightHandPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (RightHandPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of GesturePoint

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
        }// end of GesturePoint


        public struct RightShoulderPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (RightShoulderPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of RightShoulderPoint

        public struct LeftShoulderPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (LeftShoulderPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of LeftShoulderPoint


        public struct RightElbowPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (RightElbowPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of RightElbowPoint

        public struct LeftElbowPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (LeftElbowPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of LeftElbowPoint

        public struct RightWristPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (RightWristPoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of RightWristPoint

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

        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
                return;

            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);
                int stride = colorFrame.Width * 4; // RGB+ empty color
                cameraImage.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height
                    , 96, 96, PixelFormats.Bgr32, null, pixels, stride);

            }

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

                // right hand point information
                DepthImagePoint RightHandDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                RightHandPoint newRightHandPoint = new RightHandPoint()
                {
                    X = RightHandDepthPoint.X,
                    Y = RightHandDepthPoint.Y,
                    Z = RightHandDepthPoint.Depth,
                    T = DateTime.Now
                };


                //left hand point information
                DepthImagePoint LeftHandDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                LeftHandPoint newLeftHandPoint = new LeftHandPoint()
                {
                    X = LeftHandDepthPoint.X,
                    Y = LeftHandDepthPoint.Y,
                    Z = LeftHandDepthPoint.Depth,
                    T = DateTime.Now
                };

                // right shoulder point information
                DepthImagePoint RightShoulderDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.ShoulderRight].Position);
                RightShoulderPoint newRightShoulderPoint = new RightShoulderPoint()
                {
                    X = RightShoulderDepthPoint.X,
                    Y = RightShoulderDepthPoint.Y,
                    Z = RightShoulderDepthPoint.Depth,
                    T = DateTime.Now
                };

                // left shoulder point information
                DepthImagePoint LeftShoulderDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.ShoulderLeft].Position);
                LeftShoulderPoint newLeftShoulderPoint = new LeftShoulderPoint()
                {
                    X = LeftShoulderDepthPoint.X,
                    Y = LeftShoulderDepthPoint.Y,
                    Z = LeftShoulderDepthPoint.Depth,
                    T = DateTime.Now
                };

                // right elbow point
                DepthImagePoint RightElbowDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.ElbowRight].Position);
                RightElbowPoint newRightElbowPoint = new RightElbowPoint()
                {
                    X = RightElbowDepthPoint.X,
                    Y = RightElbowDepthPoint.Y,
                    Z = RightElbowDepthPoint.Depth,
                    T = DateTime.Now
                };

                // left elbow point
                DepthImagePoint LeftElbowDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.ElbowLeft].Position);
                LeftElbowPoint newLeftElbowPoint = new LeftElbowPoint()
                {
                    X = LeftElbowDepthPoint.X,
                    Y = LeftElbowDepthPoint.Y,
                    Z = LeftElbowDepthPoint.Depth,
                    T = DateTime.Now
                };

                DepthImagePoint RightWristDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.WristRight].Position);
                RightWristPoint newRightWristPoint = new RightWristPoint()
                {
                    X = RightWristDepthPoint.X,
                    Y = RightWristDepthPoint.Y,
                    Z = RightWristDepthPoint.Depth,
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
                    StatusLabel.Content = "";
                    LeftHandPointsList.Clear();
                    return;
                }

                StatusLabel.Content = "Control Mode(1.7m~2m): " + newHeadPoint.Z / 1000 + "m";

                // the left hand  push event;
                if (newLeftHandPoint.X > newLeftElbowPoint.X)
                {
                    Push(newLeftHandPoint, newLeftElbowPoint);
                }


                if (newLeftHandPoint.Y < newHeadPoint.Y)// left hand wave to quit
                {
                    // MessageBox.Show("Left wave");
                    LeftHandWave(newLeftHandPoint, newHeadPoint);
                }
                else
                {
                    IsLeftHandWave = false;
                }

                //Volume control
                if (Math.Abs(newRightHandPoint.Y - newHeadPoint.Y) < 150
                   && newRightHandPoint.X - newRightShoulderPoint.X > -30)
                {
                    VolumeControl(newRightHandPoint, newHeadPoint);
                }
                else
                {
                    IsVolumeStart = false;
                }


                if (newRightHandPoint.Y > newRightShoulderPoint.Y
                    && newRightHandPoint.X > newHeadPoint.X + 200)//right swipe
                {
                    //trigger the right swipe gesture
                    rightSwipeGesture();
                }
                else
                {
                    IsRightSwipeStart = false;
                }

                //left swipe
                if (newLeftHandPoint.Y > newLeftShoulderPoint.Y 
                    && newLeftHandPoint.X < newHeadPoint.X - 200)
                {
                    leftSwipeGesture();
                }
                else
                {
                    IsLeftSwipeStart = false;
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

        private void VolumeControl(RightHandPoint newRightHandPoint, HeadPoint newHeadPoint)
        {
            //volumeLabel.Content = "Volume:" + VolumeSlider.Value;
            // VolumeSlider.Visibility = System.Windows.Visibility.Visible;
            switch (VolumeState)
            {
                case 0:
                    if (newRightHandPoint.Y == newHeadPoint.Y
                        && Math.Abs(newHeadPoint.X - newRightHandPoint.X) > 40)
                    {
                        VolumeState = 1;
                        GestureLabel.UpdateLayout();
                        if (GestureLabel.Visibility == Visibility.Hidden)
                        {
                            GestureLabel.Content = "Volume Control Stage 1";
                            GestureLabel.Visibility = Visibility.Visible;
                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           GestureLabel.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);

                        }
                        else
                        {
                            GestureLabel.Content = "Volume Control Stage 1";
                        }
                        //volumeLabel.Content = "Volume:" + VolumeSlider.Value;
                    }
                    break;
                case 1:
                    if (newRightHandPoint.X - newHeadPoint.X < 50)
                    {
                        VolumeState = 2;
                        IsVolumeStart = true;
                        GestureLabel.UpdateLayout();
                        if (GestureLabel.Visibility == Visibility.Hidden)
                        {
                            GestureLabel.Content = "Volume Control Stage 2";
                            GestureLabel.Visibility = Visibility.Visible;
                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           GestureLabel.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);

                        }
                        else
                        {
                            GestureLabel.Content = "Volume Control Stage 2";
                        }
                        //MessageBox.Show("Value Changed1 ");
                    }

                    break;
                default:
                    break;
            }


            if (!VolumeStart && IsVolumeStart == true)
            {

                IsVolumeStart = false;
                VolumeStart = true;
                VolumeState = 0;
                GestureLabel.UpdateLayout();
                if (GestureLabel.Visibility == Visibility.Hidden)
                {
                    GestureLabel.Content = "Volume Control Succeed!";
                    GestureLabel.Visibility = Visibility.Visible;
                    timer = new System.Threading.Timer(
                           (state) =>
                           {
                               GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                               {
                                   GestureLabel.Visibility = Visibility.Hidden;
                               }));
                           }, null, 1000, Int32.MaxValue);

                }
                else
                {
                    GestureLabel.Content = "Volume Control Succeed!";
                }
                
            }
            else
            {
                VolumeStart = false;
            }

        }

        private void rightSwipeGesture()
        {
            if (!IsRightSwipeStart)
            {
                IsRightSwipeStart = true;
                // write the control here

                GestureLabel.UpdateLayout();
                if (GestureLabel.Visibility == Visibility.Hidden)
                {
                    GestureLabel.Content = "Forward/Next Photo";
                    GestureLabel.Visibility = Visibility.Visible;
                    timer = new System.Threading.Timer(
                           (state) =>
                           {
                               GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                               {
                                   GestureLabel.Visibility = Visibility.Hidden;
                               }));
                           }, null, 1000, Int32.MaxValue);

                }
                else
                {
                    GestureLabel.Content = "Forward/Next Photo";
                }
            }
        }// end of righte swipte gesture


        private void leftSwipeGesture()
        {
            if (!IsLeftSwipeStart)
            {
                IsLeftSwipeStart = true;
                //write control here
                GestureLabel.UpdateLayout();
                if (GestureLabel.Visibility == Visibility.Hidden)
                {
                    GestureLabel.Content = "Rewind/Previous Photo";
                    GestureLabel.Visibility = Visibility.Visible;
                    timer = new System.Threading.Timer(
                           (state) =>
                           {
                               GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                               {
                                   GestureLabel.Visibility = Visibility.Hidden;
                               }));
                           }, null, 1000, Int32.MaxValue);

                }
                else
                {
                    GestureLabel.Content = "Rewind/Previous Photo";
                }
            }
        }


        //left hand push gesture
        private void Push(LeftHandPoint newLeftHandPoint, LeftElbowPoint newLeftElbowPoint)
        {
            LeftHandPoint startLeftHandPoint;
            LeftHandPointsList.Add(newLeftHandPoint);
            startLeftHandPoint = LeftHandPointsList[0];

            // check if press gesture is in the boundary box;
            if (Math.Abs(startLeftHandPoint.X - newLeftHandPoint.X) > 100
                    || Math.Abs(startLeftHandPoint.Y - newLeftHandPoint.Y) > 100)
            {
                LeftHandPointsList.Clear();
                return;
            }


            if (Math.Abs(startLeftHandPoint.X - newLeftHandPoint.X) < 100
                && Math.Abs(startLeftHandPoint.Y - newLeftHandPoint.Y) < 100)
            {
                if ((newLeftHandPoint.T - startLeftHandPoint.T).Milliseconds > 700)
                {
                    LeftHandPointsList.RemoveAt(0);
                    startLeftHandPoint = LeftHandPointsList[0];
                }

                if (startLeftHandPoint.Z - newLeftHandPoint.Z > 250)
                {
                    LeftHandPointsList.Clear();

                    
                        GestureLabel.UpdateLayout();
                        if (GestureLabel.Visibility == Visibility.Hidden)
                        {
                            GestureLabel.Content = "Play/Pause";
                            GestureLabel.Visibility = Visibility.Visible;
                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           GestureLabel.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);

                        }
                        else
                        {
                            GestureLabel.Content = "Play/Pause";
                        }
                    

                }

            }

            // end of lefthand press


        }

        //a very simple version of left hand wave
        private void LeftHandWave(LeftHandPoint newLeftHandPoint, HeadPoint newHeadPoint)
        {
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            if (!IsLeftHandWave && newHeadPoint.X - newLeftHandPoint.X > 200)
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            {
                IsLeftHandWave = true;
                GestureLabel.UpdateLayout();
                if (GestureLabel.Visibility == Visibility.Hidden)
                {
                    GestureLabel.Content = "Quit";
                    GestureLabel.Visibility = Visibility.Visible;
                    timer = new System.Threading.Timer(
                           (state) =>
                           {
                               GestureLabel.Dispatcher.BeginInvoke((Action)(() =>
                               {
                                   GestureLabel.Visibility = Visibility.Hidden;
                               }));
                           }, null, 1000, Int32.MaxValue);

                }
                else
                {
                    GestureLabel.Content = "Quit";
                }
                _sensor.AllFramesReady -= _sensor_AllFramesReady;
                (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.RelativeOrAbsolute);

            }



        }



    }
}
