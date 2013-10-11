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
    public partial class Song : Page
    {
        private KinectSensorChooser sensorChooser;
        private string songName;

        public Song()
        {
            InitializeComponent();
            
            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser = Generics.GlobalKinectSensorChooser;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

         /*   //fill scroll content 
            for (int i = 1; i < 20; i++)
            {
                var button = new KinectTileButton
                {
                    Content = i,
                    Width = 100,
                    Height = 120
                };

                int i1 = i;
                //button.Click += (o, args) => label.Content = "Button " + i1;
                scrollContent.Children.Add(button);
            }*/
        }

        public Song(string s) : this()
        {
            this.songName = s;
            
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer=new SongPlayer("GangnamStyle",sensorChooser);
            this.NavigationService.Navigate(newplayer);
            //this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
           // (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative); 
        }

        private void songButton1_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("CruiseFloridaGeogiaLine",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void songButton2_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("Drake",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }
        private void songButton3_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("GangnamStyle",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }
        private void songButton4_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("HOLYGRAIL",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void songButton5_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("Royals",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void songButton6_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("WakeMeUp",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void songButton7_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("WalkingOnAir",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void songButton8_Click(object sender, RoutedEventArgs e)
        {
            SongPlayer newplayer = new SongPlayer("WreckingBall",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        

        
    }
}
