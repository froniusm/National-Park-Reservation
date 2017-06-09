using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Capstone.Models;
using Capstone.DAL;
using Capstone.Search;


namespace Capstone.ProjectCLI
{
    public class CLI
    {
        private string databaseConnection;
        private const string Command_SearchCampsiteByPark = "1";
        private const string Command_SearchCampsiteByCampground = "2";
        private const string Command_Quit = "Q";

        public CLI(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void RunMainMenu()
        {
            CLIHelper.DisplayHeader();
            
            // Present user with options and record his or her selection
            string message = " Welcome! Please select from among the following options:\n [1] Search campsite by park" +
                "\n [2] Search campsite by campground\n [Q] Quit\n\n >> ";
            List<string> answerChoices = new List<string> { "1", "2", "Q" };
            bool isCaseSensitive = false;
            string userChoice = CLIHelper.GetString(message, answerChoices, isCaseSensitive);

            // For either option, user must select park first
            Park selectedPark = SelectPark();

            // Retrieve list of campsites meeting search criteria
            switch (userChoice)
            {
                case Command_SearchCampsiteByPark:
                    List<Campsite> campsMeetingCriteria = SearchForCampsitesMeetingCriteria(selectedPark.ParkID);
                    break;

                case Command_SearchCampsiteByCampground:
                    break;
            }
        }

        public Park SelectPark()
        {
            CLIHelper.DisplayHeader();

            ParkSqlDAL p = new ParkSqlDAL(databaseConnection);
            List<Park> AllParks = p.GetAllParks();

            // Print table header for information about all parks
            Console.WriteLine("Name".PadLeft(9).PadRight(25) + "Location".PadRight(20)
                + "Date Established".PadRight(25) + "Acres".PadRight(10)
                + "Annual Number of Visitors".PadRight(10) + "Description");

            // Print information for each park
            for (int i = 1; i <= AllParks.Count; i++)
            {
                Console.WriteLine($" [{i}] {AllParks[i - 1].ToString()}\n");
            }

            Console.ReadLine();

            // Store user park selection
            string message = "\n Please choose a park. >> ";
            List<int> availableChoices = Enumerable.Range(1, AllParks.Count).ToList<int>();
            int userSelection = CLIHelper.GetInteger(message, availableChoices);
            Park selectedPark = AllParks[userSelection - 1];

            return selectedPark;
        }

        public List<Campsite> SearchForCampsitesMeetingCriteria(int locationID)
        {
            CLIHelper.DisplayHeader();
            
            // Create new CampsiteSqlDAL and empty list of campsites
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);
            List<Campsite> campsitesMeetingCriteria = new List<Campsite>();

            // Ask user if they want to perform a basic or advanced search and then record their choice
            string searchMessage = " Would you like to perform a basic or advanced search?\n" +
                " [1] Basic\n [2] Advanced\n >> ";
            string userSearchChoice = CLIHelper.GetString(searchMessage, new List<string> { "1", "2" });

            // Create basic search object
            BasicSearch basicSearch = RunBasicSearch(locationID);

            // If basic search requested, retrieve campsites meeting criteria
            if (userSearchChoice == "1")
            {
                campsitesMeetingCriteria = campsiteDAL.GetAllCampsitesFromPark(basicSearch);
            }

            // Otherwise, conduct advanced search and retrieve campsites meeting basic and advanced criteria
            else
            {
                AdvancedSearchOptions advancedSearch = RunAdvancedSearch();
                campsitesMeetingCriteria = campsiteDAL.GetAllCampsitesFromPark(basicSearch, advancedSearch);
            }

            foreach (Campsite c in campsitesMeetingCriteria)
            {
                Console.WriteLine(c.ToString());
            }
            Console.ReadLine();
            return campsitesMeetingCriteria;
        }

        public BasicSearch RunBasicSearch(int locationID)
        {
            CLIHelper.DisplayHeader();
            
            // Messages
            string startDateMessage = " On what date would you like to begin your adventure at National" +
                " Park System?  Please type in the format 'yyyy-mm-dd'. >> ";
            string endDateMessage = " On what date would you like to end your adventure? " +
                " Please type in the format 'yyyy-mm-dd'. >> ";

            // Prompt user for basic search criteria
            DateTime userStartDate = CLIHelper.GetDateTime(startDateMessage);
            DateTime userEndDate = CLIHelper.GetDateTime(endDateMessage);

            // Generate BasicSearch object
            BasicSearch bs = new BasicSearch();
            bs.LocationID = locationID;
            bs.StartDate = userStartDate;
            bs.EndDate = userEndDate;

            return bs;
        }

        public AdvancedSearchOptions RunAdvancedSearch()
        {
            // Messages
            string maxOccupancyMessage = " How many persons are in your party? >> ";
            string accessibilityMessage = " Do you require disability accessiblity services? (Y/N) >> ";
            string hasRVMessage = " Are you planning to bring an RV to the campsite? (Y/N) >> ";
            string rvLengthMessage = " What is the size of your RV in feet? >> ";
            string needsUtilitiesMessage = " Do you require a utility hookup for your RV? >> ";

            // Prompt the user for AdvancedSearchOptions criteria
            int maxOccupancy = CLIHelper.GetInteger(maxOccupancyMessage);
            string needsAccessibility = CLIHelper.GetString(accessibilityMessage, new List<string> { "Y", "N" });
            string hasRV = CLIHelper.GetString(hasRVMessage, new List<string> { "Y", "N" });
            bool hasRVBool = hasRV == "Y" ? true : false;

            // -- If user has RV, prompt for their RV length and whether their party requires a utility hookup
            int rvLength = hasRVBool ? CLIHelper.GetInteger(rvLengthMessage) : 0;
            bool needsUtilitiesBool = false;
            if (hasRVBool)
            {
                string needsUtilities = CLIHelper.GetString(needsUtilitiesMessage, new List<string> { "Y", "N" });
                needsUtilitiesBool = needsUtilities == "Y" ? true : false;
            }

            // Create AdvancedSearchOptions object
            AdvancedSearchOptions aso = new AdvancedSearchOptions();
            aso.MaxOccupancy = maxOccupancy;
            aso.NeedsAccessibility = needsAccessibility == "Y" ? true : false;
            aso.NeedsUtilityHookup = needsUtilitiesBool;
            aso.RequiredRVLength = rvLength;

            return aso;
        }
    }
}
