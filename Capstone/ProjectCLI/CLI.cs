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
        private BasicSearch userBasicSearch;
        private AdvancedSearchOptions userAdvancedSearch;
        private const string Command_SearchCampsiteByPark = "1";
        private const string Command_SearchCampsiteByCampground = "2";
        private const string Command_Quit = "Q";

        public CLI(string databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public void RunMainMenu()
        {
            // Run main menu loop
            bool mainMenuRunning = true;
            while (mainMenuRunning)
            {
                CLIHelper.DisplayHeader();
                
                // Initialize variables to be used in either first or second menu option
                Park selectedPark = new Park();
                List<Campsite> campsMeetingSearchCriteria = new List<Campsite>();

                // Present user with three options and record his or her selection
                string message = " Welcome! Please select from among the following options:\n [1] Search for campsite by park" +
                    "\n [2] Search for campsite by campground\n [Q] Quit\n\n >> ";
                string userChoice = CLIHelper.GetString(message, new List<string> { "1", "2", "Q" });

                switch (userChoice)
                {
                    case Command_SearchCampsiteByPark:
                        selectedPark = SelectPark();
                        campsMeetingSearchCriteria = CampsitesInParkSearchLoop(selectedPark.ParkID);
                        if (campsMeetingSearchCriteria.Count > 0)
                        {
                            ReservationLoop(campsMeetingSearchCriteria);
                        }
                        break;

                    case Command_SearchCampsiteByCampground:
                        selectedPark = SelectPark();
                        Campground selectedCampground = SelectCampgroundFromPark(selectedPark.ParkID);
                        campsMeetingSearchCriteria = CampsitesInCampgroundSearchLoop(selectedCampground.CampgroundID);
                        if (campsMeetingSearchCriteria.Count > 0)
                        {
                            ReservationLoop(campsMeetingSearchCriteria);
                        }
                        break;

                    case Command_Quit:
                        Console.Write("\n Thank you for using the National Park Reservation System.  Please come again soon! ");
                        Console.ReadLine();
                        mainMenuRunning = false;
                        break;
                }
            }
        }

        private Park SelectPark()
        {
            CLIHelper.DisplayHeader();

            ParkSqlDAL p = new ParkSqlDAL(databaseConnection);
            List<Park> AllParks = p.GetAllParks();

            // Print information for each park
            for (int i = 1; i <= AllParks.Count; i++)
            {
                Console.WriteLine($" [{i}] {AllParks[i - 1].Name.ToUpper()}");
                Console.WriteLine($"     State: {AllParks[i - 1].Location}");
                Console.WriteLine($"     Established: {AllParks[i - 1].EstablishedDate.ToShortDateString()}");
                Console.WriteLine($"     No. Acres: {AllParks[i - 1].Area.ToString("n0")}");
                Console.WriteLine($"     No. Annual Visitors: {AllParks[i - 1].AnnualVisitorCount.ToString("n0")}");
                Console.WriteLine("\n" + CLIHelper.FormatParagraph(AllParks[i - 1].Description) + "\n");
            }

            // Store user park selection
            string message = " Please choose a park. >> ";
            List<int> availableChoices = Enumerable.Range(1, AllParks.Count).ToList<int>();
            int userSelection = CLIHelper.GetInteger(message, availableChoices);
            Park selectedPark = AllParks[userSelection - 1];

            return selectedPark;
        }

        private Campground SelectCampgroundFromPark(int parkID)
        {
            CLIHelper.DisplayHeader();

            CampgroundSqlDAL campgroundDAL = new CampgroundSqlDAL(databaseConnection);

            // Retrieve all campgrounds in selected park
            List<Campground> campgroundsInPark = campgroundDAL.GetAllCampgroundsFromPark(parkID);

            // Print header for campground information
            Console.WriteLine("Name".PadLeft(9).PadRight(40) + "Months Open".PadRight(30)
                + "Daily Fee".PadRight(10));

            // Print each campground in selected park
            for (int i = 1; i <= campgroundsInPark.Count; i++)
            {
                Console.WriteLine($" [{i}] {campgroundsInPark[i - 1].ToString()}");
            }

            // Store user campground selection
            string message = "\n Please choose a campground. >> ";
            List<int> availableChoices = Enumerable.Range(1, campgroundsInPark.Count).ToList<int>();
            int userSelection = CLIHelper.GetInteger(message, availableChoices);
            Campground selectedCampground = campgroundsInPark[userSelection - 1];

            return selectedCampground;
        }

        private List<Campsite> CampsitesInParkSearchLoop(int parkID)
        {
            List<Campsite> cs = new List<Campsite>();
            while (true)
            {
                cs = SearchForCampsitesInPark(parkID);
                if (cs.Count == 0)
                {
                    string warningMessage = "\n There were no campsites that met your search criteria.\n\n" +
                        " Please check that the park's campgrounds are open during the month in which you\n" +
                        " would like to visit. We apologize if we are unable to accommodate your request for\n" +
                        " accessiblity services, RV parking and utility hookups, or large parties.\n\n" +
                        " Would you like to revise your search? (Y/N) >> ";
                    string userChoice = CLIHelper.GetString(warningMessage, new List<string> { "Y", "N" });
                    bool tryAgain = userChoice == "Y" ? true : false;
                    if (!tryAgain)
                    {
                        return cs;
                    }
                }
                else
                {
                    return cs;
                }
            }
        }

        private List<Campsite> CampsitesInCampgroundSearchLoop(int campgroundID)
        {
            List<Campsite> cs = new List<Campsite>();
            while (true)
            {
                cs = SearchForCampsitesInCampground(campgroundID);
                if (cs.Count == 0)
                {
                    string warningMessage = "\n There were no campsites that met your search criteria.\n\n" +
                        " Please check that the park's campgrounds are open during the month in which you\n" +
                        " would like to visit. We apologize if we are unable to accommodate your request for\n" +
                        " accessiblity services, RV parking and utility hookups, or large parties.\n\n" +
                        " Would you like to revise your search? (Y/N) >> ";
                    string userChoice = CLIHelper.GetString(warningMessage, new List<string> { "Y", "N" });
                    bool tryAgain = userChoice == "Y" ? true : false;
                    if (!tryAgain)
                    {
                        return cs;
                    }
                }
                else
                {
                    return cs;
                }
            }
        }

        private List<Campsite> SearchForCampsitesInPark(int parkID)
        {
            // Create new CampsiteSqlDAL
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);

            // Conduct search and store basic search criteria
            List<ISearchObject> parkSearchCriteria = RecordSearchCriteria(parkID);
            userBasicSearch = (BasicSearch) parkSearchCriteria[0];

            // If only basic search conducted, return campsites meeting basic criteria
            if (parkSearchCriteria.Count == 1)
            {
                return campsiteDAL.GetAllCampsitesFromPark(userBasicSearch);
            }

            // If advanced search conducted, return campsites meeting basic and advanced criteria
            userAdvancedSearch = (AdvancedSearchOptions) parkSearchCriteria[1];
            return campsiteDAL.GetAllCampsitesFromPark(userBasicSearch, userAdvancedSearch);
        }

        private List<Campsite> SearchForCampsitesInCampground(int campgroundID)
        {
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);

            // Retrieve user search criteria
            List<ISearchObject> campgroundSearchCriteria = RecordSearchCriteria(campgroundID);
            userBasicSearch = (BasicSearch) campgroundSearchCriteria[0];

            // If only basic search conducted, return campsites meeting basic criteria
            if (campgroundSearchCriteria.Count == 1)
            {
                return campsiteDAL.GetAllCampsitesFromCampground(userBasicSearch);
            }

            // If advanced search conducted, return campsites meeting basic *and* advanced criteria
            userAdvancedSearch = (AdvancedSearchOptions) campgroundSearchCriteria[1];

            return campsiteDAL.GetAllCampsitesFromCampground(userBasicSearch, userAdvancedSearch);
        }

        private List<ISearchObject> RecordSearchCriteria(int locationID)
        {
            CLIHelper.DisplayHeader();

            // Initialize empty list of search objects
            List<ISearchObject> searches = new List<ISearchObject>();

            // Ask user to select either a basic or advanced campsite search
            string searchMessage = " Would you like to perform a basic or advanced campsite search?\n" +
                " [1] Basic\n [2] Advanced\n\n >> ";
            string userSearchChoice = CLIHelper.GetString(searchMessage, new List<string> { "1", "2" });

            // Conduct basic search and add object to list
            BasicSearch basicSearch = EnterBasicSearchCriteria(locationID);
            searches.Add(basicSearch);

            // Conduct advanced search if requested and add object to list
            if (userSearchChoice == "2")
            {
                AdvancedSearchOptions advancedSearch = EnterAdvancedSearchCriteria();
                searches.Add(advancedSearch);
            }

            return searches;
        }

        private BasicSearch EnterBasicSearchCriteria(int locationID)
        {
            CLIHelper.DisplayHeader();

            // Messages
            string startDateMessage = " On what date would you like to begin your adventure at National" +
                " Park System?\n Please type in the format 'yyyy-mm-dd'.\n\n >> ";
            string endDateMessage = " On what date would you like to end your adventure?\n" +
                " Please type in the format 'yyyy-mm-dd'.\n\n >> ";

            // Prompt user for basic search criteria
            DateTime userStartDate;
            DateTime userEndDate;
            while (true)
            {
                userStartDate = CLIHelper.GetDateTime(startDateMessage);
                userEndDate = CLIHelper.GetDateTime(endDateMessage);

                if (userEndDate >= userStartDate)
                {
                    break;
                }

                Console.Write($" Error! Your proposed camping start date is {userStartDate.ToShortDateString()}, " +
                        $"but your end date is {userEndDate.ToShortDateString()}.\n Please make sure that your end date " +
                        $"follows your start date. ");
                Console.ReadLine();
                CLIHelper.DisplayHeader();
            }

            // Generate BasicSearch object
            BasicSearch bs = new BasicSearch();
            bs.LocationID = locationID;
            bs.StartDate = userStartDate;
            bs.EndDate = userEndDate;

            return bs;
        }

        private AdvancedSearchOptions EnterAdvancedSearchCriteria()
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

        private Campsite SelectCampsite(List<Campsite> campsMeetingSearchCriteria)
        {
            // Display header
            Console.WriteLine(" CAMPSITES MEETING SEARCH CRITERIA");
            Console.WriteLine($" Proposed Date Range: {userBasicSearch.StartDate.ToShortDateString()} to " +
                $"{userBasicSearch.EndDate.ToShortDateString()}");

            // Display information for each campsite meeting search criteria
            int currentCampgroundID = 0;
            for (int i = 1; i <= campsMeetingSearchCriteria.Count; i++)
            {
                if (currentCampgroundID != campsMeetingSearchCriteria[i - 1].CampgroundID)
                {
                    currentCampgroundID = campsMeetingSearchCriteria[i - 1].CampgroundID;
                    Console.WriteLine($"\n Campground: {GetCampgroundNameFromCampsite(campsMeetingSearchCriteria[i - 1])}" +
                        $"\n Total Cost: {CalculateTotalCostOfStay(currentCampgroundID).ToString("C")}");
                    Console.WriteLine("Site Number".PadLeft(21) + "Max Occupancy".PadLeft(22) +
                        "Accessible".PadLeft(17) + "Max RV Length".PadLeft(23) + "Utilities".PadLeft(16));
                }
                
                int spacesToInsert = i < 10 ? 6 : 5;
                Console.WriteLine($" [{i}]" + "".PadLeft(spacesToInsert) + $"{campsMeetingSearchCriteria[i - 1].ToString()}");
            }

            // Prompt user to select campsite
            List<int> availableChoices = Enumerable.Range(1, campsMeetingSearchCriteria.Count).ToList<int>();
            int userChoice = CLIHelper.GetInteger("\n Please select campsite to check availability for booking. >> ", availableChoices);

            // Return selected campsite
            return campsMeetingSearchCriteria[userChoice - 1];
        }

        private string GetCampgroundNameFromCampsite(Campsite site)
        {
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);
            return campsiteDAL.GetCampgroundName(site);
        }

        private string GetParkNameFromCampsite(Campsite site)
        {
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);
            return campsiteDAL.GetParkName(site);
        }

        private decimal CalculateTotalCostOfStay(int campgroundID)
        {
            CampgroundSqlDAL campgroundDAL = new CampgroundSqlDAL(databaseConnection);
            decimal dailyFee = campgroundDAL.GetDailyFee(campgroundID);
            int numDaysInStay = userBasicSearch.EndDate.Subtract(userBasicSearch.StartDate).Days;

            return dailyFee * numDaysInStay;
        }

        private void ReservationLoop(List<Campsite> campsMeetingSearchCriteria)
        {
            ReservationSqlDAL reservationDAL = new ReservationSqlDAL(databaseConnection);
            while (true)
            {
                CLIHelper.DisplayHeader();
                Campsite campToBook = SelectCampsite(campsMeetingSearchCriteria);
                bool isCampAvailable = IsCampsiteAvailableForReservation(campToBook);

                if (!isCampAvailable)
                {
                    string message = $"\n We're sorry; those dates are not currently available for campsite #{campToBook.SiteNumber}.\n" +
                        " [1] Select another date range\n [2] Select alternate campsite\n [3] Return to main menu >> ";
                    int userChoice = CLIHelper.GetInteger(message, new List<int> { 1, 2, 3 });
                    if (userChoice == 1)
                    {
                        CLIHelper.DisplayHeader();
                        int numDaysAhead = CLIHelper.GetInteger("\n To help you plan, let's check what dates are booked " +
                            "in the near future for this campsite...\n View upcoming reservations over the next __ days. >> ");
                        List<Reservation> upcomingReservations = reservationDAL.GetUpcomingReservations(DateTime.Now, DateTime.Now.AddDays(numDaysAhead), campToBook);
                        Console.WriteLine(" There (is)are " + upcomingReservations.Count + " upcoming reservation(s) in that timeframe.\n");
                        Console.WriteLine(" Reservation For".PadRight(41) + "Start Date".PadRight(15) +
                            "End Date".PadRight(15) + "Date Booked");
                        foreach (Reservation r in upcomingReservations)
                        {
                            Console.WriteLine(" " + r.ToString());
                        }
                        Console.ReadLine();
                        userBasicSearch = EnterBasicSearchCriteria(campToBook.SiteID);
                    }
                    else if (userChoice == 3)
                    {
                        break;
                    }
                }
                else
                {
                    MakeReservation(campToBook);
                    break;
                }
            }
        }

        private bool IsCampsiteAvailableForReservation(Campsite site)
        {
            ReservationSqlDAL r = new ReservationSqlDAL(databaseConnection);
            return r.IsCampsiteAvailableForReservation(userBasicSearch, site);
        }

        private void MakeReservation(Campsite site)
        {
            CLIHelper.DisplayHeader();

            // Display reservation information
            Dictionary<string, string> yesNoChoices = new Dictionary<string, string>() { { "True", "YES" }, { "False", "NO" } };
            string needsAccessiblity = userAdvancedSearch == null ? "NO" : yesNoChoices[userAdvancedSearch.NeedsAccessibility.ToString()];
            string numOccupants = userAdvancedSearch == null ? "Not specified" : userAdvancedSearch.MaxOccupancy.ToString();
            string rvLength = userAdvancedSearch == null ? "N/A" : userAdvancedSearch.RequiredRVLength.ToString();
            string needsUtilityHookup = userAdvancedSearch == null ? "N/A" : yesNoChoices[userAdvancedSearch.NeedsUtilityHookup.ToString()];

            Console.WriteLine($" NEW RESERVATION\n PARK: {GetParkNameFromCampsite(site)}\n" +
                $" CAMPGROUND: {GetCampgroundNameFromCampsite(site)}\n" +
                $" SITE NO.{site.SiteNumber}\n" +
                $" FROM: {userBasicSearch.StartDate.ToShortDateString()} TO {userBasicSearch.EndDate.Date.ToShortDateString()}\n" +
                $" ACCOMODATIONS\n\t ACCESSIBILITY: {needsAccessiblity}\n\t" +
                $" NUMBER OF OCCUPANTS: {numOccupants}\n\t" +
                $" NEEDS RV LENGTH OF: {rvLength}\n\t" +
                $" RV UTILITY HOOKUP: {needsUtilityHookup}\n" +
                $" TOTAL COST: {CalculateTotalCostOfStay(site.CampgroundID).ToString("C")}\n\n");

            // Prompt user to enter family name
            string familyName = CLIHelper.GetString(" Please enter your family name for the reservation: >> ");

            // Create new reservation
            Reservation userReservation = new Reservation();
            userReservation.SiteID = site.SiteID;
            userReservation.StartDate = userBasicSearch.StartDate;
            userReservation.EndDate = userBasicSearch.EndDate;
            userReservation.Name = familyName;
            userReservation.DateReserved = DateTime.Now;

            // Book reservation
            ReservationSqlDAL reservationDAL = new ReservationSqlDAL(databaseConnection);
            int confirmationNum = reservationDAL.BookReservation(userReservation);
            Console.WriteLine($"\n Congratulations! A reservation has been successfully booked for {userReservation.Name}.");
            Console.WriteLine($" Confirmation Id#: {confirmationNum}\n");
            Console.ReadLine();
        }
    }
}