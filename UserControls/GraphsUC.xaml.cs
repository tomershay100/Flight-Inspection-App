using System;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.UserControls
{
    public partial class GraphsUc : IObservable
    {
        public GraphsUc()
        {
            InitializeComponent();
            //this.columnsName.SelectedItem = "aileron";
        }
        public event Update Notify;
        //upload DLL button.
        private void DLLFile_Click(object sender, RoutedEventArgs e)
        {
            Notify?.Invoke(this, new GraphEventArgs("ClickDLL"));
        }
        //the ListBox anomalies selection button
        private void AnomaliesPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string t = (string)e.AddedItems[0];
                Notify?.Invoke(this, new GraphEventArgs("SelectedAnomaly", t));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
