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
using System.Windows.Threading;

namespace KinectTest
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class SongPlayer : Page
    {
        KinectSensor _sensor;
        bool playOrNot = true;
        bool closing = false;
        const int skeletoncount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletoncount];
        //private List<RightHandPoint> RightHandPointsList;
        private List<LeftHandPoint> LeftHandPointsList;
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

        System.Threading.Timer timer;

      
        private string songName;
        public SongPlayer()
        {
            InitializeComponent();
            LeftHandPointsList = new List<LeftHandPoint>();
           
        }
       

        
        ~SongPlayer()
        {
            //MessageBox.Show("Destroy songplayer");
        }

        public SongPlayer(string s)
            : this()
        {
            this.songName = s;
            //this._sensor = w.Kinect;
            MoviePlayer.Source = new Uri(@"F:\KinectTest\KinectTest\KinectTest\Musics\" + songName + ".MP3", UriKind.Absolute);
            MoviePlayer.Play();
        }
        public SongPlayer(string s,KinectSensorChooser w) : this()
        {
            this.songName = s;
            this._sensor = w.Kinect;
            MoviePlayer.Source = new Uri(@"F:\KinectTest\KinectTest\KinectTest\Musics\" + songName + ".MP3", UriKind.Absolute);
            MoviePlayer.Play();
        }

        private void player_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += (ss, ee) =>
            {
                //显示当前视频进度
                var ts = MoviePlayer.Position;
                label1.Content = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                ProgressBar.Value = ts.TotalMilliseconds;
            };
            timer.Start();
        }



        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            //显示视频的时长
            var ts = MoviePlayer.NaturalDuration.TimeSpan;
            label2.Content = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            ProgressBar.Maximum = ts.TotalMilliseconds;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //调整视频进度
            var ts = TimeSpan.FromMilliseconds(e.NewValue);

            MoviePlayer.Position = ts;
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
                      
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
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

                //user should stand in the right place before eveything start
                // the two if condition requires the user to stand in front of Kinect in a box area
                if (newHeadPoint.Z < 1700 || newHeadPoint.Z > 2000)
                {
                    StatusLabel.Content = "";
                    ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                    label1.Visibility = System.Windows.Visibility.Hidden;
                    label2.Visibility = System.Windows.Visibility.Hidden;
                    LeftHandPointsList.Clear();
                    return;
                }
                


                StatusLabel.Content = "Control Mode(1.7m~2m): " + newHeadPoint.Z / 1000 + "m";
                ProgressBar.Visibility = System.Windows.Visibility.Visible;
                label1.Visibility = System.Windows.Visibility.Visible;
                label2.Visibility = System.Windows.Visibility.Visible;
                // the left hand  push event;
                if (newLeftHandPoint.X > newLeftElbowPoint.X)
                {
                    Push(newLeftHandPoint, newLeftElbowPoint);
                }


                if (newLeftHandPoint.Y < newHeadPoint.Y)// left hand wave to quit
                {
                    // MessageBox.Show("Left wave");
                    LeftHandWave(newLeftHandPoint, newHeadPoint, newLeftElbowPoint);
                }
                else
                {
                    IsLeftHandWave = false;
                }

                //Volume control
                if (Math.Abs(newRightHandPoint.Y - newHeadPoint.Y) < newRightShoulderPoint.Y - newHeadPoint.Y
                   && Math.Abs(newRightHandPoint.X - newRightShoulderPoint.X) < newRightShoulderPoint.X - newHeadPoint.X)
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
                if (newLeftHandPoint.Y > newLeftShoulderPoint.Y && newLeftHandPoint.X < newHeadPoint.X - 200)
                {
                    leftSwipeGesture();
                }
                else
                {
                    IsLeftSwipeStart = false;
                }
          
            }
        }// end of GetCamera point



        private void VolumeControl(RightHandPoint newRightHandPoint, HeadPoint newHeadPoint)
        {
            //volumeLabel.Content = "Volume:" + VolumeSlider.Value;
            // VolumeSlider.Visibility = System.Windows.Visibility.Visible;
            switch (VolumeState)
            {
                case 0:
                    if (newRightHandPoint.Y == newHeadPoint.Y)
                    {
                        VolumeState = 1;
                        //volumeLabel.Content = "Volume:" + VolumeSlider.Value;
                    }
                    break;
                case 1:
                    VolumeSlider.Value = PreviousVolumeValue + (newHeadPoint.Y - newRightHandPoint.Y);
                    VolumeSlider.Visibility = System.Windows.Visibility.Visible;
                    volumeLabel.Content = "Volume:" + VolumeSlider.Value;// ( newHeadPoint.Y - newRightHandPoint.Y )

                    if (newRightHandPoint.X - newHeadPoint.X < 50)
                    {
                        VolumeState = 2;
                        IsVolumeStart = true;
                        //MessageBox.Show("Value Changed1 ");
                    }

                    break;
                default:
                    break;
            }


            if (!VolumeStart && IsVolumeStart == true)
            {
                //if (!VolumeStart)
                // MessageBox.Show("Value Changed2 " + (newHeadPoint.Y - newRightHandPoint.Y));
                IsVolumeStart = false;
                PreviousVolumeValue = VolumeSlider.Value;
                volumeLabel.Content = "";
                VolumeSlider.Visibility = System.Windows.Visibility.Hidden;
                VolumeStart = true;
                VolumeState = 0;
                //VolumeSlider.Value += newHeadPoint.Y - RightHandPointsList.Last().Y;
                //return;
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
                MoviePlayer.Position = MoviePlayer.Position + TimeSpan.FromSeconds(5);
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
            }
        }// end of righte swipte gesture


        private void leftSwipeGesture()
        {
            if (!IsLeftSwipeStart)
            {
                IsLeftSwipeStart = true;
                //write control here
                MoviePlayer.Position = MoviePlayer.Position - TimeSpan.FromSeconds(5);
                Label1.UpdateLayout();
                if (Label1.Visibility == Visibility.Hidden)
                {
                    Label1.Content = "BACKWARD";
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
                    Label1.Content = "BACKWARD";
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

            // end of lefthand press


        }

        //a very simple version of left hand wave
        private void LeftHandWave(LeftHandPoint newLeftHandPoint, HeadPoint newHeadPoint, LeftElbowPoint newLeftElbowPoint)
        {
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            if (!IsLeftHandWave && newHeadPoint.X - newLeftHandPoint.X > 200)
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            {
                IsLeftHandWave = true;
                _sensor.AllFramesReady -= _sensor_AllFramesReady;
                GC.Collect();
                (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("Song.xaml", UriKind.RelativeOrAbsolute);
            }

            // closing the event handler


        }

        private void ResetGesturePoints(List<RightHandPoint> gesturePoints)
        {
            for (int i = 0; i < gesturePoints.Count; i++)
            {
                gesturePoints.RemoveAt(i);
            }
            // throw new NotImplementedException();
        }

        
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

        private void MoviePlayer_MediaEnded_1(object sender, RoutedEventArgs e)
        {
            _sensor.AllFramesReady -=_sensor_AllFramesReady;
            (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("Song.xaml", UriKind.Relative);
        }


    }

}

