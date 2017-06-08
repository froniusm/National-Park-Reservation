using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Campsite
    {
        public int SiteID { get; set; }
        public int CampgroundID { get; set; }  // Remove if unnecessary
        public int SiteNumber { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsAccessible { get; set; }
        public int MaxRVLength { get; set; }
        public bool HasUtilities { get; set; }

        public override string ToString()
        {
            string accessibleYesNo = IsAccessible ? "Yes" : "No";
            string utilitiesYesNo = HasUtilities ? "Yes" : "No";

            return SiteID.ToString().PadRight(10) + CampgroundID.ToString().PadRight(10)
                + SiteNumber.ToString().PadRight(10) + MaxOccupancy.ToString().PadRight(10) 
                + accessibleYesNo.PadRight(10) + MaxRVLength.ToString().PadRight(10)
                + utilitiesYesNo.PadRight(10);
        }
    }
}
