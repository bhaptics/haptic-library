using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Bhaptics.Tact;

namespace TestApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private IHapticPlayer _player;

        private string[] arr = { "Swirl.tact" ,
            "Vehicle.tact", "WaistDeep.tact", "RifleImpact.tact"};

        public MainWindow()
        {
            InitializeComponent();
            _player = new HapticPlayer();


            string sssss = "[\"Head\"]";

            var jsonNode = JSON.Parse(sssss);

            foreach (var keyValuePair in jsonNode.AsArray)
            {
                Debug.WriteLine(keyValuePair);

                var positionType = EnumParser.ToPositionType(keyValuePair.Value);

                Debug.WriteLine(positionType);
            }


            foreach (var s in arr)
            {
                try
                {
                    var readAllText = File.ReadAllText(s);

                    var hapticFeedbackFile = CommonUtils.ConvertJsonStringToTactosyFile(readAllText);
                    _player.Register(s, hapticFeedbackFile.Project);

                    var button = new Button();
                    button.Content = s;
                    button.Click += (sender, args) =>
                    {
                        _player.SubmitRegisteredVestRotation(s, s, new RotationOption(0, 0), new ScaleOption(1, 1));
                    };
                    ButtonContainer.Children.Add(button);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + " " + e.StackTrace);
                }

            }

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
//            _player.Submit("VestFront", PositionType.VestFront, new DotPoint(0, 100), 2000);
//            _player.Submit("VestBack", PositionType.VestBack, new DotPoint(0, 100), 2000);
            _player.SubmitRegisteredVestRotation("Swirl", "Swirl",  new RotationOption(0, 0), new ScaleOption(0.5f, 1));
            
        }
    }
}
