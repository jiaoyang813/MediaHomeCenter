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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class SongPlayer : Page
    {
       KinectSensor _sensor;
        bool playOrNot = false;
        bool closing = false;
        const int skeletoncount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletoncount];
        //private List<RightHandPoint> RightHandPointsList;
        private List<LeftHandPoint> LeftHandPointsList;
        private int LeftWave = 0;
        private bool LeftWaveStart = false;
        private int VolumeState = 0;
        private bool VolumeStart = false;

        private int RotateState = 0;
        private bool RotateStart = false;

        System.Threading.Timer timer;

        //System.Timers.Timer volumeUpTimer;
        //System.Timers.Timer volumeDownTimer;

        //private int counter = 0;
        // private List<double> historyPoints;
        private string songName;
        public SongPlayer()
        {
            InitializeComponent();
            LeftHandPointsList = new List<LeftHandPoint>();
           // RightHandPointsList = new List<RightHandPoint>();
            //VolumeDownPointsList = new List<RightHandPoint>();


            //volumeUpTimer = new System.Timers.Timer(500);
            //volumeUpTimer.Elapsed += new System.Timers.ElapsedEventHandler(volumeUpTimer_Elapsed);
            //volumeUpTimer.AutoReset = true;
            //volumeUpTimer.Enabled = false;

            //volumeDownTimer = new System.Timers.Timer(500);
            //volumeDownTimer.Elapsed += new System.Timers.ElapsedEventHandler(volumeDownTimer_Elapsed);
        }
        public SongPlayer(string s,KinectSensorChooser w) : this()
        {
            this.songName = s;
            this._sensor = w.Kinect;
            MoviePlayer.Source = new Uri(@"F:\KinectTest\KinectTest\KinectTest\Musics\" + songName + ".MP3", UriKind.Absolute);
        }
        //private void volumeUpTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    volumeUpTimer.Enabled = false;
        //   if(MoviePlayer.Volume<=100) MoviePlayer.Volume += 5;
        //   //Label1.Content = MoviePlayer.Volume;
        //   MessageBox.Show(MoviePlayer.Volume.ToString());
        //}

        //private void volumeDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    volumeDownTimer.Enabled = false;
        //    if(MoviePlayer.Volume>=5) MoviePlayer.Volume -= 5;
        //    //Label1.Content = MoviePlayer.Volume;
        //    // throw new NotImplementedException();
        //    MessageBox.Show(MoviePlayer.Volume.ToString());
        //}
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
                    // MessageBox.Show("Started");

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
        private void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
                return;

            GetCameraPoint(first, e);
          //  throw new NotImplementedException();
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
        }// end of RightHand

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
        public struct SpinePoint
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public DateTime T { get; set; }

            public override bool Equals(object obj)
            {
                var o = (SpinePoint)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }// end of SpinePoint

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
                RightHandPoint startRightHandPoint;

                //left hand point information
                DepthImagePoint LeftHandDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                LeftHandPoint newLeftHandPoint = new LeftHandPoint()
                {
                    X = LeftHandDepthPoint.X,
                    Y = LeftHandDepthPoint.Y,
                    Z = LeftHandDepthPoint.Depth,
                    T = DateTime.Now
                };
                LeftHandPoint startLeftHandPoint;
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

                //DepthImagePoint HipCenterDepthPoint = depthFrame.MapFromSkeletonPoint(first.Joints[JointType.HipCenter].Position);
                //SpinePoint newSpinePoint = new SpinePoint()
                //{
                //    X = HipCenterDepthPoint.X,
                //    Y = HipCenterDepthPoint.Y,
                //    Z = HipCenterDepthPoint.Depth,
                //    T = DateTime.Now
                //};

                //MessageBox.Show(newHeadPoint.X.ToString() + newHeadPoint.Y.ToString());

              


                //Volume Up and Down
                double x1 = newRightElbowPoint.X;
                double y1 = newRightElbowPoint.Y;// elbow point(x1, y1)
                double x2 = newRightWristPoint.X;
                double y2 = newRightWristPoint.Y;// Wrist point(x2, y2)
                double x3 = newRightHandPoint.X;
                double y3 = newRightHandPoint.Y; // hand point (x3, y3)

                double angle1 = Math.Abs(y2 - y1) / Math.Abs(x2 - x1);
                double angle2 = Math.Abs(y2 - y3) / Math.Abs(x2 - x3);


                if (newRightHandPoint.X > newRightShoulderPoint.X + 100)
                {
                    VolumeStart = true;
                    switch (VolumeState)
                    {
                        case 0:
                            if (newRightHandPoint.Y < newRightShoulderPoint.Y)
                            {
                                VolumeState = 1; // enter volume up   
                            }// right hand in upper position
                            if (newRightHandPoint.Y > newRightShoulderPoint.Y)
                            {
                                VolumeState = 2; // right hand in lower position   
                            }
                            break;
                        case 1:

                            if (newRightHandPoint.Y > newRightShoulderPoint.Y)
                            {
                                VolumeState = 2; // right hand in lower position
                            }
                            else if (Math.Abs(angle1 - angle2) > 0.5) //angle change
                            {
                                VolumeState = 3;
                                // MessageBox.Show("VolumeUp");    
                            }
                            break;

                        case 2:

                            if (newRightHandPoint.Y < newRightShoulderPoint.Y)
                            {
                                VolumeState = 1;  // right hand in upper position  
                            }
                            else if (Math.Abs(angle1 - angle2) > 0.9)//angle change
                            {
                                VolumeState = 4;
                                //MessageBox.Show("VolumeDown");    
                            }
                            break;

                    }
                }

                if (Math.Abs(newRightHandPoint.X - newRightShoulderPoint.X) < 50 && newRightHandPoint.Y > newRightElbowPoint.Y && VolumeStart == true)
                {
                    if (VolumeState == 3)
                    {
                        VolumeStart = false;
                        VolumeState = 0;
                        MoviePlayer.Volume += 10;
                        Label1.UpdateLayout();
                        if (Label1.Visibility == Visibility.Hidden)
                        {
                            Label1.Content = "VOLUME " + MoviePlayer.Volume.ToString();
                            Label1.Visibility = Visibility.Visible;


                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       Label1.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           Label1.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);


                        }
                        else
                        {
                            Label1.Content = "VOLUME " + MoviePlayer.Volume.ToString();
                        }
                        // MessageBox.Show(MoviePlayer.Volume.ToString());
                        return;
                    }

                    if (VolumeState == 4)
                    {
                        VolumeStart = false;
                        VolumeState = 0;
                        MoviePlayer.Volume -= 10;
                        Label1.UpdateLayout();
                        if (Label1.Visibility == Visibility.Hidden)
                        {
                            Label1.Content = "VOLUME " + MoviePlayer.Volume.ToString();
                            Label1.Visibility = Visibility.Visible;


                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       Label1.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           Label1.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);


                        }
                        else
                        {
                            Label1.Content = "VOLUME " + MoviePlayer.Volume.ToString();
                        }
                        //MessageBox.Show(MoviePlayer.Volume.ToString());
                        return;
                    }
                }//end of volume control
                // rotate gesture to move forward and backward
                // state 0 = in the body area, 1 = spine, 2 = rightshoulder, 3 = head, 4 = leftshoulder
                if (newRightHandPoint.X > newLeftShoulderPoint.X && newRightHandPoint.X < newRightShoulderPoint.X)
                       // && newRightHandPoint.Y < newSpinePoint.Y && newRightHandPoint.Y > newHeadPoint.Y)
                {
                    RotateStart = true;
                   

                    switch (RotateState)
                    {
                       
                        case 0:
                            if (Math.Abs(newRightHandPoint.X - newRightShoulderPoint.X) < 70 &&
                                Math.Abs(newRightHandPoint.Y - newRightShoulderPoint.Y) < 70)
                            {
                                RotateState = 1;   // leftSHoulder
                                //MessageBox.Show("Enter Rotate2");
                            }
                            break;

                        case 1:
                            if (Math.Abs(newRightHandPoint.X - newHeadPoint.X) < 70 &&
                                Math.Abs(newRightHandPoint.Y - newHeadPoint.Y) < 70)
                            {
                                RotateState = 2;   // head
                                //MessageBox.Show("Enter Rotate3");
                            }
                            break;
                        case 2:
                            if (Math.Abs(newRightHandPoint.X - newLeftShoulderPoint.X) < 70 &&
                                Math.Abs(newRightHandPoint.Y - newLeftShoulderPoint.Y) < 70)
                            {
                                RotateState = 3;   // leftSHoulder
                                //MessageBox.Show("Enter Rotate4");
                            }
                            break;
                        default:
                            break;
                    }

                }


                if ( RotateStart == true)
                {
                    if (RotateState == 3)
                    {
                        RotateStart = false;
                        RotateState = 0;
                        MoviePlayer.Position=MoviePlayer.Position+TimeSpan.FromSeconds(5);
                        Label1.UpdateLayout();
                        if (Label1.Visibility == Visibility.Hidden)
                        {
                            Label1.Content = "FORWARD";
                            Label1.Visibility = Visibility.Visible;


                            timer = new System.Threading.Timer(
                                   (state) =>
                                   {
                                       Label1.Dispatcher.BeginInvoke((Action)(() =>
                                       {
                                           Label1.Visibility = Visibility.Hidden;
                                       }));
                                   }, null, 1000, Int32.MaxValue);


                        }
                        else
                        {
                            Label1.Content = "FORWARD";
                        }
                        return;
                    }
                } // end of rotate backward
                //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
                if (newLeftHandPoint.Y < newHeadPoint.Y)
                {
                    LeftWaveStart = true;

                    switch (LeftWave)
                    {
                        case 0:
                            if (newLeftHandPoint.Y < newHeadPoint.Y)
                                //seg 1 of leftwave
                                LeftWave = 1;
                            break;
                        case 1:
                            if (newLeftHandPoint.X > newLeftElbowPoint.X + 50)
                                LeftWave = 2;
                            break;
                        case 2:
                            if (newLeftHandPoint.X + 50 < newLeftElbowPoint.X)
                                LeftWave = 3;
                            break;
                        //case 3:
                        //    if (newLeftHandPoint.Y > newLeftShoulderPoint.Y)
                        //        LeftWave = 4;
                        //    break;
                    }

                }
                // case 4: putdown hands
                if (newLeftHandPoint.Y > newLeftElbowPoint.Y && LeftWaveStart == true)
                {
                    if (LeftWave == 3)
                    {
                        LeftWaveStart = false;
                        LeftWave = 0;
                        (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative);
                        return;
                    }
                } //end of waveleft


                // the  press event;
                if (newLeftHandPoint.X > newLeftElbowPoint.X)
                {
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
                        if ((newLeftHandPoint.T - startLeftHandPoint.T).Milliseconds > 500)
                        {
                            LeftHandPointsList.RemoveAt(0);
                            startLeftHandPoint = LeftHandPointsList[0];

                        }

                        if (startLeftHandPoint.Z - newLeftHandPoint.Z > 300)
                        {
                            LeftHandPointsList.Clear();

                            if (!playOrNot)
                            {
                                MoviePlayer.Play();
                                playOrNot = true;
                                Label1.UpdateLayout();
                                if (Label1.Visibility == Visibility.Hidden)
                                {
                                    Label1.Content = "PLAY";
                                    Label1.Visibility = Visibility.Visible;


                                    timer = new System.Threading.Timer(
                                           (state) =>
                                           {
                                               Label1.Dispatcher.BeginInvoke((Action)(() =>
                                               {
                                                   Label1.Visibility = Visibility.Hidden;
                                               }));
                                           }, null, 1000, Int32.MaxValue);


                                }
                                else
                                {
                                    Label1.Content = "PLAY";
                                }
                            }
                            else
                            {
                                MoviePlayer.Pause();
                                playOrNot = false;
                                Label1.UpdateLayout();
                                if (Label1.Visibility == Visibility.Hidden)
                                {
                                    Label1.Content = "PAUSE";
                                    Label1.Visibility = Visibility.Visible;


                                    timer = new System.Threading.Timer(
                                           (state) =>
                                           {
                                               Label1.Dispatcher.BeginInvoke((Action)(() =>
                                               {
                                                   Label1.Visibility = Visibility.Hidden;
                                               }));
                                           }, null, 1000, Int32.MaxValue);


                                }
                                else
                                {
                                    Label1.Content = "PAUSE";
                                }
                            }

                        }

                    }

                    //throw new NotImplementedException();

                }// end of lefthand press
            }
        }// end of GetCamera point






        private void ResetGesturePoints(List<RightHandPoint> gesturePoints)
        {
            for (int i = 0; i < gesturePoints.Count; i++)
            {
                gesturePoints.RemoveAt(i);
            }
            // throw new NotImplementedException();
        }

        //private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        //{
        //    Canvas.SetLeft(element, point.X - element.Width / 2 );
        //    Canvas.SetTop(element, point.Y - element.Height / 2 );

        //    //throw new NotImplementedException();
        //}

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
            //   throw new NotImplementedException();
        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                closing = true;
                _sensor.AllFramesReady -= new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
            }
        }

        // private void Page_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //   StopKinect(_sensor);
        //}


    }

}

