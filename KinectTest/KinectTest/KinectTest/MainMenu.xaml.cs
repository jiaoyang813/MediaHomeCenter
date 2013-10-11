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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private KinectSensorChooser sensorChooser;
        
        public MainMenu()
        {
            InitializeComponent();

            // initialize the sensor chooser and UI
            if (Generics.GlobalKinectSensorChooser == null)
            {
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
                this.sensorChooser.Start();
                Generics.GlobalKinectSensorChooser = this.sensorChooser;
            }
            else
            {
                this.sensorChooser = new KinectSensorChooser();
                this.sensorChooser = Generics.GlobalKinectSensorChooser;
                this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser = sensorChooser; 
            }
           

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
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

        // Called when movie button clicked
        private void Movie1ButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
         //   (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("Movie.xaml", UriKind.Relative);

            Movie m = new Movie("movie");
            this.NavigationService.Navigate(m);
        }

        // Called when music button clicked
        private void Movie2ButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
            
            Song m = new Song("song");
            this.NavigationService.Navigate(m);
        }

        // Called when photo button clicked
        private void PhotoButtonOnClick(object sender, RoutedEventArgs e)
        {
            

            Photo m = new Photo("1",sensorChooser);
            this.NavigationService.Navigate(m);
        }
    }
}
