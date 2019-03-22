using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Bhaptics.Tact;

namespace SampleApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private HapticPlayer hapticPlayer;
        public MainWindow()
        {
            InitializeComponent();

            hapticPlayer = new HapticPlayer();

            var file = File.ReadAllText("ElectricFront.tact");
            var project = CommonUtils.ConvertJsonStringToTactosyFile(file).Project;
            hapticPlayer.Register("file", project);
            
        }

        private void PathClicked(object sender, RoutedEventArgs e)
        {
            hapticPlayer.Submit("path1", PositionType.VestFront, new PathPoint(0.5f, 0.5f, 100), 1000);
            hapticPlayer.Submit("path2", PositionType.ForearmL, new PathPoint(0.5f, 0.5f, 100), 1000);
            hapticPlayer.Submit("path3", PositionType.ForearmR, new PathPoint(0.5f, 0.5f, 100), 1000);
            hapticPlayer.Submit("path4", PositionType.VestBack, new PathPoint(0.5f, 0.5f, 100), 1000);
            hapticPlayer.Submit("path5", PositionType.Head, new PathPoint(0.5f, 0.5f, 100), 1000);
        }

        private void DotClicked(object sender, RoutedEventArgs e)
        {
            var list = new List<DotPoint>()
            {
                new DotPoint(1, 100),
                new DotPoint(5, 30)
            };

            hapticPlayer.Submit("dot1", PositionType.VestFront, list, 1000);
            hapticPlayer.Submit("dot2", PositionType.ForearmL, list, 1000);
            hapticPlayer.Submit("dot3", PositionType.ForearmR, list, 1000);
            hapticPlayer.Submit("dot4", PositionType.VestBack, list, 1000);
            hapticPlayer.Submit("dot5", PositionType.Head, list, 1000);
        }

        private void FileClicked(object sender, RoutedEventArgs e)
        {
            hapticPlayer.SubmitRegistered("file");
        }
    }
}
