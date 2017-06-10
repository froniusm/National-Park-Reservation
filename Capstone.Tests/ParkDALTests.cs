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
    public class ParkDALTests
    {
        private TransactionScope tran;
        readonly string databaseConnection = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();          
        }

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        [TestMethod()]
        public void GetAllParksTest()
        {
            int originalNumParks;
            using (SqlConnection conn = new SqlConnection(databaseConnection))
            {
                conn.Open();

                // Count original number of parks
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM park;", conn);
                originalNumParks = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert fake park into database
                Park fakePark = new Park();
                fakePark.Name = "Hawaiian Mountains";
                fakePark.Location = "Hawaii";
                fakePark.EstablishedDate = Convert.ToDateTime("1907-05-14");
                fakePark.Area = 25000;
                fakePark.AnnualVisitorCount = 123987;
                fakePark.Description = "Beautiful, lush, tropic paradise away from the tourist laden beaches.";

                cmd = new SqlCommand($"INSERT INTO park VALUES('{fakePark.Name}', '{fakePark.Location}'," +
                    $" '{fakePark.EstablishedDate}', {fakePark.Area}, {fakePark.AnnualVisitorCount}," +
                    $"'{fakePark.Description}');", conn);

                cmd.ExecuteNonQuery();
            }
            // Arrange, act, and assert!
            ParkSqlDAL classToTest = new ParkSqlDAL(databaseConnection);
            List<Park> parks = classToTest.GetAllParks();
            Assert.AreEqual(originalNumParks + 1, parks.Count);
        }
    }
}
