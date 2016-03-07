using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;

namespace Zuris.SPDAL.UnitTests
{
    [TestClass]
    public class FrameworkTests
    {
        [TestMethod]
        public void TestSingleResultSetProcedure()
        {
            string connectionString = "Data Source=.;Initial Catalog=TestingForRob;Integrated Security=True;MultipleActiveResultSets=True";
            using (var dataManager = new SampleDataManager(connectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var resultSetCommand = new SingleResultSetTest(cdp);
                resultSetCommand.Parameters.Name.Value = "Harry Buttz";
                var results = resultSetCommand.ExecuteIntoList();

                foreach (var record in results)
                {
                    System.Console.WriteLine(record.Name);
                }
            }
        }

        [TestMethod]
        public void TestMultiResultSetProcedure()
        {
            // The dataManager is a class that you override, one for each database you have. If you have 2 databases, you would have 2 implementations of DataManager. 
            // This is a custom datamanager class that takes the connection string in the constructor.
            using (var dataManager = new SampleDataManager("Data Source=.;Initial Catalog=TestingForRob;Integrated Security=True;MultipleActiveResultSets=True"))
            {
                // The ICommandDataProvider is a database indepenant interface (no requirement on databases in contract). This is the one we use for ADO.Net database access. 
                // I have used a custom implmenetation of this class that calls special web services that make proc calls. It would take the results (serialized datasets) and make 
                // them work with the API. You could switch direct database or web service access by changing dependancy injection setup.
                var cdp = new SampleCommandDataProvider(dataManager);

                // The two results we are getting back in this call
                var results1 = new List<FirstResultSetRecord>();
                var results2 = new List<SecondResultSetRecord>();

                // create the command object and set any parameters through strongly typed properties
                var multiResultSetCommand = new MultiResultSetTest(cdp);
                multiResultSetCommand.Parameters.Id.Value = 112;

                // execute into multiple lists (returns as out paramters)
                multiResultSetCommand.ExecuteIntoLists(out results1, out results2);

                foreach (var record in results1)
                {
                    System.Console.WriteLine("First::" + record.Id);
                }

                foreach (var record in results2)
                {
                    System.Console.WriteLine("Second::" + record.Id);
                }

                // You can also execute into a dataSet
                var dataSet = multiResultSetCommand.ExecuteIntoDataSet();
                foreach (DataTable dt in dataSet.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        System.Console.WriteLine("DataSet::" + dt.TableName + "::" + row[0]);
                    }
                }
            }
        }
    }
}