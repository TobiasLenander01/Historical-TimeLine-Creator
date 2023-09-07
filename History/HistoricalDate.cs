using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// This class represents a historical date.
    /// It includes functionality to manage dates
    /// before year 0 (before Christ). And a ToDouble
    /// method for comparing dates with each other.
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    [Serializable]
    public class HistoricalDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public Era Era { get; set; }

        public HistoricalDate(int year, int month, int day, Era era) : this(year, month, era)
        {
            Day = Math.Clamp(day, 1, DateTime.DaysInMonth(Year, Month));
        }

        public HistoricalDate(int year, int month, Era era) : this(year, era)
        {
            Month = Math.Clamp(month, 1, 12);
        }

        public HistoricalDate(int year, Era era)
        {
            Year = Math.Clamp(year, 1, 9999);
            Era = era;
        }

        public HistoricalDate(int date)
        {
            Year = Math.Abs(date);
            if (date < 0)
                Era = Era.BC;
            else
                Era = Era.AD;
        }

        /// <summary>
        /// Method for returning the whole 
        /// historical date as a string
        /// </summary>
        public override string ToString()
        {
            return $"{Day}/{Month}/{Year} {Era}";
        }

        /// <summary>
        /// Method for returning only the 
        /// year and era as a string
        /// </summary>
        public string YearToString()
        {
            return $"{Year} {Era}";
        }

        /// <summary>
        /// Method for returning date as a 
        /// double for easy comparisons
        /// </summary>
        public double ToDouble()
        {
            int daysInMonth = DateTime.DaysInMonth(Year, Month);
            int days = daysInMonth * Month + Day;

            double monthFraction = (double)days / 365;

            if (Era == Era.BC)
            {
                return -(Year + monthFraction);
            }
            else
            {
                return Year + monthFraction;
            }
        }
    }
}
