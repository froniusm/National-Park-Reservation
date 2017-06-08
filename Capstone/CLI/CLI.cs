using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Capstone.Models;
using Capstone.DAL;
using Capstone.Search;


namespace Capstone.CLI
{
    public class CLI
    {
        private string databaseConnection;

        public CLI(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void DisplayHeader()
        {
            Console.WriteLine("".PadRight(30, '*'));
            Console.WriteLine("*NATIONAL PARK CAMPSITE RESERVATION SYSTEM*");
            Console.WriteLine("".PadRight(30, '*'));
        }
    }
}
