using ExcelModifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    public class WholesaleHelper : IWholesaleHelper
    {
        public string azImporterSku { get; set; }

        public int AzImporterRegisterWeight { get; set; }

        public Dictionary<string, int> azImporterWeightSku { get; set; }

        public Dictionary<string, int> azImportQuantity { get; set; }

        public Dictionary<int, double> ShippingtWeight { get; set; }

        public Dictionary<string, double> azImportPrice { get; set; }

        public double AzImporterPriceWeight { get; set; }

        public double AzImporterWeight { get; set; }

        public Dictionary<string, bool> blackListed { get; set; }

        public bool isAzImporter(string sku)
        {
            int result = -5;
            string internalSku = sku.ToUpper();

            for (int i = 1; i < sku.Length; i++)
            {
                if (azImportQuantity.ContainsKey(internalSku.ToUpper()))
                {
                    azImportQuantity.TryGetValue(internalSku.ToUpper(), out result);
                    azImporterSku = internalSku.ToUpper();
                    return true;
                }
                else
                {
                    internalSku = sku.Substring(0, sku.Length - i);
                    //internalSku = internalSku + sku[i];
                }
            }

            return false;

            //azImporterSku = "";
            
            
            //for (int i = 0; i < sku.Length; i++)
            //{
            //    internalSku = internalSku.ToUpper();
            //    //if (sku[i] == ' ')
            //    //{
            //        if (azImportQuantity.ContainsKey(internalSku))
            //        {
            //            azImportQuantity.TryGetValue(internalSku, out result);
            //            azImporterSku = internalSku;
            //            return true;
            //        }
            //        else
            //        {
            //            internalSku = internalSku + sku[i];
            //        }
            //    //}
            //    //else
            //    //{
            //    //    internalSku = internalSku + sku[i];
            //    //}
            //}

            //internalSku = internalSku.ToUpper();

            //if (azImportQuantity.ContainsKey(internalSku))
            //{
            //    azImportQuantity.TryGetValue(internalSku, out result);
            //    azImporterSku = internalSku;
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public bool isFragrancex(long? innerItem)
        {
            Match hasLetters = Regex.Match(innerItem.ToString(), @"[a-zA-Z]");

            if (hasLetters.Success)
            {
                return false;
            }

            if (innerItem.ToString().Length != 6)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public long? DigitGetter(string v)
        {
            string answer = "";

            v.TrimStart();

            for (int i = 0; i < v.Length; i++)
            {
                if (answer.Length != 6)
                {
                    answer = answer + v[i];
                }
                else
                {
                    try
                    {
                        Convert.ToInt64(answer);
                        return Convert.ToInt64(answer);
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }
            }
            try
            {
                Convert.ToInt64(answer);
                return Convert.ToInt64(answer);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public bool isWeightRegister(double WeightPrice)
        {
            if (WeightPrice > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double getSellingPrice()
        {
            double sellingPrice = 0;

            double summer = 0.0;

            azImportPrice.TryGetValue(azImporterSku.ToUpper(), out sellingPrice);

            if (sellingPrice == 0)
            {
                return 0.0;
            }
            
            // EA Group Fee 20%
            
            summer = sellingPrice + (sellingPrice * 15) / 100;
            
            // profit 20% by default
            summer = summer + (sellingPrice * 20) / 100;

            // AzImporter Fee
            summer = summer + 2;

            // shipping
            summer = summer + AzImporterPriceWeight;

            // Amazon Fee 20%
            summer = summer + (summer * 18) / 100;

            return summer;
        }
    }
}
