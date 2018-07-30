using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class Database : IDatabase
    {
        public void ColumnMaker(DataTable upcTable, string columnName, string type)
        {
            DataColumn item = new DataColumn();
            item.DataType = System.Type.GetType(type);
            item.ColumnName = columnName;
            upcTable.Columns.Add(item);
        }

        public void upload(DataTable dataTable, int bulkSize, string tableName)
        {

            {
                var builder = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables();

                IConfiguration Configuration;
                builder.AddEnvironmentVariables();
                Configuration = builder.Build();
                string connectionstring = Configuration.GetConnectionString("BloggingDatabase");

                using (SqlConnection sourceConnection =
                       new SqlConnection(connectionstring))
                {
                    sourceConnection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionstring))
                    {
                        bulkCopy.DestinationTableName = tableName;

                        // Set the BatchSize.
                        bulkCopy.BatchSize = bulkSize;

                        try
                        {
                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(dataTable);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}