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
    public class ReservationDALTests
    {
        private TransactionScope tran;
        readonly string databaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
        private Campsite fakeCampsite;
        private Campground fakeCampground;
        private Park fakePark;
        private Reservation fakeReservation;

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

                // Create fake campsite
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

                // Insert fake reservation
                fakeReservation = new Reservation();
                fakeReservation.SiteID = fakeCampsite.SiteID;
                fakeReservation.Name = "The Bob Smith Family";
                fakeReservation.StartDate = Convert.ToDateTime("2017-08-14");
                fakeReservation.EndDate = Convert.ToDateTime("2017-08-21");
                fakeReservation.DateReserved = DateTime.Now;

                cmd = new SqlCommand($"INSERT INTO reservation VALUES({fakeReservation.SiteID}," +
                    $"'{fakeReservation.Name}', '{fakeReservation.StartDate}', " +
                    $"'{fakeReservation.EndDate}', '{fakeReservation.DateReserved}'); " +
                    $"SELECT SCOPE_IDENTITY();", conn);

                fakeReservation.ReservationID = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetUpcomingReservationsTest_DateRangeParameter()
        {
            ReservationSqlDAL classToTest = new ReservationSqlDAL(databaseConnection);
            DateTime startDate = Convert.ToDateTime("2017-08-01");
            DateTime endDate = Convert.ToDateTime("2017-08-31");
            List<Reservation> reservations = classToTest.GetUpcomingReservations(startDate, endDate);

            Assert.AreEqual(1, reservations.Count);
        }

        [TestMethod()]
        public void GetUpcomingReservationsTest_DateRangeAndCampsiteParameters()
        {
            ReservationSqlDAL classToTest = new ReservationSqlDAL(databaseConnection);
            DateTime startDate = Convert.ToDateTime("2017-08-01");
            DateTime endDate = Convert.ToDateTime("2017-08-31");
            List<Reservation> reservations = classToTest.GetUpcomingReservations(startDate, endDate, fakeCampsite);

            Assert.AreEqual(1, reservations.Count);
        }

        [TestMethod()]
        public void BookReservationTest()
        {
            ReservationSqlDAL classToTest = new ReservationSqlDAL(databaseConnection);

            // Arrange: create new, non-conflicting reservation
            Reservation nonConflictingReservation = new Reservation();
            nonConflictingReservation.SiteID = fakeCampsite.SiteID;
            nonConflictingReservation.Name = "Crazy Town";
            nonConflictingReservation.StartDate = Convert.ToDateTime("2017-08-01");
            nonConflictingReservation.EndDate = Convert.ToDateTime("2017-08-13");

            // Act
            nonConflictingReservation.ReservationID = classToTest.BookReservation(nonConflictingReservation);

            // Assert
            Assert.AreEqual(fakeReservation.ReservationID + 1, nonConflictingReservation.ReservationID);
        }

        [TestMethod()]
        public void IsCampsiteAvailableForReservationTest()
        {
            ReservationSqlDAL classToTest = new ReservationSqlDAL(databaseConnection);

            // Arrange: create new search whose proposed lodging dates conflict with fakeReservation
            BasicSearch search = new BasicSearch();
            search.LocationID = fakeCampsite.SiteID;
            search.StartDate = Convert.ToDateTime("2017-08-14");
            search.EndDate = Convert.ToDateTime("2017-08-17");

            // Act
            bool wasReserved = classToTest.IsCampsiteAvailableForReservation(search, fakeCampsite);

            // Assert
            Assert.IsFalse(wasReserved);
        }
    }
}
