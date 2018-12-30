using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DBTester.Models;

namespace DatabaseModifier
{
    public class DBModifierShopifyUserList : Database, IDatabaseModifier
    {
        public DBModifierShopifyUserList(ConcurrentDictionary<string, string> _shopifyUser, Profile _profile)
        {
            shopifyUser = _shopifyUser;
            prof = _profile;
        }

        private ConcurrentDictionary<string, string> shopifyUser { get; set; }
        private Profile prof { set; get; }

        public DataTable CreateTable()
        {
            DataTable shopifyUserTable = new DataTable("UsersListTemp");

            ColumnMaker(shopifyUserTable, "ItemID", "System.Int32");
            ColumnMaker(shopifyUserTable, "sku", "System.String");
            ColumnMaker(shopifyUserTable, "userID", "System.String");

            return shopifyUserTable;
        }

        public void TableExecutor()
        {
            DataTable uploadShopifyUser = CreateTable();
            int bulkSize = 1;
            string item = string.Empty;

            try
            {
                foreach (var profile in shopifyUser)
                {
                    DataRow insideRow = uploadShopifyUser.NewRow();

                    insideRow["ItemID"] = bulkSize + 1;
                    insideRow["sku"] = profile.Key;
                    insideRow["userID"] = profile.Value;

                    uploadShopifyUser.Rows.Add(insideRow);
                    uploadShopifyUser.AcceptChanges();
                    bulkSize++;
                }

                upload(uploadShopifyUser, bulkSize, "dbo.UsersListTemp");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
