using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bhaptics.Tact;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WebSocketSender WebSocketSender;
        private HapticPlayer HapticPlayer;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (sender, args) =>
            {
                HapticPlayer = new HapticPlayer();
                HapticPlayer.StatusReceived += response =>
                {
                    Debug.WriteLine("response " + response.ActiveKeys.Count);
                };

                WebSocketSender = new WebSocketSender();
                WebSocketSender.Initialize(true);

                try
                {
                    var assembly = this.GetType().GetTypeInfo().Assembly;
                    var resource = assembly.GetManifestResourceStream("App1.BowShoot.tact");
                    StreamReader reader = new StreamReader(resource);
                    string text = reader.ReadToEnd();
                    
                    var project = HapticFeedbackFile.ToHapticFeedbackFile(text).Project;
                    HapticPlayer.Register("test", project);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            };
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            HapticPlayer.Submit(
                "test2",
                PositionType.ForearmL,
                new List<DotPoint>()
                {
                    new DotPoint(0, 100)
                }, 1000);
        }

        private void FileClicked(object sender, RoutedEventArgs e)
        {
            //WebSocketSender.SubmitRegistered("test");

            HapticPlayer.SubmitRegistered("test", "test", 
                new ScaleOption(2f, 4f));
        }
    }
}
