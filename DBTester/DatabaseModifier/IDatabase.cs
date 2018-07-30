using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    interface IDatabase
    {
        void upload(DataTable dataTable, int bulkSize, string tableName);

        void ColumnMaker(DataTable upcTable, string columnName, string type);
    }
}
