using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Transactions;
using Capstone.DAL;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class CampgroundDALTests
    {
        private TransactionScope tran;
        readonly string databaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        private Campground fakeCampground;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();

            using (SqlConnection conn = new SqlConnection(databaseConnection))
            {
                conn.Open();

                // Insert fake park into database
                Park fakePark = new Park();
                fakePark.Name = "Hawaiian Mountains";
                fakePark.Location = "Hawaii";
                fakePark.EstablishedDate = Convert.ToDateTime("1907-05-14");
                fakePark.Area = 25000;
                fakePark.AnnualVisitorCount = 123987;
                fakePark.Description = "Beautiful, lush, tropic paradise away from the tourist laden beaches.";

                SqlCommand cmd = new SqlCommand($"INSERT INTO park VALUES('{fakePark.Name}', '{fakePark.Location}'," +
                    $" '{fakePark.EstablishedDate}', {fakePark.Area}, {fakePark.AnnualVisitorCount}," +
                    $"'{fakePark.Description}'); SELECT SCOPE_IDENTITY();", conn);

                int fakeParkID = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert fake campground
                fakeCampground = new Campground();
                fakeCampground.ParkID = fakeParkID;
                fakeCampground.Name = "Party Campground";
                fakeCampground.OpenMonth = 5;
                fakeCampground.CloseMonth = 10;
                fakeCampground.DailyFee = 40M;

                cmd = new SqlCommand($"INSERT INTO campground VALUES({fakeCampground.ParkID}," +
                    $"'{fakeCampground.Name}', {fakeCampground.OpenMonth}, {fakeCampground.CloseMonth}," +
                    $"{fakeCampground.DailyFee}); SELECT SCOPE_IDENTITY();", conn);

                fakeCampground.CampgroundID = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetAllCampgroundsFromParkTest()
        {
            // Act, arrange, and assert!
            CampgroundSqlDAL classToTest = new CampgroundSqlDAL(databaseConnection);
            List<Campground> campgrounds = classToTest.GetAllCampgroundsFromPark(fakeCampground.ParkID);
            Assert.AreEqual(1, campgrounds.Count);
        }

        [TestMethod()]
        public void GetDailyFeeTest()
        {
            // Act, arrange, and assert!
            CampgroundSqlDAL classToTest = new CampgroundSqlDAL(databaseConnection);
            Assert.AreEqual(fakeCampground.DailyFee, classToTest.GetDailyFee(fakeCampground.CampgroundID));
        }
    }
}
