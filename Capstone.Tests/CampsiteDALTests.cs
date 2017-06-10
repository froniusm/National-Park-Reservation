using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;
using Capstone.Search;


namespace Capstone.Tests
{
    [TestClass]
    public class CampsiteDALTests
    {
        private TransactionScope tran;
        readonly string databaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        private Campsite fakeCampsite;
        private Campground fakeCampground;
        private Park fakePark;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(databaseConnection))
            {
                conn.Open();

                // Insert fake park into database
                fakePark = new Park();
                fakePark.Name = "Hawaiian Mountains";
                fakePark.Location = "Hawaii";
                fakePark.EstablishedDate = Convert.ToDateTime("1907-05-14");
                fakePark.Area = 25000;
                fakePark.AnnualVisitorCount = 123987;
                fakePark.Description = "Beautiful, lush, tropic paradise away from the tourist laden beaches.";

                SqlCommand cmd = new SqlCommand($"INSERT INTO park VALUES('{fakePark.Name}', '{fakePark.Location}'," +
                    $" '{fakePark.EstablishedDate}', {fakePark.Area}, {fakePark.AnnualVisitorCount}," +
                    $"'{fakePark.Description}'); SELECT SCOPE_IDENTITY();", conn);

                fakePark.ParkID = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert fake campground
                fakeCampground = new Campground();
                fakeCampground.ParkID = fakePark.ParkID;
                fakeCampground.Name = "Party Campground";
                fakeCampground.OpenMonth = 5;
                fakeCampground.CloseMonth = 10;
                fakeCampground.DailyFee = 40M;

                cmd = new SqlCommand($"INSERT INTO campground VALUES({fakeCampground.ParkID}," +
                    $"'{fakeCampground.Name}', {fakeCampground.OpenMonth}, {fakeCampground.CloseMonth}," +
                    $"{fakeCampground.DailyFee}); SELECT SCOPE_IDENTITY();", conn);

                fakeCampground.CampgroundID = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert fake campsite
                fakeCampsite = new Campsite();
                fakeCampsite.CampgroundID = fakeCampground.CampgroundID;
                fakeCampsite.SiteNumber = 1;
                fakeCampsite.MaxOccupancy = 5;
                fakeCampsite.IsAccessible = true;
                fakeCampsite.MaxRVLength = 20;
                fakeCampsite.HasUtilities = true;

                cmd = new SqlCommand($"INSERT INTO site VALUES({fakeCampsite.CampgroundID}," +
                    $"{fakeCampsite.SiteNumber}, {fakeCampsite.MaxOccupancy}, '{fakeCampsite.IsAccessible}'," +
                    $"{fakeCampsite.MaxRVLength}, '{fakeCampsite.HasUtilities}'); SELECT SCOPE_IDENTITY();", conn);

                fakeCampsite.SiteID = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetAllCampsitesFromCampgroundTest()
        {
            // Act, arrange, and assert!
            CampsiteSqlDAL classToTest = new CampsiteSqlDAL(databaseConnection);
            BasicSearch search = new BasicSearch();
            search.LocationID = fakeCampground.CampgroundID;
            search.StartDate = Convert.ToDateTime("2017-06-08");
            search.EndDate = Convert.ToDateTime("2017-06-12");

            List<Campsite> campsites = classToTest.GetAllCampsitesFromCampground(search);
            Assert.AreEqual(1, campsites.Count);
        }

        [TestMethod()]
        public void GetAllCampsitesFromCampgroundWithAdvancedOptionsTest()
        {
            // Act, arrange, and assert!
            CampsiteSqlDAL classToTest = new CampsiteSqlDAL(databaseConnection);
            BasicSearch search = new BasicSearch();
            search.LocationID = fakeCampground.CampgroundID;
            search.StartDate = Convert.ToDateTime("2017-06-08");
            search.EndDate = Convert.ToDateTime("2017-06-12");

            AdvancedSearchOptions advancedSearch = new AdvancedSearchOptions();
            advancedSearch.MaxOccupancy = 2;
            advancedSearch.NeedsAccessibility = true;
            advancedSearch.RequiredRVLength = 0;
            advancedSearch.NeedsUtilityHookup = false;

            List<Campsite> campsites = classToTest.GetAllCampsitesFromCampground(search, advancedSearch);
            Assert.AreEqual(1, campsites.Count);
        }

        [TestMethod()]
        public void GetAllCampsitesFromParkTest()
        {
            // Act, arrange, and assert!
            CampsiteSqlDAL classToTest = new CampsiteSqlDAL(databaseConnection);
            BasicSearch search = new BasicSearch();
            search.LocationID = fakePark.ParkID;
            search.StartDate = Convert.ToDateTime("2017-06-08");
            search.EndDate = Convert.ToDateTime("2017-06-12");

            List<Campsite> campsites = classToTest.GetAllCampsitesFromPark(search);
            Assert.AreEqual(1, campsites.Count);
        }

        [TestMethod()]
        public void GetAllCampsitesFromParkWithAdvancedOptionsTest()
        {
            // Act, arrange, and assert!
            CampsiteSqlDAL classToTest = new CampsiteSqlDAL(databaseConnection);
            BasicSearch search = new BasicSearch();
            search.LocationID = fakePark.ParkID;
            search.StartDate = Convert.ToDateTime("2017-06-08");
            search.EndDate = Convert.ToDateTime("2017-06-12");

            AdvancedSearchOptions advancedSearch = new AdvancedSearchOptions();
            advancedSearch.MaxOccupancy = 2;
            advancedSearch.NeedsAccessibility = true;
            advancedSearch.RequiredRVLength = 0;
            advancedSearch.NeedsUtilityHookup = false;

            List<Campsite> campsites = classToTest.GetAllCampsitesFromPark(search, advancedSearch);
            Assert.AreEqual(1, campsites.Count);
        }
    }
}
