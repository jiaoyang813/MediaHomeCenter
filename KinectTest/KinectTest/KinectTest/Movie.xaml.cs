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
        private string movieName;
        
        public Movie()
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
            Player newplayer = new Player("Frozen",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton2_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("Gravity",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton3_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("JackRyan",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton4_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("Linsanity",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton5_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("Rio2",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton8_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("EndersGame",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton7_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("Vikingdom",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void movieButton6_Click(object sender, RoutedEventArgs e)
        {
            Player newplayer = new Player("TheHobbit",sensorChooser);
            this.NavigationService.Navigate(newplayer);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
