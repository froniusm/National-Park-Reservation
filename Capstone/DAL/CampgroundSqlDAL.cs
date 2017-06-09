using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class CampgroundSqlDAL
    {
        private string databaseConnection;

        public CampgroundSqlDAL(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Campground> GetAllCampgroundsFromPark(int parkID)
        {
            List<Campground> allCampgrounds = new List<Campground>();

            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    string sqlQuery = $"SELECT * FROM campground WHERE campground.park_id = {parkID};";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        allCampgrounds.Add(PopulateCampgroundObject(reader));
                    }

                    return allCampgrounds;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public Campground PopulateCampgroundObject(SqlDataReader reader)
        {
            Campground cg = new Campground();
            cg.CampgroundID = Convert.ToInt32(reader["campground_id"]);
            cg.ParkID = Convert.ToInt32(reader["park_id"]);
            cg.Name = Convert.ToString(reader["name"]);
            cg.OpenMonth = Convert.ToInt32(reader["open_from_mm"]);
            cg.CloseMonth = Convert.ToInt32(reader["open_to_mm"]);
            cg.DailyFee = Convert.ToDecimal(reader["daily_fee"]);

            return cg;
        }
    }
}
