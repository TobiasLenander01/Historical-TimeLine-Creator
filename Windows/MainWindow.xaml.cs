using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button event method for adding a new timeline
        /// </summary>
        private void buttonNewTimeLine_Click(object sender, RoutedEventArgs e)
        {
            //Show confirmation message box if there is already a timeline
            if (ScrollViewerTimeLine.Content != null)
            {
                if (MessageBox.Show("Current timeline will be overwritten.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            //Create a new eventWindow for creating a new timeLine
            HistoryWindow eventWindow = new HistoryWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            //Show eventwindow
            if (eventWindow.ShowDialog() == true)
            {
                HistoryControl? timeLine = eventWindow.GetHistoryControl();

                //If a timeline was created, display in content control
                if (timeLine != null)
                {
                    AddTimeLine(timeLine);
                }
            }
        }

        /// <summary>
        /// Method for adding given timeline to scrollviewer
        /// </summary>
        private void AddTimeLine(HistoryControl timeLine)
        {
            timeLine.VerticalAlignment = VerticalAlignment.Center;
            ScrollViewerTimeLine.Content = timeLine;

            //Make instructions visible
            LabelInstructions.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Button for closing the program
        /// </summary>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (ScrollViewerTimeLine.Content != null)
            {
                if (MessageBox.Show("Make sure to save timeline before exiting!", "Are you sure you want to exit?", MessageBoxButton.YesNo) 
                    == MessageBoxResult.No)
                {
                    return;
                }
            }

            Close();
        }

        /// <summary>
        /// Method for dragging around window with mouse click
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Method for saving timeline
        /// </summary>
        private void ButtonSaveTimeLine_Click(object sender, RoutedEventArgs e)
        {
            if (ScrollViewerTimeLine.Content == null)
            {
                MessageBox.Show("There is no timeline to save!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Binary|*.bin",
                Title = "Save Time Line",
                FileName = "TimeLine",
                InitialDirectory = Environment.CurrentDirectory
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    HistoryControl historyControl = (HistoryControl)ScrollViewerTimeLine.Content;
                    HistorySerializable historySerializable = new HistorySerializable(historyControl);
                    Serializor.SaveHistoryAsBinary(historySerializable, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Method for loading timeline
        /// </summary>
        private void ButtonLoadTimeLine_Click(object sender, RoutedEventArgs e)
        {
            //Show confirmation message box if there is already a timeline
            if (ScrollViewerTimeLine.Content != null)
            {
                if (MessageBox.Show("Current timeline will be overwritten.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Binary|*.bin",
                Title = "Open Time Line",
                InitialDirectory = Environment.CurrentDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    HistorySerializable history = Serializor.LoadHistoryFromBinary(openFileDialog.FileName);
                    AddTimeLine(new HistoryControl(history));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
