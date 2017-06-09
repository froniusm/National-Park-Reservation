using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Campground
    {
        public int CampgroundID { get; set; }
        public int ParkID { get; set; }  // Remove if unnecessary
        public string Name { get; set; }
        public int OpenMonth { get; set; }
        public int CloseMonth { get; set; }
        public decimal DailyFee { get; set; }

        public override string ToString()
        {
            return Name.PadRight(30) + OpenMonth.ToString().PadRight(20)
                + CloseMonth.ToString().PadRight(20) + DailyFee.ToString("C");
        }
    }
}
