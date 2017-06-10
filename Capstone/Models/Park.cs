using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Park
    {
        public int ParkID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int Area { get; set; }
        public int AnnualVisitorCount { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Name.PadRight(20) + Location.PadRight(20)
                + EstablishedDate.ToShortDateString().PadRight(25) + Area.ToString().PadRight(20)
                + AnnualVisitorCount.ToString().PadRight(10);
        }
    }
}
