using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace VirtualATMMachine.Tests
{
    public class SQLiteConnectorTests
    {
        string testDatabaseLocation = "testDatabase.sqlite";

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
        [TestCase("12345678", 1234, 50)]
        [TestCase("83764094", 9204, 150)]
        [TestCase("33840293", 0014, 9999999)]
        public void TestCreateNewAccount(string testAccountNumber, int testPIN, int startBalance)
        {
            databaseConnector.CreateNewAccount(testAccountNumber, testPIN, startBalance);
            bool isExist = databaseConnector.CheckIsAccountExist(testAccountNumber);
            Assert.IsTrue(isExist);
            int balance = databaseConnector.GetBalance(testAccountNumber);
            Assert.IsTrue(balance == startBalance);
        }

        [Test]
        [TestCase("12345678", 1234, 99)]
        [TestCase("83764094", 9204, 9999999)]
        [TestCase("33840293", 0014, 1)]
        public void TestSetBalance(string testAccountNumber, int testPIN, int newBalance)
        {
            databaseConnector.SetBalance(testAccountNumber, newBalance);
            int balance = databaseConnector.GetBalance(testAccountNumber);
            Assert.IsTrue(balance == newBalance);
        }

        [Test]
        [TestCase("12345678", 1234)]
        [TestCase("83764094", 9204)]
        [TestCase("33840293", 0014)]
        public void TestGetPIN(string testAccountNumber, int testPIN)
        {
            databaseConnector.GetPIN(testAccountNumber);
            int pin = databaseConnector.GetPIN(testAccountNumber);
            Assert.IsTrue(pin == testPIN);
        }
    }
}