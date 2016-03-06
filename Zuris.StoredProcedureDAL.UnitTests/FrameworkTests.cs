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
            string connectionString = "Data Source=.;Initial Catalog=TestingForRob;Integrated Security=True;MultipleActiveResultSets=True";
            using (var dataManager = new SampleDataManager(connectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var results1 = new List<FirstResultSetRecord>();
                var results2 = new List<SecondResultSetRecord>();

                
                var multiResultSetCommand = new MultiResultSetTest(cdp);
                multiResultSetCommand.Parameters.Id.Value = 112;


                // execute into multiple lists
                multiResultSetCommand.ExecuteIntoLists(out results1, out results2);

                foreach (var record in results1)
                {
                    System.Console.WriteLine("First::" + record.Id);
                }

                foreach (var record in results2)
                {
                    System.Console.WriteLine("Second::" + record.Id);
                }


                // execute into dataSet
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