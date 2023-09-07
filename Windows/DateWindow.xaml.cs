using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// Interaction logic for HistoricalDateWindow.xaml
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    public partial class DateWindow : Window
    {
        Button dateButton;

        public DateWindow(Button dateButton)
        {
            InitializeComponent();
            this.dateButton = dateButton;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            HistoricalDate? startDate = GetStartDate();
            HistoricalDate? endDate = GetEndDate();

            if (startDate != null && endDate != null)
            {
                dateButton.Tag = (startDate, endDate);

                if (endDate.ToDouble() < startDate.ToDouble())
                {
                    MessageBox.Show("End date has to be after the start date");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Check input");
                return;
            }

            dateButton.Content = DateToString();
            DialogResult = true;
        }

        /// <summary>
        /// Returns a string that represents both
        /// the start and end dates
        /// </summary>
        private string DateToString()
        {
            HistoricalDate? startDate = GetStartDate();
            HistoricalDate? endDate = GetEndDate();

            if (startDate == null || endDate == null)
                return "No date";
            else
                return $"{startDate} - {endDate}";
        }

        /// <summary>
        /// Method for creating and returning a
        /// new historical date from the start date
        /// input controls
        /// </summary>
        private HistoricalDate? GetStartDate()
        {
            bool success = true;

            int year;
            int month;
            int day;
            Era era;

            if (!int.TryParse(TextBoxStartYear.Text, out year))
                success = false;

            if (!int.TryParse(TextBoxStartMonth.Text, out month))
                success = false;

            if (!int.TryParse(TextBoxStartDay.Text, out day))
                success = false;

            era = (Era)ComboBoxStartEra.SelectedIndex;

            if (success)
            {
                return new HistoricalDate(year, month, day, era);
            }
            else
                return null;
        }

        /// <summary>
        /// Method for creating and returning a
        /// new historical date from the end date
        /// input controls
        /// </summary>
        private HistoricalDate? GetEndDate()
        {
            bool success = true;

            int year;
            int month;
            int day;
            Era era;

            if (!int.TryParse(TextBoxEndYear.Text, out year))
                success = false;

            if (!int.TryParse(TextBoxEndMonth.Text, out month))
                success = false;

            if (!int.TryParse(TextBoxEndDay.Text, out day))
                success = false;

            era = (Era)ComboBoxEndEra.SelectedIndex;

            if (success)
            {
                return new HistoricalDate(year, month, day, era);
            }
            else
                return null;
        }
    }
}
