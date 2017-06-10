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
            CLIHelper.DisplayHeader();

            // Present user with options and record his or her selection
            string message = " Welcome! Please select from among the following options:\n [1] Search campsite by park" +
                "\n [2] Search campsite by campground\n [Q] Quit\n\n >> ";
            List<string> answerChoices = new List<string> { "1", "2", "Q" };
            bool isCaseSensitive = false;
            string userChoice = CLIHelper.GetString(message, answerChoices, isCaseSensitive);

            // Retrieve list of campsites meeting search criteria
            Park selectedPark = new Park();
            List<Campsite> campsMeetingSearchCriteria = new List<Campsite>();
 
            switch (userChoice)
            {
                case Command_SearchCampsiteByPark:
                    selectedPark = SelectPark();
                    campsMeetingSearchCriteria = CampsitesInParkSearchLoop(selectedPark.ParkID);
                    ReservationLoop(campsMeetingSearchCriteria);
                    Console.WriteLine("Thank you!");
                    break;

                case Command_SearchCampsiteByCampground:
                    selectedPark = SelectPark();
                    Campground selectedCampground = SelectCampground(selectedPark.ParkID);
                    campsMeetingSearchCriteria = CampsitesInCampgroundSearchLoop(selectedCampground.CampgroundID);
                    ReservationLoop(campsMeetingSearchCriteria);
                    Console.WriteLine("Thank you!");
                    break;

                case Command_Quit:
                    Console.WriteLine("Thank you and come again soon!");
                    return;
            }
        }

        public void ReservationLoop(List<Campsite> campsMeetingSearchCriteria)
        {
            ReservationSqlDAL reservationDAL = new ReservationSqlDAL(databaseConnection);
            while (true)
            {
                CLIHelper.DisplayHeader();
                Campsite campToBook = SelectCampsite(campsMeetingSearchCriteria);
                bool isCampAvailable = IsAvailableForReservation(campToBook);

                if (!isCampAvailable)
                {
                    string message = $" We're sorry; those dates are not currently available for campsite #{campToBook.SiteNumber}.\n" +
                        " [1] Select another date range\n [2] Select alternate campsite\n [3] Return to main menu >> ";
                    int userChoice = CLIHelper.GetInteger(message, new List<int> { 1, 2, 3 });
                    if (userChoice == 1)
                    {
                        CLIHelper.DisplayHeader();
                        int numDaysAhead = CLIHelper.GetInteger(" View upcoming reservations over the next __ days. >> ");
                        List<Reservation> upcomingReservations = reservationDAL.GetUpcomingReservations(DateTime.Now, DateTime.Now.AddDays(numDaysAhead), campToBook);
                        Console.WriteLine("There are " + upcomingReservations.Count + " upcoming reservations in that timeframe.");
                        foreach (Reservation r in upcomingReservations)
                        {
                            Console.WriteLine(" " + r.ToString());
                        }
                        Console.ReadLine();
                        userBasicSearch = RunBasicSearch(campToBook.SiteID);
                    }
                    else if (userChoice == 3)
                    {
                        break;
                    }
                }
                else
                {
                    RequestReservation(campToBook);
                    break;
                }
            }
        }

        public Campsite SelectCampsite(List<Campsite> campsMeetingSearchCriteria)
        {
            // Display header
            Console.WriteLine(" CAMPSITES");

            // Display information for each campsite meeting search criteria
            int currentCampgroundID = 0;
            for (int i = 1; i <= campsMeetingSearchCriteria.Count; i++)
            {
                if (currentCampgroundID != campsMeetingSearchCriteria[i - 1].CampgroundID)
                {
                    currentCampgroundID = campsMeetingSearchCriteria[i - 1].CampgroundID;
                    Console.WriteLine($"\n CAMPGROUND #{currentCampgroundID} | TOTAL COST OF STAY FROM " +
                        $"{userBasicSearch.StartDate.ToShortDateString()} TO {userBasicSearch.EndDate.ToShortDateString()}: " +
                        $"{CalculateTotalCost(currentCampgroundID).ToString("C")}");
                    Console.WriteLine(" Site Number".PadRight(15) + "Max Occupancy".PadRight(20) +
                        "Accessible".PadRight(20) + "Max RV Length".PadRight(20) + "Utilities");
                }
                Console.WriteLine($" [{i}] {campsMeetingSearchCriteria[i - 1].ToString()}");
            }

            // Prompt user to select campsite
            List<int> availableChoices = Enumerable.Range(1, campsMeetingSearchCriteria.Count).ToList<int>();
            int userChoice = CLIHelper.GetInteger("\n Please select campsite. >> ", availableChoices);

            // Return selected campsite
            return campsMeetingSearchCriteria[userChoice - 1];
        }

        public bool IsAvailableForReservation(Campsite site)
        {
            ReservationSqlDAL r = new ReservationSqlDAL(databaseConnection);
            return r.IsCampsiteAvailableForReservation(userBasicSearch, site);
        }

        public void RequestReservation(Campsite site)
        {
            CLIHelper.DisplayHeader();

            // Display reservation information
            bool needsAccessiblity = userAdvancedSearch == null ? false : userAdvancedSearch.NeedsAccessibility;
            string numOccupants = userAdvancedSearch == null ? "Not specified" : userAdvancedSearch.MaxOccupancy.ToString();
            string rvLength = userAdvancedSearch == null ? "N/A" : userAdvancedSearch.RequiredRVLength.ToString();
            bool needsUtilityHookup = userAdvancedSearch == null ? false : userAdvancedSearch.NeedsUtilityHookup;

            Console.WriteLine($" RESERVATION FOR SITE #{site.SiteNumber}\n" +
                $" FROM {userBasicSearch.StartDate.ToShortDateString()} TO {userBasicSearch.EndDate.Date.ToShortDateString()}\n" +
                $" ACCOMODATIONS\n\t ACCESSIBILITY: {needsAccessiblity}\n\t" +
                $" NUMBER OF OCCUPANTS: {numOccupants}\n\t" +
                $" NEEDS RV LENGTH OF: {rvLength}\n\t" +
                $" RV UTILITY HOOKUP: {needsUtilityHookup}\n\n");
            
            // Prompt user to enter family name
            string familyName = CLIHelper.GetString(" Please enter your family name for the reservation: >> ");

            // Create new reservation
            Reservation userReservation = new Reservation();
            userReservation.SiteID = site.SiteID;
            userReservation.StartDate = userBasicSearch.StartDate;
            userReservation.EndDate = userBasicSearch.EndDate;
            userReservation.Name = familyName;

            // Book reservation
            ReservationSqlDAL reservationDAL = new ReservationSqlDAL(databaseConnection);
            int confirmationNum = reservationDAL.BookReservation(userReservation);
            Console.WriteLine("\n Congratulations! Reservation successfully booked!");
            Console.WriteLine($" Confirmation Id#: {confirmationNum}\n {userReservation.ToString()}");
            Console.ReadLine();
        }

        public List<Campsite> CampsitesInParkSearchLoop(int parkID)
        {
            List<Campsite> cs = new List<Campsite>();
            while (true)
            {
                cs = SearchForCampsitesInPark(parkID);
                if (cs.Count == 0)
                {
                    string warningMessage = "There were no campsites that met your search criteria.\n" +
                        "Would you like to try again? (Y/N) >> ";
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

        public List<Campsite> CampsitesInCampgroundSearchLoop(int campgroundID)
        {
            List<Campsite> cs = new List<Campsite>();
            while (true)
            {
                cs = SearchForCampsitesInCampground(campgroundID);
                if (cs.Count == 0)
                {
                    string warningMessage = "There were no campsites that met your search criteria.\n" +
                        "Would you like to try again? (Y/N) >> ";
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

        public Park SelectPark()
        {
            CLIHelper.DisplayHeader();

            ParkSqlDAL p = new ParkSqlDAL(databaseConnection);
            List<Park> AllParks = p.GetAllParks();

            // Print table header for information about all parks
            Console.WriteLine("Name".PadLeft(9).PadRight(25) + "Location".PadRight(20)
                + "Date Established".PadRight(25) + "Acres".PadRight(20)
                + "# Visitors Yearly");

            // Print tabular information for each park
            for (int i = 1; i <= AllParks.Count; i++)
            {
                Console.WriteLine($" [{i}] {AllParks[i - 1].ToString()}\n");
            }

            // Print park descriptions
            Console.WriteLine(" PARK DESCRIPTIONS\n");
            for (int i = 1; i <= AllParks.Count; i++)
            {
                Console.WriteLine(" " + AllParks[i - 1].Name.ToUpper());
                Console.WriteLine(CLIHelper.FormatParagraph(AllParks[i - 1].Description) + "\n");
            }

            // Store user park selection
            string message = "\n Please choose a park. >> ";
            List<int> availableChoices = Enumerable.Range(1, AllParks.Count).ToList<int>();
            int userSelection = CLIHelper.GetInteger(message, availableChoices);
            Park selectedPark = AllParks[userSelection - 1];

            return selectedPark;
        }

        public Campground SelectCampground(int parkID)
        {
            CLIHelper.DisplayHeader();

            CampgroundSqlDAL campgroundDAL = new CampgroundSqlDAL(databaseConnection);

            // Retrieve all campgrounds in selected park
            List<Campground> campgroundsInPark = campgroundDAL.GetAllCampgroundsFromPark(parkID);

            // Print header for campground information
            Console.WriteLine("Name".PadLeft(9).PadRight(35) + "Month Open".PadRight(20)
                + "Month Closed".PadRight(20) + "Daily Fee".PadRight(10));

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

        public List<Campsite> SearchForCampsitesInPark(int parkID)
        {
            // Create new CampsiteSqlDAL
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);

            // Conduct search and store basic search criteria
            List<ISearchObject> parkSearchCriteria = ConductCampsiteSearch(parkID);
            userBasicSearch = (BasicSearch)parkSearchCriteria[0];

            // If only basic search conducted, return campsites meeting basic criteria
            if (parkSearchCriteria.Count == 1)
            {
                return campsiteDAL.GetAllCampsitesFromPark(userBasicSearch);
            }

            // If advanced search conducted, return campsites meeting basic and advanced criteria
            userAdvancedSearch = (AdvancedSearchOptions)parkSearchCriteria[1];
            return campsiteDAL.GetAllCampsitesFromPark(userBasicSearch, userAdvancedSearch);
        }

        public List<Campsite> SearchForCampsitesInCampground(int campgroundID)
        {
            // Create new CampsiteSqlDAL and empty list of campsites
            CampsiteSqlDAL campsiteDAL = new CampsiteSqlDAL(databaseConnection);
            List<Campsite> campsitesMeetingCriteria = new List<Campsite>();

            // Conduct search and store basic search criteria
            List<ISearchObject> campgroundSearchCriteria = ConductCampsiteSearch(campgroundID);
            userBasicSearch = (BasicSearch)campgroundSearchCriteria[0];

            // If only basic search conducted, return campsites meeting basic criteria
            if (campgroundSearchCriteria.Count == 1)
            {
                return campsiteDAL.GetAllCampsitesFromCampground(userBasicSearch);
            }

            // If advanced search conducted, return campsites meeting basic and advanced criteria
            userAdvancedSearch = (AdvancedSearchOptions)campgroundSearchCriteria[1];

            return campsiteDAL.GetAllCampsitesFromCampground(userBasicSearch, userAdvancedSearch);
        }

        public List<ISearchObject> ConductCampsiteSearch(int locationID)
        {
            // Display header
            CLIHelper.DisplayHeader();

            // Initialize empty list of search objects
            List<ISearchObject> searches = new List<ISearchObject>();

            // Ask user if they want to perform a basic or advanced search and then record their choice
            string searchMessage = " Would you like to perform a basic or advanced search?\n" +
                " [1] Basic\n [2] Advanced\n\n >> ";
            string userSearchChoice = CLIHelper.GetString(searchMessage, new List<string> { "1", "2" });

            // Create basic search object and add to list
            BasicSearch basicSearch = RunBasicSearch(locationID);
            searches.Add(basicSearch);

            // Conduct advanced search if requested and add to list
            if (userSearchChoice == "2")
            {
                AdvancedSearchOptions advancedSearch = RunAdvancedSearch();
                searches.Add(advancedSearch);
            }

            return searches;
        }

        public BasicSearch RunBasicSearch(int locationID)
        {
            CLIHelper.DisplayHeader();

            // Messages
            string startDateMessage = " On what date would you like to begin your adventure at National" +
                " Park System?\n Please type in the format 'yyyy-mm-dd'.\n\n >> ";
            string endDateMessage = " On what date would you like to end your adventure?\n" +
                " Please type in the format 'yyyy-mm-dd'.\n\n >> ";

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

        public decimal CalculateTotalCost(int campgroundID)
        {
            CampgroundSqlDAL campgroundDAL = new CampgroundSqlDAL(databaseConnection);
            decimal dailyFee = campgroundDAL.GetDailyFee(campgroundID);
            int numDaysInStay = userBasicSearch.EndDate.Subtract(userBasicSearch.StartDate).Days;

            return dailyFee * numDaysInStay;
        }
    }
}
