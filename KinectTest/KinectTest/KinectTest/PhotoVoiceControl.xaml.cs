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
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech;
using System.IO;
namespace KinectTest
{
    /// <summary>
    /// Interaction logic for PhotoVoice.xaml
    /// </summary>
    public partial class PhotoVoiceControl : Page
    {
         private static string RecogniserId = "SR_MS_en-US_Kinect_10.0";
        KinectSensor _sensor;
        private string photoName;
        private int photoCount = 1;
        private SpeechRecognitionEngine speechEngine;
        private int RotateCount=0;
        public PhotoVoiceControl()
        {
            InitializeComponent();
            _sensor = Generics.GlobalKinectSensorChooser.Kinect;
            
        }
        public PhotoVoiceControl(string a,KinectSensor w)
            : this()
        {
            this.photoName = a;
            this._sensor = w;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // get sensor
            foreach (var sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    _sensor = sensor;
                    break;
                }
            }

            if (_sensor != null)
            {
                try
                {
                    _sensor.Start();
                }
                catch (IOException)
                {
                    _sensor = null;
                }
            }

            // get recognizer
            RecognizerInfo ri = GetRecognizer();

            if (null != ri)
            {
               speechEngine = new SpeechRecognitionEngine(ri.Id);

               var directions = new Choices();
                directions.Add(new SemanticResultValue("forward", "FORWARD"));
                directions.Add(new SemanticResultValue("forwards", "FORWARD"));
                directions.Add(new SemanticResultValue("straight", "FORWARD"));
                directions.Add(new SemanticResultValue("next", "FORWARD"));
                directions.Add(new SemanticResultValue("go right", "FORWARD"));
                directions.Add(new SemanticResultValue("right", "FORWARD"));
                directions.Add(new SemanticResultValue("backward", "BACKWARD"));
                directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
                //directions.Add(new SemanticResultValue("back", "BACKWARD"));
                directions.Add(new SemanticResultValue("previous", "BACKWARD"));
                directions.Add(new SemanticResultValue("go left", "BACKWARD"));
                directions.Add(new SemanticResultValue("left", "BACKWARD"));
                directions.Add(new SemanticResultValue("rotate", "ROTATE"));
                directions.Add(new SemanticResultValue("hand", "HAND"));
                directions.Add(new SemanticResultValue("gesture", "HAND"));
                directions.Add(new SemanticResultValue("quit", "MAIN"));
               
               
                var gb = new GrammarBuilder { Culture = ri.Culture };
                gb.Append(directions);
               
                var g = new Grammar(gb);
                speechEngine.LoadGrammar(g);

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                ////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

                speechEngine.SetInputToAudioStream(
                    _sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
 
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //MessageBox.Show("What?Say again!");
           // throw new NotImplementedException();
        }
        

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {  
            if (e.Result.Confidence < 0.7)
            { return; }
            string semantic = "";
            switch ( e.Result.Semantics.Value.ToString() )
            {
                case "FORWARD":
                    if (photoCount <= 9)
                    {
                        photoCount += 1;
                        Label1.Content = photoCount.ToString() + "/10";
                        Image.Source = new BitmapImage(new Uri(@"F:\KinectTest\KinectTest\KinectTest\Photo\" + photoCount.ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                    }
                    break;
                case "BACKWARD":
                    if (photoCount >= 2)
                    {
                        photoCount -= 1;
                        Label1.Content = photoCount.ToString() + "/10";
                        Image.Source = new BitmapImage(new Uri(@"F:\KinectTest\KinectTest\KinectTest\Photo\" + photoCount.ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                    }
                    break;
                case "ROTATE":
                    RotateCount += 1;
                    //Rotate
                    //Create source
                    BitmapImage bi = new BitmapImage();
                    //BitmapImage properties must be in a BeginInit/EndInit block
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"F:\KinectTest\KinectTest\KinectTest\Photo\" + photoCount.ToString() + ".jpg", UriKind.RelativeOrAbsolute);
                    //Set image rotation
                    if (RotateCount % 4 == 1)
                    {
                        bi.Rotation = Rotation.Rotate90;
                    }
                    else if (RotateCount % 4 == 2)
                    {
                        bi.Rotation = Rotation.Rotate180;
                    }
                    else if (RotateCount % 4 == 3)
                    {
                        bi.Rotation = Rotation.Rotate270;
                    }
                    else if (RotateCount % 4 == 0)
                    {
                        bi.Rotation = Rotation.Rotate0;
                    }
                    bi.EndInit();
                    //set image source
                    Image.Source = bi;
                    break;
                case "HAND":
                            semantic = "hand";
                             if (null != this._sensor)
                            {
                                this._sensor.AudioSource.Stop();
                            }

                            if (null != this.speechEngine)
                            {
                                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                                this.speechEngine.RecognizeAsyncStop();
                            }
                            statusLabel.Content = "Loading...";
                            Photo newPhoto = new Photo("1");
                            this.NavigationService.Navigate(newPhoto);
                            break;
                case "MAIN":
                           if (null != this._sensor)
                        {
                            this._sensor.AudioSource.Stop();
                        }

                        if (null != this.speechEngine)
                        {
                            this.speechEngine.SpeechRecognized -= SpeechRecognized;
                            this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                            this.speechEngine.RecognizeAsyncStop();
                        }
                        (Application.Current.MainWindow.FindName("mainFrame") as Frame).Source = new Uri("MainMenu.xaml", UriKind.Relative);
                        break;
                default:
                    break;
            }

            

            //throw new NotImplementedException();
        }

        public static RecognizerInfo GetRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }
    }

    }


