using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// This class is necessary because
    /// the historyControl class inherits
    /// from Usercontrol which is not
    /// serializable.
    /// </summary>
    [Serializable]
    public class HistorySerializable
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public HistoricalDate StartDate { get; set; }
        public HistoricalDate EndDate { get; set; }
        public Color Color { get; set; }
        public Bitmap? Image { get; set; }
        public List<HistorySerializable> Children { get; set; }

        public HistorySerializable(HistoryControl historyControl)
        {
            Title = historyControl.Title;
            Description = historyControl.Description;
            StartDate = historyControl.StartDate;
            EndDate = historyControl.EndDate;
            Color = Converter.ConvertBrushToColor(historyControl.Brush);
            Image = Converter.ConvertImageSourceToBitmap(historyControl.Image);

            Children = new List<HistorySerializable>();
            foreach (HistoryControl child in historyControl.GridTimeLine.Children)
            {
                Children.Add(new HistorySerializable(child));
            }
        }

        public override string ToString()
        {
            return $"{Title} \n {Description} \n {StartDate} \n {EndDate} \n {Color}";
        }
    }
}
