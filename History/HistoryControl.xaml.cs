using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
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

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// This class is not serializable. Therefore, a
    /// seperate class called HistorySerializable
    /// is used for saving.
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public string Title { get; set; }
        public string Description { get; set; } //Optional
        public HistoricalDate StartDate { get; set; }
        public HistoricalDate EndDate { get; set; }
        public Brush Brush { get; set; }
        public ImageSource? Image { get; set; } //Optional

        private List<HistorySerializable> serializedChildren = new List<HistorySerializable>();

        /// <summary>
        /// Constructor for creating a new HistoryControl
        /// with a title, description etc
        /// </summary>
        public HistoryControl(string title, string description, HistoricalDate startDate, HistoricalDate endDate, Brush brush, ImageSource? image)
        {
            InitializeComponent();

            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Brush = brush;
            Image = image;
        }

        /// <summary>
        /// Constructor for reconstructing a HistoryControl from
        /// a HistorySerializable object
        /// </summary>
        public HistoryControl(HistorySerializable history) : 
            this(history.Title, history.Description, history.StartDate, history.EndDate, Converter.ConvertColorToBrush(history.Color), Converter.ConvertBitmapToImageSource(history.Image))
        {
            serializedChildren = history.Children;
        }

        /// <summary>
        /// Method for updating Labels and background
        /// </summary>
        public void UpdateGUI()
        {
            //Update controls
            BorderTimeLine.Background = Brush;
            LabelTitle.Content = Title;
            LabelStartDate.Content = StartDate.YearToString();
            LabelEndDate.Content = EndDate.YearToString();
            LabelMidDate.Content = CalculateMidDate(StartDate.ToDouble(), EndDate.ToDouble()).YearToString();

            UpdateGUIForChildren();
            GetParentHistoryControl()?.UpdateGUIForChildren();
            UpdateLabelVisibility();
            UpdateContextMenu();
            UpdateToolTip();
        }

        /// <summary>
        /// This method excecutes when user control has
        /// loaded in.
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int scaleFactor = 2;
            int margin = 10;

            HistoryControl? parent = GetParentHistoryControl();

            //Increase scale and margins of labels margin if historyControl is root timeLine
            if (parent == null)
            {
                LabelTitle.Margin = new Thickness(0, 0, 0, margin);
                LabelStartDate.Margin = new Thickness(0, margin, 0, 0);
                LabelEndDate.Margin = new Thickness(0, margin, 0, 0);
                LabelMidDate.Margin = new Thickness(0, margin, 0, 0);

                LabelTitle.FontSize = (int)LabelTitle.FontSize * scaleFactor;
                LabelStartDate.FontSize = (int)LabelStartDate.FontSize * scaleFactor;
                LabelEndDate.FontSize = (int)LabelEndDate.FontSize * scaleFactor;
                LabelMidDate.FontSize = (int)LabelMidDate.FontSize * scaleFactor;
            }

            //Add serialized children
            foreach (HistorySerializable historyItem in serializedChildren)
            {
                GridTimeLine.Children.Add(new HistoryControl(historyItem));
            }

            UpdateGUI();
        }

        /// <summary>
        /// Method for editing this historyControl
        /// </summary>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow eventWindow = new HistoryWindow(this);
            eventWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Show eventwindow
            if (eventWindow.ShowDialog() == true)
            {
                HistoryControl? historyControl = eventWindow.GetHistoryControl();
                if (historyControl != null)
                {
                    Title = historyControl.Title;
                    Description = historyControl.Description;
                    StartDate = historyControl.StartDate;
                    EndDate = historyControl.EndDate;
                    Brush = historyControl.Brush;
                    Image = historyControl.Image;

                    UpdateGUI();
                }
            }
        }

        /// <summary>
        /// Method for deleting this historyControl
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                $"You are about to delete {Title}.",
                "Are you sure?",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (Parent.GetType() == typeof(ScrollViewer))
            {
                ((ScrollViewer)Parent).Content = null;
                return;
            }

            Grid? parentGrid = GetParentGrid();
            if (parentGrid != null)
            {
                parentGrid.Children.Remove(this);
                GetParentHistoryControl()?.UpdateGUIForChildren();
            }
        }

        /// <summary>
        /// Method for adding history to historyControl
        /// </summary>
        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            //If borderTimeLine is to small for adding history
            if (BorderTimeLine.ActualWidth < 50)
            {
                Console.WriteLine($"{Title} can't fit history inside it");
                return;
            }

            //Create a new historyWindow
            HistoryWindow window = new HistoryWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Show historyWindow
            if (window.ShowDialog() == true)
            {
                //Get historycontrol from historywindow
                HistoryControl? historyControl = window.GetHistoryControl();

                //Return if there is no historycontrol
                if (historyControl == null)
                    return;

                //Set verticalalignment to top
                historyControl.VerticalAlignment = VerticalAlignment.Top;

                //Add historycontrol to children
                GridTimeLine.Children.Add(historyControl);

                UpdateGUIForChildren();
            }
        }

        /// <summary>
        /// Method that sets margin and row for each
        /// child in GridTimeLine.Children
        /// </summary>
        public void UpdateGUIForChildren()
        {
            foreach (HistoryControl child in GridTimeLine.Children)
            {
                //Begin with row index 0
                child.SetValue(Grid.RowProperty, 0);

                //Calculate margins for child
                child.CalculateMargin();

                //Set row for child
                if (child.IsOverlapping())
                    child.RowMove(LogicalDirection.Forward, true);

                //Set GUI positions for childs children
                child.UpdateGUIForChildren();
            }
        }

        /// <summary>
        /// Method for moving historyControl either up
        /// or down.
        /// </summary>
        /// <param name="direction">Defines which direction to move</param>
        /// <param name="forceMove">If true, will continue until a row is found</param>
        private void RowMove(LogicalDirection direction, bool forceMove)
        {
            //Get current row
            int currentRowIndex = Grid.GetRow(this);

            //Get the reference to the parent grid
            Grid? parentGrid = GetParentGrid();

            //Don't continue if there is no parent grid
            if (parentGrid == null)
                return;

            if (direction == LogicalDirection.Forward)
            {
                //Create a new row if there isn't one above in the grid
                if (parentGrid.RowDefinitions.Count < currentRowIndex + 2)
                    parentGrid.RowDefinitions.Add(new RowDefinition());

                //Set new row value
                SetValue(Grid.RowProperty, currentRowIndex + 1);

                //If is still overlapping in the new row
                if (forceMove && IsOverlapping())
                    RowMove(LogicalDirection.Forward, forceMove);
                else if (!forceMove && IsOverlapping())
                    RowMove(LogicalDirection.Backward, false);
            }
            else if (direction == LogicalDirection.Backward)
            {
                //Throw exception if trying to force move up
                if (forceMove)
                    throw new InvalidOperationException("Can't force move up/backwards");

                //If already at the top row, show message and return
                if (currentRowIndex == 0)
                {
                    MessageBox.Show($"Can't move {Title} up!");
                    return;
                }

                //Set new row value
                SetValue(Grid.RowProperty, currentRowIndex + 1);

                //If is still overlapping in the new row
                if (IsOverlapping())
                    RowMove(LogicalDirection.Forward, false);
            }
        }

        /// <summary>
        /// Method for finding out if historyControl is overlapping
        /// with others in same row
        /// </summary>
        /// <returns>True if overlapping</returns>
        public bool IsOverlapping()
        {
            HistoryControl[] others = GetOthersInSameRow();

            foreach (HistoryControl other in others)
            {
                if (IsInRange(this.StartDate.ToDouble(), other.StartDate.ToDouble(), other.EndDate.ToDouble()) ||
                    IsInRange(this.EndDate.ToDouble(), other.StartDate.ToDouble(), other.EndDate.ToDouble()) ||
                    this.StartDate.ToDouble() == other.StartDate.ToDouble() || this.EndDate.ToDouble() == other.EndDate.ToDouble())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Method for finding out if a number is
        /// in a range
        /// </summary>
        /// <returns>True if number is in range</returns>
        public static bool IsInRange(double number, double rangeStart, double rangeEnd)
        {
            if (rangeStart < number && number < rangeEnd)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method for retrieving others from
        /// same row
        /// </summary>
        /// <returns>An array of historycontrols from the same row</returns>
        public HistoryControl[] GetOthersInSameRow()
        {
            List<HistoryControl> others = new List<HistoryControl>();

            Grid? parentGrid = GetParentGrid();

            if (parentGrid == null)
                return new HistoryControl[0];

            foreach (HistoryControl other in parentGrid.Children)
            {
                if (ReferenceEquals(other, this))
                    continue;

                if (Grid.GetRow(other) == Grid.GetRow(this))
                    others.Add(other);
            }

            return others.ToArray();
        }

        /// <summary>
        /// Method for calculating margin of historycontrol
        /// </summary>
        private void CalculateMargin()
        {
            HistoryControl? parent = GetParentHistoryControl();

            if (parent == null)
                return;

            double baseMargin = 2;

            //Calculate left and right margin in years
            double timeLineSpan = parent.EndDate.ToDouble() - parent.StartDate.ToDouble();
            double leftMargin = this.StartDate.ToDouble() - parent.StartDate.ToDouble();
            double rightMargin = timeLineSpan - (this.EndDate.ToDouble() - parent.StartDate.ToDouble());

            //Convert margin in years into margins in pixels
            double parentPixelWidth = parent.BorderTimeLine.ActualWidth;
            double pixelAdjust = parentPixelWidth / timeLineSpan;

            leftMargin *= pixelAdjust;
            rightMargin *= pixelAdjust;

            double pixelWidthResult = parentPixelWidth - leftMargin - rightMargin;
            double minPixelWidth = 20;
            if (pixelWidthResult < minPixelWidth)
            {
                leftMargin -= minPixelWidth / 2;
                rightMargin -= minPixelWidth / 2;
            }

            Margin = new Thickness(
                baseMargin + leftMargin, //Left
                baseMargin + 1, //Top
                rightMargin, //Right
                baseMargin //Bottom
                );

            UpdateGUIForChildren();
        }

        /// <summary>
        /// This returns the first parent that is a history
        /// control. This method is necessary since the first
        /// parent way be a grid or some other control.
        /// </summary>
        /// <returns>The historyControl parent</returns>
        public HistoryControl? GetParentHistoryControl()
        {
            //Get parent as dependency object
            DependencyObject parent = Parent;

            //Try to find a parent that is a historyControl
            try
            {
                while (!(parent is HistoryControl))
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }

                return (HistoryControl)parent;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This returns the first parent that is a grid
        /// This method is necessary since the first parent
        /// may be of some other type.
        /// </summary>
        /// <returns>The grid parent</returns>
        private Grid? GetParentGrid()
        {
            //Get parent as dependency object
            DependencyObject parent = Parent;

            //Try to find a parent that is a grid
            try
            {
                while (!(parent is Grid))
                {
                    parent = LogicalTreeHelper.GetParent(parent);
                }

                return (Grid)parent;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method for calculating the mid date
        /// </summary>
        /// <returns>HistoricalDate that is the middle date</returns>
        public static HistoricalDate CalculateMidDate(double startDate, double endDate)
        {
            double range = endDate - startDate;
            double mid = startDate + range / 2;

            return new HistoricalDate((int)mid);
        }

        /// <summary>
        /// Method for configuring the context menu
        /// </summary>
        private void UpdateContextMenu()
        {
            var menuItemOpen = new MenuItem();
            menuItemOpen.Header = "Open";
            menuItemOpen.Click += Edit_Click;

            var menuItemDelete = new MenuItem();
            menuItemDelete.Header = "Delete";
            menuItemDelete.Click += Delete_Click;

            var menuItemAddHistory = new MenuItem();
            menuItemAddHistory.Header = "Add History";
            menuItemAddHistory.Click += AddHistory_Click;

            BorderTimeLine.ContextMenu = new ContextMenu();
            BorderTimeLine.ContextMenu.Style = FindResource("RoundContextMenu") as Style;
            BorderTimeLine.ContextMenu.Items.Add(menuItemOpen);
            BorderTimeLine.ContextMenu.Items.Add(menuItemDelete);
            BorderTimeLine.ContextMenu.Items.Add(new Separator());
            BorderTimeLine.ContextMenu.Items.Add(menuItemAddHistory);
        }

        /// <summary>
        /// This method excecutes when the timeLine changes
        /// in size.
        /// </summary>
        private void BorderTimeLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGUI();
        }

        /// <summary>
        /// Method for updating the visibility for
        /// labels.
        /// </summary>
        private void UpdateLabelVisibility()
        {
            double titleWidth = 20;
            double datesWidth = 54;

            Console.WriteLine($"{Title} start date width: {LabelStartDate.ActualWidth}");

            if (titleWidth > BorderTimeLine.ActualWidth)
            {
                LabelStartDate.Visibility = Visibility.Collapsed;
                LabelTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                LabelStartDate.Visibility = Visibility.Visible;
                LabelTitle.Visibility = Visibility.Visible;
            }

            if (datesWidth * 2 > BorderTimeLine.ActualWidth)
            {
                LabelEndDate.Visibility = Visibility.Collapsed;
            }
            else
            {
                LabelEndDate.Visibility = Visibility.Visible;
            }

            if (datesWidth * 3 > BorderTimeLine.ActualWidth)
            {
                
                LabelMidDate.Visibility = Visibility.Collapsed;
            }
            else
            {
                LabelMidDate.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Method for configuriing tool tip
        /// </summary>
        private void UpdateToolTip()
        {
            StackPanel panel = new StackPanel();
            panel.Children.Add(new Label() { Content = Title, FontWeight = FontWeights.Bold });
            panel.Children.Add(new Label() { Content = $"{StartDate} - {EndDate}", FontStyle = FontStyles.Italic });
            BorderTimeLine.ToolTip = new ToolTip() { Content = panel };
        }
    }
}
