using System;
using System.Windows;
using System.ComponentModel;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ViewModel _vmPanel;

        public MainWindow()
        {
            InitializeComponent();
            var vm = new ViewModel(new Panel(), new AnomalyData(new SimpleAnomalyDetector(0)));
            _vmPanel = vm;
            DataContext = _vmPanel;
            Playback.Notify += OnPress;
            Graphs.Notify += OnPress;
        }

        private void OnPress(object sender, EventArgs e)
        {
            if (e.GetType() == typeof(MediaEventArgs))
            {
                var m = (MediaEventArgs) e;
                _vmPanel.PlaybackControl(m);
            }
            else if (e.GetType() == typeof(GraphEventArgs))
            {
                var m = (GraphEventArgs) e;
                _vmPanel.GraphControls(m);
            }
        }

        private void XmlButtonClick(object sender, RoutedEventArgs e)
        {
            _vmPanel.ClickXml();

            CsvFileTrain.Visibility = Visibility.Visible;
        }

        private void CSVTrainFile_Click(object sender, RoutedEventArgs e)
        {
            if (!_vmPanel.ClickCsvTrain()) return;
            CsvFileTrain.Content = "Upload CSV Train File";
            CsvFileTest.Visibility = Visibility.Visible;
            XmlFile.Visibility = Visibility.Hidden;
        }

        private void CSVFileTest_Click(object sender, RoutedEventArgs e)
        {
            if (!_vmPanel.ClickCsvTest()) return;
            Graphs.DataContext = _vmPanel;
            _vmPanel.LearnProcess();
            Graphs.ColumnsName.SelectedItem = "aileron";

            CsvFileTrain.Visibility = Visibility.Hidden;
            GraphsTab.Visibility = Visibility.Visible;
        }

        private void exit_Click(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Really close?", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }

            _vmPanel.CloseAll();
        }
    }
}