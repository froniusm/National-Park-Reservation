using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;

namespace Capstone.DAL
{
    public class ParkSqlDAL
    {
        private string databaseConnection;

        public ParkSqlDAL(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Park> GetAllParks()
        {
            List<Park> AllParks = new List<Park>();

            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM park;", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        AllParks.Add(PopulateParkObject(reader));
                    }

                    return AllParks;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        private Park PopulateParkObject(SqlDataReader reader)
        {
            Park p = new Park();

            p.ParkID = Convert.ToInt32(reader["park_id"]);
            p.Name = Convert.ToString(reader["name"]);
            p.Location = Convert.ToString(reader["location"]);
            p.EstablishedDate = Convert.ToDateTime(reader["establish_date"]);
            p.Area = Convert.ToInt32(reader["area"]);
            p.AnnualVisitorCount = Convert.ToInt32(reader["visitors"]);
            p.Description = Convert.ToString(reader["description"]);

            return p;
        }
    }
}
