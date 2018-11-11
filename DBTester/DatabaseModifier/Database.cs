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

                using (SqlConnection sourceConnection = new SqlConnection(connectionstring))
                {
                    sourceConnection.Open();

                    var command = sourceConnection.CreateCommand();
                    command.Connection = sourceConnection;
                    command.CommandText = "DELETE FROM " + tableName;
                    command.ExecuteNonQuery();

                    using (SqlTransaction trans = sourceConnection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionstring
                                , SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.UseInternalTransaction))
                            {
                                bulkCopy.DestinationTableName = tableName;

                                // Set the BatchSize.
                                bulkCopy.BatchSize = bulkSize;

                                try
                                {
                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dataTable);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    sourceConnection.Close();
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            trans.Rollback();
                            throw e;
                        }
                        
                    }
                    
                }
            }
        }
        
    }
}