using System;
using System.Data.SQLite;

namespace VirtualATMMachine
{
    class Program
    {
        static Random random = new Random();
        static string databaseLocation = "database.sqlite";

        static void Main(string[] args)
        {
            if (args != null)
            {
                if (args.Length > 0)
                {
                    databaseLocation = args[0];
                    Console.WriteLine("Your database file location is: " + databaseLocation);
                }
            }

            InitialisationSqliteFile();

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

            CreateNewAccount(accountNumber, pin, startBalance);

            Console.WriteLine("A new account has been successfully created! Account number: " + accountNumber);
        }

        static void CreateNewAccount(string accountNumber, int pin, int balance)
        {
            using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
            {
                sqlite2.Open();
                string sql = $"INSERT INTO accounts(accountNumber, pin, balance) VALUES(@param1,@param2,@param3);";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                command.Parameters.Add(new SQLiteParameter("@param2", pin));
                command.Parameters.Add(new SQLiteParameter("@param3", balance));
                command.ExecuteNonQuery();
                sqlite2.Close();
            }
        }

        static string RandomAccountNumberGenerator()
        {
            string newRandomAccountNumber;
            bool isDuplicated = false;

            do
            {
                newRandomAccountNumber = random.Next(10000000, 99999999).ToString();
                isDuplicated = CheckIsAccountExist(newRandomAccountNumber);
            }
            while (isDuplicated);

            return newRandomAccountNumber;
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
                isAccountExist = CheckIsAccountExist(accountNumber);
            } while (!isAccountExist);

            do
            {
                Console.WriteLine("Enter your account PIN (4 digits):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            isPINValid = GetPIN(accountNumber) == pin;

            if (!isPINValid)
            {
                Console.WriteLine("The PIN is incorrect!");
                return;
            }

            Console.WriteLine("Welcome to your account number: " + accountNumber);
            Console.WriteLine("Your account balance is: " + GetBalance(accountNumber));

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
                    SetBalance(accountNumber, GetBalance(accountNumber)+ howMuchInt);
                    Console.WriteLine("You deposited "+ howMuch);
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
                    SetBalance(accountNumber, GetBalance(accountNumber) - howMuchInt);
                    Console.WriteLine("You withdrawed " + howMuch);
                }
            }
            else
                Console.WriteLine("Invalid value. You will be logged out.");
        }
        
        static void InitialisationSqliteFile()
        {
            if (!System.IO.File.Exists(databaseLocation))
            {
                SQLiteConnection.CreateFile(databaseLocation);

                using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
                {
                    sqlite2.Open();
                    string sql = "CREATE TABLE accounts (accountNumber VARCHAR(8), pin INT, balance INT)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                    command.ExecuteNonQuery();
                    sqlite2.Close();
                }
            }
        }

        static bool CheckIsAccountExist(string accountNumber)
        {
            bool isExist = false;

            using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
            {
                sqlite2.Open();
                string sql = $"SELECT * FROM accounts WHERE accountNumber = @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    isExist = true;
                sqlite2.Close();
            }

            return isExist;
        }

        static int GetPIN(string accountNumber)
        {
            int pin = 0;

            using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
            {
                sqlite2.Open();
                string sql = $"SELECT pin FROM accounts WHERE accountNumber= @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                pin = Int32.Parse(reader["pin"].ToString());
                sqlite2.Close();
            }
            return pin;
        }

        static int GetBalance(string accountNumber)
        {
            int balance = 0;

            using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
            {
                sqlite2.Open();
                string sql = $"SELECT balance FROM accounts WHERE accountNumber = @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                balance = Int32.Parse(reader["balance"].ToString());
                sqlite2.Close();
            }

            return balance;
        }

        static void SetBalance(string accountNumber, int balance)
        {
            using (var sqlite2 = new SQLiteConnection($"Data Source={databaseLocation}"))
            {
                sqlite2.Open();
                string sql = $"UPDATE accounts SET balance = @param1 WHERE accountnumber = @param2";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", balance));
                command.Parameters.Add(new SQLiteParameter("@param2", accountNumber));
                command.ExecuteNonQuery();
                sqlite2.Close();
            }
        }


    }
}
