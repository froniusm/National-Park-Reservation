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
        public List<Campground> GetAllCampgrounds()
        {
            throw new NotImplementedException();

            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(databaseConnection))
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand( , conn);
            //    }
            //}
            //catch (SqlException)
            //{
            //    throw;
            //}
        }
    }
}
