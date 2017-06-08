using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.Search;

namespace Capstone.DAL
{
    public class ReservationSqlDAL
    {
        public List<Reservation> GetAllUpcomingReservations()
        {
            throw new NotImplementedException();
        }

        public List<Reservation> GetAllUpcomingReservations(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public int  BookReservation(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public bool IsCampsiteOpen(BasicSearch bs)
        {
            throw new NotImplementedException();
        }
    }
}
