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
        private string databaseConnection;

        public ReservationSqlDAL(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Reservation> GetUpcomingReservations(DateTime startDate, DateTime endDate)
        {
            List<Reservation> reservations = new List<Reservation>();
            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    string sqlQuery = $"SELECT * FROM reservation WHERE from_date " +
                        $"BETWEEN '{startDate}' AND '{endDate}' AND to_date " +
                        $"BETWEEN '{startDate}' AND '{endDate}';";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reservations.Add(PopulateReservationObject(reader));
                    }
                    return reservations;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Reservation> GetUpcomingReservations(DateTime startDate, DateTime endDate, Campsite site)
        {
            List<Reservation> reservations = new List<Reservation>();
            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    string sqlQuery = $"SELECT * FROM reservation " +
                        $"WHERE site_id = {site.SiteID} AND " +
                        $"((from_date BETWEEN '{startDate}' AND '{endDate}') OR " +
                        $"(to_date BETWEEN '{startDate}' AND '{endDate}'));";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reservations.Add(PopulateReservationObject(reader));
                    }
                    return reservations;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public int BookReservation(Reservation reservation)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    string sqlQuery = $"INSERT INTO reservation VALUES({reservation.SiteID}, " +
                        $"@Name, '{reservation.StartDate}', '{reservation.EndDate}', '{DateTime.Now}'); " +
                        $"SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@Name", reservation.Name);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public bool IsCampsiteAvailableForReservation(BasicSearch bs, Campsite cs)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    string sqlQuery = @"SELECT COUNT(*) FROM reservation WHERE reservation.site_id = " +
                        "@SiteID AND (((from_date BETWEEN @StartDate AND @EndDate) OR" +
                        "(to_date BETWEEN @StartDate AND @EndDate)) OR" +
                        "((@StartDate BETWEEN from_date AND to_date) OR" +
                        "(@EndDate BETWEEN from_date AND to_date)));";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@SiteID", cs.SiteID);
                    cmd.Parameters.AddWithValue("@StartDate", bs.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", bs.EndDate);

                    int numScheduleConflicts = Convert.ToInt32(cmd.ExecuteScalar());

                    return numScheduleConflicts == 0;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public Reservation PopulateReservationObject(SqlDataReader reader)
        {
            Reservation r = new Reservation();
            r.SiteID = Convert.ToInt32(reader["site_id"]);
            r.Name = Convert.ToString(reader["name"]);
            r.StartDate = Convert.ToDateTime(reader["from_date"]);
            r.EndDate = Convert.ToDateTime(reader["to_date"]);
            r.DateReserved = Convert.ToDateTime(reader["create_date"]);

            return r;
        }
    }
}
