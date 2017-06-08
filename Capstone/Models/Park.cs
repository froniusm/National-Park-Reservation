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
            return ParkID.ToString().PadRight(10) + Name.PadRight(40) + Location.PadRight(40)
                + EstablishedDate.ToString().PadRight(20) + Area.ToString().PadRight(20)
                + AnnualVisitorCount.ToString().PadRight(20) + "/n/t" + Description;
        }
    }
}
