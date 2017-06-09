using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.ProjectCLI;

namespace Capstone
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;
            CLI program = new CLI(connectionString);
            program.RunMainMenu();
        }
    }
}
