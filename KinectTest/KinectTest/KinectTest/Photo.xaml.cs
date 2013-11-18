using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.ComponentModel;
using System.IO;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Windows.Threading;
namespace KinectTest
{
    /// <summary>
    /// Interaction logic for Photo.xaml
    /// </summary>
    public partial class Photo : Page
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

        private int photoCount=1;
        private bool photoForwardOrNot=true;

        private string photoName="1";
        public Photo()
        {
            InitializeComponent();
            _sensor = Generics.GlobalKinectSensorChooser.Kinect;
            LeftHandPointsList = new List<LeftHandPoint>();
        }
        public Photo(string s) : this()
        {
            this.photoName = s;
            
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

             

                if (newHeadPoint.Z < 1700 || newHeadPoint.Z > 2000)
                {
                    StatusLabel.Content = "";
                    
                    return;
                }

                StatusLabel.Content = "Control Mode(1.7m~2m): "+ newHeadPoint.Z/1000+ "m";
                
               if (newRightHandPoint.Y - newHeadPoint.Y < 0
                   && newRightHandPoint.X - newRightShoulderPoint.X > -30)
                {
                    VolumeControl(newRightHandPoint, newHeadPoint);
                }
                else
                {
                    IsVolumeStart = false;
                }

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
                     //MessageBox.Show("Swipe left");
                     leftSwipeGesture();
                 }
                 else
                 {
                     IsLeftSwipeStart = false;
                 }
                        
            }
 } // end of GetCamera point
             
        private void rightSwipeGesture()
        {
            if (!IsRightSwipeStart)
            {
                IsRightSwipeStart = true;
                // write the control here
                if (photoCount <= 9 )
                {
                    photoCount += 1;
                    Label1.Content = photoCount.ToString() + "/10";
                    Image.Source = new BitmapImage(new Uri(@"F:\KinectTest\KinectTest\KinectTest\Photo\" + photoCount.ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                }
            }
        }// end of righte swipte gesture


        private void leftSwipeGesture()
        {
            if (!IsLeftSwipeStart)
            {
                IsLeftSwipeStart = true;
                //write control here
               if (photoCount >=2 )
                {
                    photoCount -= 1;
                    Label1.Content = photoCount.ToString() + "/10";
                    Image.Source = new BitmapImage(new Uri(@"F:\KinectTest\KinectTest\KinectTest\Photo\" + photoCount.ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                }
                            
            }
        }


		private void VolumeControl(RightHandPoint newRightHandPoint, HeadPoint newHeadPoint)
		{
			if(!IsVolumeStart)
			{
			    IsVolumeStart = true;
			
			    StopKinect(_sensor);
                        
			    statusLabel.Content = "Loading...";
                _sensor.AllFramesReady -= _sensor_AllFramesReady;
			    PhotoVoiceControl newVoice = new PhotoVoiceControl("1",_sensor);
			    this.NavigationService.Navigate(newVoice);
			    return;
			}
		}


        private void LeftHandWave(LeftHandPoint newLeftHandPoint, HeadPoint newHeadPoint)
        {
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            if (!IsLeftHandWave && newHeadPoint.X - newLeftHandPoint.X > 200)
            //left hand wave gesture(start = 0, raisehand = 1, rightside = 2, leftside = 3, putdown = 4)
            {
                IsLeftHandWave = true;
                _sensor.AllFramesReady -= _sensor_AllFramesReady;
                GC.Collect();
                (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.RelativeOrAbsolute);
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

    }//end of page
}// end of namespace kinect test

