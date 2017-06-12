using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.ProjectCLI
{
    public class CLIHelper
    {
        public static DateTime GetDateTime(string message)
        {
            string userInput = String.Empty;
            DateTime dateValue = DateTime.MinValue;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid date format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!DateTime.TryParse(userInput, out dateValue));
            DisplayHeader();
            return dateValue;
        }

        public static int GetInteger(string message, List<int> availableChoices)
        {
            string userInput = String.Empty;
            int intValue = 0;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input. Please try again");
                }

                Console.Write(message);

                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!int.TryParse(userInput, out intValue) || !availableChoices.Contains(intValue));

            return intValue;
        }

        public static int GetInteger(string message)
        {
            string userInput = String.Empty;
            int intValue = 0;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input. Please try again");
                }

                Console.Write(message);

                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!int.TryParse(userInput, out intValue));

            return intValue;
        }

        public static double GetDouble(string message)
        {
            string userInput = String.Empty;
            double doubleValue = 0.0;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!double.TryParse(userInput, out doubleValue));

            return doubleValue;

        }

        public static bool GetBool(string message)
        {
            string userInput = String.Empty;
            bool boolValue = false;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("Invalid input format. Please try again");
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;
            }
            while (!bool.TryParse(userInput, out boolValue));

            return boolValue;
        }

        public static string GetString(string message, List<string> availableChoices, bool isCaseSensitive = false)
        {
            string userInput = String.Empty;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.Write("\n Invalid input. Please try again. ");
                    Console.ReadLine();
                    DisplayHeader();
                }

                Console.Write(message);

                userInput = isCaseSensitive ? Console.ReadLine() : Console.ReadLine().ToUpper();
                numberOfAttempts++;
            }
            while (String.IsNullOrEmpty(userInput) || !availableChoices.Contains(userInput));

            return userInput;
        }

        public static string GetString(string message)
        {
            string userInput = String.Empty;
            int numberOfAttempts = 0;

            do
            {
                if (numberOfAttempts > 0)
                {
                    Console.WriteLine("\n Invalid input. Please try again. ");
                    Console.ReadLine();
                }

                Console.Write(message + " ");
                userInput = Console.ReadLine();
                numberOfAttempts++;

                numberOfAttempts++;
            }
            while (String.IsNullOrEmpty(userInput));

            return userInput;
        }

        public static string FormatParagraph(string message)
        {
            int maxNumCharPerLine = 80;
            string[] words = message.Split(' ');

            string line = "".PadLeft(5, ' ');
            StringBuilder paragraph = new StringBuilder();
            foreach (string word in words)
            {
                if (String.Concat(line, word).Length > maxNumCharPerLine)
                {
                    paragraph.AppendLine(line);
                    line = "".PadLeft(5, ' ');
                }

                line = String.Concat(line, word, " ");
            }

            if (line != "")
            {
                paragraph.AppendLine(line);
            }
            return paragraph.ToString();
        }

        public static void DisplayHeader()
        {
            Console.Clear();
            Console.WriteLine("    //\\\\   _ _______ _ ");
            Console.WriteLine("   ////\\\\ /___________\\    ___________");
            Console.WriteLine("  /////\\\\\\|---_____---|   |========\\___\\");
            Console.WriteLine(" //////\\\\\\\\- |     |--|   |_______||__=_|");
            Console.WriteLine("    |__|  |--|     |--|   (o) (o) (o) (o)");
            Console.WriteLine("".PadRight(50, '*'));
            Console.WriteLine("* NATIONAL PARK CAMPSITE RESERVATION SYSTEM *");
            Console.WriteLine("".PadRight(50, '*') + "\n");
        }
    }
}
