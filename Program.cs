using System;

namespace VirtualATMMachine
{
    class Program
    {
        static Random random = new Random();
        private static SQLiteConnector sqliteConnector;

        static void Main(string[] args)
        {
            string databaseLocation = "database.sqlite";

            if (args != null)
            {
                if (args.Length > 0)
                {
                    databaseLocation = args[0];
                    Console.WriteLine("Your database file location is: " + databaseLocation);
                }
            }

            sqliteConnector = new SQLiteConnector(databaseLocation);

            sqliteConnector.InitialisationSqliteFile();

            Console.WriteLine("------------------------");
            Console.WriteLine("------Virtual ATM-------");
            Console.WriteLine("------------------------");

            string choice;
            bool shouldReEnterChoice = true;

            do
            {
                Console.WriteLine("Enter 1 to login to your virtual account.");
                Console.WriteLine("Enter 2 to create a new virtual account.");

                choice = Console.ReadLine();

                if (choice == "1")
                {
                    Login();
                    shouldReEnterChoice = false;
                }
                else if (choice == "2")
                {
                    CreateAccount();
                    shouldReEnterChoice = false;
                }
                else
                    Console.WriteLine("Invalid value.");
            }
            while (shouldReEnterChoice);

            Console.WriteLine("End of program. Press anything to close.");
            Console.ReadKey();
        }

        static void CreateAccount()
        {
            int perCent = random.Next(0, 100);
            int startBalance = 0;

            string accountNumber = RandomAccountNumberGenerator();
            int pin;
            do
            {
                Console.WriteLine("Enter your account PIN (4 digits):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            if (perCent < 1)
            {
                startBalance = 100;
                Console.WriteLine("Congratulations, you just won start bonus 100 virtual units!");
            }
            else if (perCent < 5)
            {
                startBalance = 30;
                Console.WriteLine("Congratulations, you just won start bonus 30 virtual units!");
            }
            else if (perCent < 10)
            {
                startBalance = 10;
                Console.WriteLine("Congratulations, you just won start bonus 10 virtual units!");
            }

            sqliteConnector.CreateNewAccount(accountNumber, pin, startBalance);

            Console.WriteLine("A new account has been successfully created! Account number: " + accountNumber);
        }

        static void Login()
        {
            string accountNumber;
            int pin;
            bool isAccountExist = false;
            bool isPINValid = false;

            do
            {
                Console.WriteLine("Enter your account number:");
                accountNumber = Console.ReadLine();
                isAccountExist = sqliteConnector.CheckIsAccountExist(accountNumber);
            } while (!isAccountExist);

            do
            {
                Console.WriteLine("Enter your account PIN (4 digits):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            isPINValid = sqliteConnector.GetPIN(accountNumber) == pin;

            if (!isPINValid)
            {
                Console.WriteLine("The PIN is incorrect!");
                return;
            }

            Console.WriteLine("Welcome to your account number: " + accountNumber);
            Console.WriteLine("Your account balance is: " + sqliteConnector.GetBalance(accountNumber));

            string choice, howMuch;
            int howMuchInt;

            Console.WriteLine("Enter 1 to deposit money.");
            Console.WriteLine("Enter 2 to withdraw money.");
            Console.WriteLine("Enter 3 to logout.");

            choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("How much do you want to deposit?");
                howMuch = Console.ReadLine();
                if (!Int32.TryParse(howMuch, out howMuchInt))
                {
                    Console.WriteLine("You entered an incorrect value! You will be logged out.");
                }
                else
                {
                    sqliteConnector.SetBalance(accountNumber, sqliteConnector.GetBalance(accountNumber)+ howMuchInt);
                    Console.WriteLine("You deposited "+ howMuch);
                    Console.WriteLine("Your account balance is now: " + sqliteConnector.GetBalance(accountNumber));
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine("How much do you want to withdraw?");
                howMuch = Console.ReadLine();
                if (!Int32.TryParse(howMuch, out howMuchInt))
                {
                    Console.WriteLine("You entered an incorrect value! You will be logged out.");
                }
                else
                {
                    sqliteConnector.SetBalance(accountNumber, sqliteConnector.GetBalance(accountNumber) - howMuchInt);
                    Console.WriteLine("You withdrawed " + howMuch);
                    Console.WriteLine("Your account balance is now: " + sqliteConnector.GetBalance(accountNumber));
                }
            }
            else
                Console.WriteLine("Invalid value. You will be logged out.");
        }




        static string RandomAccountNumberGenerator()
        {
            string newRandomAccountNumber;
            bool isDuplicated = false;

            do
            {
                newRandomAccountNumber = random.Next(10000000, 99999999).ToString();
                isDuplicated = sqliteConnector.CheckIsAccountExist(newRandomAccountNumber);
            }
            while (isDuplicated);

            return newRandomAccountNumber;
        }

    }
}
