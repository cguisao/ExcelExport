using DBTester.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class DBModifierAmazon : Database, IDatabaseModifier
    {
        public DBModifierAmazon(List<Amazon> _amazonList)
        {
            amazonList = _amazonList;
        }

        private List<Amazon> amazonList { get; set; }

        public DataTable CreateTable()
        {
            DataTable amazonTable = new DataTable("Amazon");

            ColumnMaker(amazonTable, "id", "System.Int32");
            ColumnMaker(amazonTable, "Asin", "System.String");
            ColumnMaker(amazonTable, "price", "System.Double");
            ColumnMaker(amazonTable, "sku", "System.String");
            ColumnMaker(amazonTable, "wholesaler", "System.String");

            return amazonTable;
        }

        public void TableExecutor()
        {
            DataTable uploadAmazon = CreateTable();

            int bulkSize = 0;

            int exception = 0;

            int id = 1;

            foreach (Amazon item in amazonList)
            {
                exception++;

                DataRow insideRow = uploadAmazon.NewRow();

                insideRow["id"] = id;
                insideRow["Asin"] = item.Asin;
                insideRow["sku"] = item.sku;
                insideRow["price"] = item.price;
                insideRow["wholesaler"] = item.wholesaler;

                uploadAmazon.Rows.Add(insideRow);
                uploadAmazon.AcceptChanges();

                id++;
                bulkSize++;
            }

            upload(uploadAmazon, bulkSize, "dbo.Amazon");
        }
    }
}
