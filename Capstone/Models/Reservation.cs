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
        public int SiteID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateReserved { get; set; }

        public override string ToString()
        {
            return Name.ToString().PadRight(40) + StartDate.ToShortDateString().PadRight(15)
                + EndDate.ToShortDateString().PadRight(15) + DateReserved.ToShortDateString();
        }
    }
}
