using System;
using System.Data.SQLite;

namespace VirtualATMMachine
{
    public class SQLiteConnector
    {
            private static string databaseLocation { get; set; }
            private static string connectionString { get; set; }

            public SQLiteConnector(string _databaseLocation)
            {
                databaseLocation = _databaseLocation;
                connectionString = $"Data Source={_databaseLocation}";
            }


            public void InitialisationSqliteFile()
            {
                if (!System.IO.File.Exists(databaseLocation))
                {
                    SQLiteConnection.CreateFile(databaseLocation);

                    using (var sqlite2 = new SQLiteConnection(connectionString))
                    {
                        sqlite2.Open();
                        string sql = "CREATE TABLE accounts (accountNumber VARCHAR(8), pin INT, balance INT)";
                        using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                        {
                            command.ExecuteNonQuery();
                        }
                        sqlite2.Close();
                    }
                }
            }


            public void CreateNewAccount(string accountNumber, int pin, int balance)
            {
                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    string sql = $"INSERT INTO accounts(accountNumber, pin, balance) VALUES(@param1,@param2,@param3);";
                    using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                    {
                        command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                        command.Parameters.Add(new SQLiteParameter("@param2", pin));
                        command.Parameters.Add(new SQLiteParameter("@param3", balance));
                        command.ExecuteNonQuery();
                    }
                    sqlite2.Close();
                }
            }

            public bool CheckIsAccountExist(string accountNumber)
            {
                bool isExist = false;

                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    string sql = $"SELECT * FROM accounts WHERE accountNumber = @param1";
                    using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                    {
                        command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                isExist = true;
                        }
                    }
                    sqlite2.Close();
                }

                return isExist;
            }

            public int GetPIN(string accountNumber)
            {
                int pin = 0;

                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    string sql = $"SELECT pin FROM accounts WHERE accountNumber= @param1";
                    using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                    {
                        command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            pin = Int32.Parse(reader["pin"].ToString());
                        }
                    }
                    sqlite2.Close();
                }
                return pin;
            }

            public int GetBalance(string accountNumber)
            {
                int balance = 0;

                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    string sql = $"SELECT balance FROM accounts WHERE accountNumber = @param1";
                    using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                    {
                        command.Parameters.Add(new SQLiteParameter("@param1", accountNumber));
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            balance = Int32.Parse(reader["balance"].ToString());
                        }
                    }
                    sqlite2.Close();
                }

                return balance;
            }

            public void SetBalance(string accountNumber, int balance)
            {
                using (var sqlite2 = new SQLiteConnection(connectionString))
                {
                    sqlite2.Open();
                    string sql = $"UPDATE accounts SET balance = @param1 WHERE accountnumber = @param2";
                    using (SQLiteCommand command = new SQLiteCommand(sql, sqlite2))
                    {
                        command.Parameters.Add(new SQLiteParameter("@param1", balance));
                        command.Parameters.Add(new SQLiteParameter("@param2", accountNumber));
                        command.ExecuteNonQuery();
                    }
                    sqlite2.Close();
                }
            }
    }
}
