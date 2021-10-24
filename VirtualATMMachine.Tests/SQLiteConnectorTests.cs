using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace VirtualATMMachine.Tests
{
    public class SQLiteConnectorTests
    {
        string testDatabaseLocation = "testDatabase.sqlite";
        string testAccountNumber = "12345678";
        int testPIN = 1234;

        SQLiteConnector databaseConnector;

        [OneTimeSetUp]
        public void CreateSQLiteConnectorObject()
        {
            databaseConnector = new SQLiteConnector(testDatabaseLocation);
        }

        [OneTimeTearDown]
        public void DeleteTestDatabaseFile()
        {
            if (File.Exists(testDatabaseLocation))
            {
                File.Delete(testDatabaseLocation);
            }
        }

        [Test, Order(1)]
        public void TestInitialisationSqliteFile()
        {
            databaseConnector.InitialisationSqliteFile();
            Assert.IsTrue(File.Exists(testDatabaseLocation));
        }

        [Test, Order(2)]
        public void TestCreateNewAccount()
        {
            databaseConnector.CreateNewAccount(testAccountNumber, testPIN, 50);
            bool isExist = databaseConnector.CheckIsAccountExist(testAccountNumber);
            Assert.IsTrue(isExist);
            int balance = databaseConnector.GetBalance(testAccountNumber);
            Assert.IsTrue(balance == 50);
        }

        [Test]
        public void TestSetBalance()
        {
            databaseConnector.SetBalance(testAccountNumber, 300);
            int balance = databaseConnector.GetBalance(testAccountNumber);
            Assert.IsTrue(balance == 400);
        }

        [Test]
        public void TestGetPIN()
        {
            databaseConnector.GetPIN(testAccountNumber);
            int pin = databaseConnector.GetPIN(testAccountNumber);
            Assert.IsTrue(pin == 1234);
        }
    }
}