using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel _VMPanel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel vm = new ViewModel(new Panel(), new AnomalyData(new SimpleAnomalyDetector((float)0)));
            this._VMPanel = vm;
            //this._VMPanel.DataContext
            DataContext = _VMPanel;
            this.playback.Notify += OnPress;
            this.Graphs.Notify += OnPress;
        }
        public void OnPress(Object sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MediaEventArgs)))
            {
                MediaEventArgs m = (MediaEventArgs)e;
                this._VMPanel.PlaybackControl(m);
            }
            else if (e.GetType().Equals(typeof(GraphEventArgs)))
            {
                GraphEventArgs m = (GraphEventArgs)e;
                this._VMPanel.GraphControls(m);
            }
        }
        private void XmlButtonClick(object sender, RoutedEventArgs e)
        {
            _VMPanel.clickXML();

            CSVFileTrain.Visibility = Visibility.Visible;
        }
        private void CSVTarinFile_Click(object sender, RoutedEventArgs e)
        {
            if (_VMPanel.ClickCSVTrain())
            {
                CSVFileTrain.Content = "Upload CSV Train File";
                CSVFileTest.Visibility = Visibility.Visible;
                XMLFile.Visibility = Visibility.Hidden;
            }
        }
        private void CSVFileTest_Click(object sender, RoutedEventArgs e)
        {
            if (_VMPanel.ClickCSVTest())
            {
                this.Graphs.DataContext = _VMPanel;
                _VMPanel.LearnProcess();
                Graphs.columnsName.SelectedItem = "aileron";

                CSVFileTrain.Visibility = Visibility.Hidden;
                GraphsTab.Visibility = Visibility.Visible;
            }
        }
        private void exit_Click(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Really close?", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            _VMPanel.CloseALL();
        }
    }
}
