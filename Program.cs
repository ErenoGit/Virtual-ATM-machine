using System;
using System.Data.SQLite;

namespace VirtualATMMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            InitialisationSqliteFile();

            Console.WriteLine("------------------------");
            Console.WriteLine("---Virtual ATM---");
            Console.WriteLine("------------------------");


            Console.WriteLine("Enter 1 to login to your virtual account.");
            Console.WriteLine("Enter 2 to create a new virtual account.");

            string choise = Console.ReadLine();

            if(choise == "1")
            {
                Login();
            }
            else if(choise == "2")
            {
                CreateAccount();
            }
            else
                Console.WriteLine("Invalid value. End of program.");

            Console.WriteLine("End of program. Press anything to close.");
            Console.ReadKey();
        }

        static void CreateAccount()
        {
            string accountNumber = RandomAccountNumberGenerator();
            int bonus = 0;
            Random rnd = new Random();
            int rand = rnd.Next(1,10);
            if (rand < 1)
            {
                bonus = 100;
                Console.WriteLine("Congratulations, You just win 100zł!");
            }
            else
            {
                bonus = 10;
                Console.WriteLine("Congratulations, You just win 10zł!");
            }

            int pin;
            do
            {
                Console.WriteLine("Enter your account PIN (4 digits):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            CreateNewAccount(accountNumber, pin, bonus);

            Console.WriteLine("A new account has been successfully created! Account number: " + accountNumber);
        }

        static void CreateNewAccount(string accountNumber, int pin, int balance)
        {
            using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
            {
                string sql = $"INSERT INTO accounts(accountNumber, pin, balance) VALUES(@param1,@param2,@param3);";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                command.Parameters.Add(new SQLiteParameter("@param2", pin));
                command.Parameters.Add(new SQLiteParameter("@param3", balance));
                command.ExecuteNonQuery();
            }
        }

        static string RandomAccountNumberGenerator()
        {
            Random random = new Random();
            return random.Next(10000000, 99999999).ToString();
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

            Console.Clear();
            Console.WriteLine("Welcome to your account number: " + accountNumber);
            Console.WriteLine("Your account balance is: " + GetBalance(accountNumber));
        }
        
        static void InitialisationSqliteFile()
        {
            if (!System.IO.File.Exists("database.sqlite"))
            {
                SQLiteConnection.CreateFile("database.sqlite");

                using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
                {
                    string sql = "CREATE TABLE accounts (accountNumber VARCHAR(8), pin INT, balance INT)";
                    SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                    command.ExecuteNonQuery();
                }
            }
        }

        static bool CheckIsAccountExist(string accountNumber)
        {
            bool isExist = false;

            using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
            {
                string sql = $"SELECT * FROM accounts WHERE accountNumber = @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    isExist = true;
            }

            return isExist;
        }

        static int GetPIN(string accountNumber)
        {
            int pin = 0;

            using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
            {
                string sql = $"SELECT pin FROM accounts WHERE accountNumber= @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                pin = Int32.Parse(reader["pin"].ToString());
            }

            return pin;
        }

        static int GetBalance(string accountNumber)
        {
            int pin = 0;

            using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
            {
                string sql = $"SELECT balance FROM accounts WHERE accountNumber = @param1";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                pin = Int32.Parse(reader["balance"].ToString());
            }

            return pin;
        }

        static void SetBalance(string accountNumber, int stanKonta)
        {
            using (var sqlite2 = new SQLiteConnection("Data Source=database.sqlite"))
            {
                string sql = $"UPDATE accounts SET balance = @param1 WHERE accountnumber = @param2";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.Parameters.Add(new SQLiteParameter("@param1", stanKonta));
                command.Parameters.Add(new SQLiteParameter("@param2", accountNumber));
                command.ExecuteNonQuery();
            }
        }


    }
}
