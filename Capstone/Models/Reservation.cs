using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int SiteID { get; set; }  // Remove if unnecessary
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateReserved { get; set; }

        public override string ToString()
        {
            return ReservationID.ToString().PadRight(10) + SiteID.ToString().PadRight(10)
                + Name.ToString().PadRight(40) + StartDate.ToString().PadRight(20)
                + EndDate.ToString().PadRight(20) + DateReserved.ToString().PadRight(20);
        }
    }
}
