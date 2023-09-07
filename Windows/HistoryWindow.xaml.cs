using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    public partial class HistoryWindow : Window
    {
        //Constructor for creating new history
        public HistoryWindow()
        {
            InitializeComponent();

            //Make sure control contents are empty
            TextBoxTitle.Text = string.Empty;
            ButtonDate.Content = "Set date";
            TextBoxDescription.Text = string.Empty;
        }

        //Constructor for displaying given historyControl
        public HistoryWindow(HistoryControl historyControl) : this()
        {
            TextBoxTitle.Text = historyControl.Title;
            ButtonDate.Tag = (historyControl.StartDate, historyControl.EndDate);
            ButtonDate.Content = $"{historyControl.StartDate} - {historyControl.EndDate}";
            TextBoxDescription.Text = historyControl.Description;
            ColorExpander.Background = historyControl.Brush;
            ImageHistory.Source = historyControl.Image;
        }

        /// <summary>
        /// Method for showing a new date window
        /// </summary>
        private void ButtonDate_Click(object sender, RoutedEventArgs e)
        {
            DateWindow dateWindow = new DateWindow(ButtonDate);

            if (ButtonDate.Tag != null)
            {
                (HistoricalDate startDate, HistoricalDate endDate) = (ValueTuple<HistoricalDate, HistoricalDate>)ButtonDate.Tag;

                dateWindow.TextBoxStartDay.Text = startDate.Day.ToString();
                dateWindow.TextBoxStartMonth.Text = startDate.Month.ToString();
                dateWindow.TextBoxStartYear.Text = startDate.Year.ToString();
                dateWindow.ComboBoxStartEra.SelectedIndex = (int)startDate.Era;

                dateWindow.TextBoxEndDay.Text = endDate.Day.ToString();
                dateWindow.TextBoxEndMonth.Text = endDate.Month.ToString();
                dateWindow.TextBoxEndYear.Text = endDate.Year.ToString();
                dateWindow.ComboBoxEndEra.SelectedIndex = (int)endDate.Era;
            }

            dateWindow.Owner = this;
            dateWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            dateWindow.ShowDialog();
        }

        /// <summary>
        /// Method for collapsing the color expander
        /// when a color is chosen
        /// </summary>
        private void ButtonColorExpander_Click(object sender, RoutedEventArgs e)
        {
            //Hide color expander
            ColorExpander.IsExpanded = false;

            //Set color expander background to chosen color
            ColorExpander.Background = ((Button)sender).Background;
        }

        /// <summary>
        /// Method for adding an image to history
        /// </summary>
        private void ButtonAddImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageHistory.Source != null)
            {
                if (MessageBox.Show("This will replace current image.", "Are you sure?", MessageBoxButton.YesNo) 
                    == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Jpeg|*.jpg",
                Title = "Select Image",
                InitialDirectory = Environment.CurrentDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.EndInit();

                    ImageHistory.Source = bitmap;

                    LabelImage.Visibility = Visibility.Hidden;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Method for removing image from history
        /// </summary>
        private void ButtonRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ImageHistory.Source != null)
            {
                if (MessageBox.Show("The image will be removed.", "Are you sure?", MessageBoxButton.YesNo)
                    == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("There is no image to remove!");
                return;
            }

            ImageHistory.Source = null;
            LabelImage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Method for moving around window with mouse click
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Method for confirming changes
        /// </summary>
        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!InputOK())
                return;

            DialogResult = true;
        }

        /// <summary>
        /// Method for checking input
        /// </summary>
        private bool InputOK()
        {
            if (string.IsNullOrEmpty(TextBoxTitle.Text))
            {
                MessageBox.Show("Title invalid!");
                return false;
            }

            if (ButtonDate.Tag == null || ButtonDate.Tag.GetType() != typeof(ValueTuple<HistoricalDate, HistoricalDate>))
            {
                MessageBox.Show("Date invalid!");
                return false;
            }

            if (ColorExpander.Background == null)
            {
                MessageBox.Show("Color invalid!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method for returning a historyControl
        /// Might cause exception when converting
        /// several values. If exception occurs,
        /// return null.
        /// </summary>
        public HistoryControl? GetHistoryControl()
        {
            try
            {
                (HistoricalDate startDate, HistoricalDate endDate) = (ValueTuple<HistoricalDate, HistoricalDate>)ButtonDate.Tag;

                string name = TextBoxTitle.Text;
                string description = TextBoxDescription.Text;
                Brush color = ColorExpander.Background;
                ImageSource? image = ImageHistory.Source;

                return new HistoryControl(
                    name,
                    description,
                    startDate,
                    endDate,
                    color,
                    image
                    );
            }
            catch
            {
                return null;
            }
        }
    }
}
