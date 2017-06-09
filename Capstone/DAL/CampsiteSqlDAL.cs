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
    public class CampsiteSqlDAL
    {
        private string databaseConnection;

        public CampsiteSqlDAL(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<Campsite> GetAllCampsitesFromCampground(BasicSearch bs)
        {
            List<Campsite> campsitesMeetingCriteria = new List<Campsite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    int campgroundID = bs.LocationID;
                    int monthStartVisit = bs.StartDate.Month;
                    int monthEndVisit = bs.EndDate.Month;

                    string sqlQuery = "SELECT * FROM [site] " +
                        "INNER JOIN campground ON campground.campground_id = [site].campground_id " +
                        $"WHERE campground.park_id = {campgroundID} AND " +
                        $"campground.open_from_mm <= {monthStartVisit} AND " +
                        $"campground.open_to_mm >= {monthEndVisit};";

                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        campsitesMeetingCriteria.Add(PopulateCampsiteObject(reader));
                    }

                    return campsitesMeetingCriteria;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Campsite> GetAllCampsitesFromCampground(BasicSearch bs, AdvancedSearchOptions aso)
        {
            throw new NotImplementedException();
        }

        public List<Campsite> GetAllCampsitesFromPark(BasicSearch bs)
        {
            List<Campsite> campsitesMeetingCriteria = new List<Campsite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    int parkID = bs.LocationID;
                    int monthStartVisit = bs.StartDate.Month;
                    int monthEndVisit = bs.EndDate.Month;

                    string sqlQuery = "SELECT * FROM [site] " +
                        "INNER JOIN campground ON campground.campground_id = [site].campground_id " +
                        $"WHERE campground.park_id = {parkID} AND " +
                        $"campground.open_from_mm <= {monthStartVisit} AND " +
                        $"campground.open_to_mm >= {monthEndVisit};";

                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        campsitesMeetingCriteria.Add(PopulateCampsiteObject(reader));
                    }

                    return campsitesMeetingCriteria;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public List<Campsite> GetAllCampsitesFromPark(BasicSearch bs, AdvancedSearchOptions aso)
        {
            List<Campsite> campsitesMeetingCriteria = new List<Campsite>();

            try
            {
                using (SqlConnection conn = new SqlConnection(databaseConnection))
                {
                    conn.Open();
                    int parkID = bs.LocationID;
                    int monthStartVisit = bs.StartDate.Month;
                    int monthEndVisit = bs.EndDate.Month;
                    int maxOccupancy = aso.MaxOccupancy;
                    bool accessible = aso.NeedsAccessibility;
                    int maxRVLength = aso.RequiredRVLength;
                    bool needsUtilities = aso.NeedsUtilityHookup;

                    string accessiblityQuery = accessible ? " AND site.accessible = 1" : "";

                    string sqlQuery = "SELECT * FROM [site] " +
                        "INNER JOIN campground ON campground.campground_id = [site].campground_id " +
                        $"WHERE campground.park_id = {parkID} AND " +
                        $"campground.open_from_mm <= {monthStartVisit} AND " +
                        $"campground.open_to_mm >= {monthEndVisit} AND " +
                        $"site.max_occupancy >= {maxOccupancy} AND " +
                        $"site.max_rv_length <= {maxRVLength}" +
                        $"{accessiblityQuery};";

                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        campsitesMeetingCriteria.Add(PopulateCampsiteObject(reader));
                    }

                    return campsitesMeetingCriteria;
                }
            }
            catch (SqlException)
            {
                throw;
            }
        }

        private Campsite PopulateCampsiteObject(SqlDataReader reader)
        {
            Campsite c = new Campsite();
            c.SiteID = Convert.ToInt32(reader["site_id"]);
            c.CampgroundID = Convert.ToInt32(reader["campground_id"]);
            c.SiteNumber = Convert.ToInt32(reader["site_number"]);
            c.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            c.IsAccessible = Convert.ToBoolean(reader["accessible"]);
            c.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
            c.HasUtilities = Convert.ToBoolean(reader["utilities"]);

            return c;
        }
    }
}
