using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bankomat
{
    class Program
    {
        static void Main(string[] args)
        {
            InicjalizacjaPlikuSqlite();

            Console.WriteLine("------------------------");
            Console.WriteLine("---Wirtualny bankomat---");
            Console.WriteLine("------------------------");


            Console.WriteLine("Wpisz 1 aby zalogować się do swojego wirtualnego konta.");
            Console.WriteLine("Wpisz 2 aby stworzyć nowe wirtualne konto.");

            string wybor = Console.ReadLine();

            if(wybor == "1")
            {
                Logowanie();
            }
            else if(wybor == "2")
            {
                StworzKonto();
            }
            else
                Console.WriteLine("Niepoprawna wartość. Koniec programu.");

            Console.WriteLine("Koniec programu. Naciśnij cokolwiek aby zamknąć.");
            Console.ReadKey();
        }

        static void StworzKonto()
        {
            string numerKonta = GeneratorLosowegoNumeruKonta();
            int pin;
            do
            {
                Console.WriteLine("Wpisz PIN do swojego konta (4 cyfry):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            DodajNoweKonto(numerKonta, pin, 0);

            Console.WriteLine("Pomyślnie stworzono nowe konto! Numer konta: " + numerKonta);
        }

        static void DodajNoweKonto(string accountNumber, int pin, int balance)
        {
            var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
            sqlite2.Open();
            string sql = $"INSERT INTO accounts(accountNumber, pin, balance) VALUES('{accountNumber}',{pin},{balance});";
            SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
            command.ExecuteNonQuery();
            sqlite2.Close();
        }

        static string GeneratorLosowegoNumeruKonta()
        {
            Random random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }

       

        static void Logowanie()
        {
            string numerKonta;
            int pin;
            bool czyKontoIstnieje = false;
            bool czyPINJestDobry = false;

            do
            {
                Console.WriteLine("Wpisz numer swojego konta:");
                numerKonta = Console.ReadLine();
                czyKontoIstnieje = SprawdzCzyKontoIstnieje(numerKonta);
            } while (!czyKontoIstnieje);

            do
            {
                Console.WriteLine("Wpisz PIN do swojego konta (4 cyfry):");
            }
            while (!Int32.TryParse(Console.ReadLine(), out pin) || pin.ToString().Length != 4);

            czyPINJestDobry = PobierzPIN(numerKonta) == pin;

            if (!czyPINJestDobry)
            {
                Console.WriteLine("PIN jest niepoprawny!");
                return;
            }

            Console.Clear();
            Console.WriteLine("Witaj na swoim koncie o numerze: "+ numerKonta);
            Console.WriteLine("Twój stan konta to: "+ PobierzStanKonta(numerKonta));
        }

        static void InicjalizacjaPlikuSqlite()
        {
            if (!System.IO.File.Exists("database.sqlite"))
            {
                SQLiteConnection.CreateFile("database.sqlite");

                var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
                sqlite2.Open();
                string sql = "CREATE TABLE accounts (accountNumber VARCHAR(8), pin INT, balance INT)";
                SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
                command.ExecuteNonQuery();
                sqlite2.Close();
            }
        }

        static bool SprawdzCzyKontoIstnieje(string numerKonta)
        {
            bool czyIstnieje = false;

            var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
            sqlite2.Open();
            string sql = $"SELECT * FROM accounts WHERE accountNumber = '{numerKonta}'";
            SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                czyIstnieje = true;
            sqlite2.Close();

            return czyIstnieje;
        }

        static int PobierzPIN(string numerKonta)
        {
            int pin = 0;

            var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
            sqlite2.Open();
            string sql = $"SELECT pin FROM accounts WHERE accountNumber='{numerKonta}'";
            SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            pin = Int32.Parse(reader["pin"].ToString());
            sqlite2.Close();

            return pin;
        }

        static int PobierzStanKonta(string accountNumber)
        {
            int pin = 0;

            var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
            sqlite2.Open();
            string sql = $"SELECT balance FROM accounts WHERE accountNumber = '{accountNumber}'";
            SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            pin = Int32.Parse(reader["balance"].ToString());
            sqlite2.Close();

            return pin;
        }

        static void UstawStanKonta(string numerKonta, int stanKonta)
        {
            var sqlite2 = new SQLiteConnection("Data Source=database.sqlite");
            sqlite2.Open();
            string sql = $"UPDATE accounts SET balance = {stanKonta} WHERE accountnumber = '{numerKonta}'";
            SQLiteCommand command = new SQLiteCommand(sql, sqlite2);
            command.ExecuteNonQuery();
            sqlite2.Close();
        }


    }
}
