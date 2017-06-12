using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Capstone.Models
{
    public class Campground
    {
        public int CampgroundID { get; set; }
        public int ParkID { get; set; }
        public string Name { get; set; }
        public int OpenMonth { get; set; }
        public int CloseMonth { get; set; }
        public decimal DailyFee { get; set; }

        public override string ToString()
        {
            string openMonthLongName = new DateTimeFormatInfo().GetMonthName(OpenMonth);
            string closeMonthLongName = new DateTimeFormatInfo().GetMonthName(CloseMonth);

            return Name.PadRight(35) + 
                String.Concat(openMonthLongName, " - ", closeMonthLongName).PadRight(30)
                + DailyFee.ToString("C");
        }
    }
}
