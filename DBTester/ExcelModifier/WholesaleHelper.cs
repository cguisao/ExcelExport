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

        public bool isAzImporter(string sku)
        {
            azImporterSku = "";
            string internalSku = "";
            int result = -5;
            for (int i = 0; i < sku.Length; i++)
            {
                if (sku[i] == ' ')
                {
                    if (azImportQuantity.ContainsKey(internalSku))
                    {
                        azImportQuantity.TryGetValue(internalSku, out result);
                        azImporterSku = internalSku;
                        return true;
                    }
                    else
                    {
                        internalSku = internalSku + sku[i];
                    }
                }
                else
                {
                    internalSku = internalSku + sku[i];
                }
            }

            if (azImportQuantity.ContainsKey(internalSku))
            {
                azImportQuantity.TryGetValue(internalSku, out result);
                azImporterSku = internalSku;
                return true;
            }
            else
            {
                return false;
            }
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
                if (v[i] != ' ')
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

        public bool isWeightRegister()
        {
            var weight = 0;
            double WeightPrice = -1;
            azImporterWeightSku.TryGetValue(azImporterSku, out weight);
            ShippingtWeight.TryGetValue(weight, out WeightPrice);
            AzImporterRegisterWeight = weight;

            if (WeightPrice > 1)
            {
                AzImporterPriceWeight = WeightPrice;
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

            azImportPrice.TryGetValue(azImporterSku, out sellingPrice);

            if (sellingPrice == 0)
            {
                return 0.0;
            }

            // AzImporter Fee
            summer = sellingPrice + 2;

            // EA Group Fee
            if (sellingPrice <= 15)
            {
                summer = summer + 1;
            }
            else
            {
                summer = summer + (summer * 5) / 100;
            }

            // profit 20% by default
            summer = summer + (summer * 20) / 100;

            // shipping
            summer = summer + AzImporterPriceWeight;

            // Amazon Fee 20%
            summer = summer + (summer * 20) / 100;

            return summer;
        }
    }
}
