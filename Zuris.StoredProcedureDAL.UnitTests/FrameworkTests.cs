using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using System;
using System.IO;

namespace Zuris.SPDAL.UnitTests
{
    [TestClass]
    public class FrameworkTests
    {
        public const string ConnectionString = "Data Source=.;Initial Catalog=TestingForRob;Integrated Security=True;MultipleActiveResultSets=True";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            AddProcedures("__ProcWithTableAndXmlParams", "__MultiResultSetTest", "__ProcWithOutputParam", "__SingleResultSetTest",
                "__ExecuteNonQueryWithOptionalError", "__ScalarProcWithReturn", "__ScalarProcWithSelect");
        }

        [ClassCleanup]
        public static void Finalize()
        {
            DropProcedures("__ProcWithTableAndXmlParams");
            DropTableTypes("__CountryType");
        }

        [TestMethod]
        public void TestTableAndXmlParametersProcedure()
        {
            using (var dataManager = new SampleDataManager(ConnectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var countries = new DataTable();
                countries.Columns.Add("Code"); countries.Columns.Add("Name");
                var row = countries.NewRow(); row["Code"] = "US"; row["Name"] = "United States"; countries.Rows.Add(row);
                row = countries.NewRow(); row["Code"] = "CA"; row["Name"] = "Canada"; countries.Rows.Add(row);

                var tblXmlCmd = new TableAndXmlParamTest(cdp);
                tblXmlCmd.Parameters.Countries.Value = countries;
                tblXmlCmd.Parameters.Xml.Value = "<stuff><x>Language</x><x>English</x></stuff>";
                var results = tblXmlCmd.ExecuteIntoList();

                foreach (var record in results)
                {
                    Console.WriteLine(new StringBuilder()
                        .Append(record.Code).Append(", ")
                        .Append(record.Name).Append(", ")
                        .Append(record.MyXml));
                }
            }
        }

        [TestMethod]
        public void TestDynamicTableAndXmlParametersProcedure()
        {
            using (var dataManager = new SampleDataManager(ConnectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var countries = new DataTable();
                countries.Columns.Add("Code"); countries.Columns.Add("Name");
                var row = countries.NewRow(); row["Code"] = "US"; row["Name"] = "United States"; countries.Rows.Add(row);
                row = countries.NewRow(); row["Code"] = "CA"; row["Name"] = "Canada"; countries.Rows.Add(row);

                var dynamicCmd = new DynamicProcedure(cdp, CommandType.StoredProcedure, "dbo.__ProcWithTableAndXmlParams");
                dynamicCmd.Parameters.Add("@countries", countries);
                dynamicCmd.Parameters.Add("@xml", "<stuff><x>Language</x><x>Endglish</x></stuff>", DbType.Xml);
                
                var results = dynamicCmd.ExecuteIntoList<CountryAndXml>();

                foreach (var record in results)
                {
                    System.Console.WriteLine(new StringBuilder()
                        .Append(record.Code).Append(", ")
                        .Append(record.Name).Append(", ")
                        .Append(record.MyXml));
                }
            }
        }

        [TestMethod]
        public void TestSingleResultSetProcedure()
        {
            using (var dataManager = new SampleDataManager(ConnectionString))
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
        public void TestScalarWithSelectProcedure()
        {
            using (var dataManager = new SampleDataManager(ConnectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var resultSetCommand = new ScalarWithSelect(cdp);
                resultSetCommand.Parameters.Id.Value = 1234;
                var returnValue = resultSetCommand.ExecuteScalar<int>();

                // This works too, but it executes into a dataset and pulls the first value
                //var returnValue = resultSetCommand.ExecuteScalarFromResultSet<int>();

                Assert.AreEqual(returnValue, 1234 + 100);
            }
        }

        [TestMethod]
        public void TestScalarWithReturnProcedure()
        {
            using (var dataManager = new SampleDataManager(ConnectionString))
            {
                var cdp = new SampleCommandDataProvider(dataManager);

                var resultSetCommand = new ScalarWithReturn(cdp);
                resultSetCommand.Parameters.Id.Value = 1234;
                resultSetCommand.ExecuteNonQuery();

                var returnValue = resultSetCommand.Parameters.ReturnValue.Value;

                Assert.AreEqual(returnValue, 1234 + 100);
            }
        }

        [TestMethod]
        public void TestMultiResultSetProcedure()
        {
            // The dataManager is a class that you override, one for each database you have. If you have 2 databases, you would have 2 implementations of DataManager. 
            // This is a custom datamanager class that takes the connection string in the constructor.
            using (var dataManager = new SampleDataManager(ConnectionString))
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

        private static void AddProcedures(params string[] procedureNames)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var procResources = assembly.GetManifestResourceNames().Where(r => r.StartsWith("Zuris.SPDAL.UnitTests.Sql") && procedureNames.Any(pn => r.EndsWith(pn + ".sql"))).ToList();
            foreach (var resourceName in procResources)
            {
                using (var dataManager = new SampleDataManager(ConnectionString))
                {
                    string sql;
                    using (var procStream = assembly.GetManifestResourceStream(resourceName))
                    using (var reader = new StreamReader(procStream))
                    {
                        sql = reader.ReadToEnd();
                    }

                    foreach (var commandSql in sql.Split(new string[] { "\r\nGO", "\r\ngo" }, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)))
                    {
                        var cmd = dataManager.CreateCommand();
                        cmd.CommandText = commandSql.Trim();
                        dataManager.ExecuteNonQuery(cmd);
                        Console.WriteLine("Added " + resourceName);
                    }
                }
            }
        }
        private static void DropProcedures(params string[] procedureNames)
        {
            foreach (var procedureName in procedureNames)
            {
                string dropSql =
@"IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + procedureName + @"]') AND type IN ( N'P', N'PC' )) 
	drop procedure [dbo].[" + procedureName + @"];";

                using (var dataManager = new SampleDataManager(ConnectionString))
                {
                    var cmd = dataManager.CreateCommand();
                    cmd.CommandText = dropSql;
                    dataManager.ExecuteNonQuery(cmd);
                    Console.WriteLine("Dropped " + procedureName);
                }
            }
        }
        private static void DropTableTypes(params string[] tableTypes)
        {
            foreach (var tableType in tableTypes)
            {
                string dropSql =
@"IF not EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = '"+ tableType+@"') 
	drop type [dbo].[" + tableType + @"];";

                using (var dataManager = new SampleDataManager(ConnectionString))
                {
                    var cmd = dataManager.CreateCommand();
                    cmd.CommandText = dropSql;
                    dataManager.ExecuteNonQuery(cmd);
                    Console.WriteLine("Dropped " + tableType);
                }
            }
        }
    }
}