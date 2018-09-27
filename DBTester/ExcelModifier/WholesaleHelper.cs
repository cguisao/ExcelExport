using DBTester.Models;
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

        public Dictionary<int, double> ShippingList { get; set; }

        public double AzImporterPriceWeight { get; set; }

        public double AzImporterWeight { get; set; }

        public Dictionary<string, bool> blackListedList { get; set; }

        public Dictionary<int, Fragrancex> fragrancexList { get; set; }

        public Dictionary<string, AzImporter> azImporterList { get; set; }

        public Dictionary<string, PerfumeWorldWide> perfumeWorldWideList { get; set; }

        public bool isAzImporter(string sku)
        {
            string internalSku = sku.ToUpper();

            for (int i = 1; i < sku.Length; i++)
            {
                if (azImporterList.ContainsKey(internalSku.ToUpper()))
                {
                    AzImporter a = new AzImporter();
                    azImporterList.TryGetValue(internalSku.ToUpper(), out a);
                    azImporter = a;
                    return true;
                }
                else
                {
                    internalSku = sku.Substring(0, sku.Length - i);
                }
            }

            return false;
        }

        public bool isPerfumeWorldWide(string sku)
        {
            // Take care when it is in the dictionary, because it is faster

            string internalSku = sku;

            for (int i = 1; i < sku.Length; i++)
            {
                if (perfumeWorldWideList.ContainsKey(internalSku))
                {
                    return true;
                }
                else
                {
                    internalSku = sku.Substring(0, sku.Length - i);
                }
            }

            if (perfumeWorldWideList.ContainsKey(sku))
            {
                return true;
            }

            // Take care of sku where the first character is a letter and the second is a '-'

            Match hasLetterAndDash = Regex.Match(sku, @"^[a-zA-Z][-]");
            
            if(hasLetterAndDash.Success)
            {
                return true;
            }

            // Take care of sku where 3 first chars are letters follow by a number

            Match has3LettersAndNumber = Regex.Match(sku, @"^[a-zA-Z]{3}\d{1}");

            if(has3LettersAndNumber.Success)
            {
                return true;
            }

            // Take care of sku where the first 2 chars are letters follow by a -

            Match has3Letters = Regex.Match(sku, @"^[a-zA-Z]{2}[-]");

            if (has3Letters.Success)
            {
                return true;
            }

            // Take care of sku where the first 3 chars are letters follow by a dash

            Match has3LettersAndDash = Regex.Match(sku, @"^[a-zA-Z]{3}[-]");
            
            if(has3LettersAndDash.Success)
            {
                return true;
            }

            // Take care of sku where they begin with numbers 5-6 digits long

            int j = 0;
            
            if (Int32.TryParse(sku, out j))
            {
                if(j.ToString().Length == 6 || j.ToString().Length == 5)
                {
                    return true;
                }
            }

            return false;
        }

        public bool isFragrancex(long? innerItem)
        {
            Match hasLetters = Regex.Match(innerItem.ToString(), @"[a-zA-Z]");

            if (hasLetters.Success)
            {
                return false;
            }

            if (innerItem.ToString().Length != 6 || perfumeWorldWideList.ContainsKey(innerItem.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int DigitGetter(string v)
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
                        return Convert.ToInt32(answer);
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
                return Convert.ToInt32(answer);
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
            double summer = 0.0;

            if (azImporter.WholeSale == 0)
            {
                return 0.0;
            }

            double s = 0.0;
            ShippingList.TryGetValue(azImporter.Weight, out s);
            AzImporterPriceWeight = s;

            // EA Group Fee 20%

            summer = azImporter.WholeSale + (azImporter.WholeSale * 15) / 100;
            
            // profit 20% by default
            summer = summer + (azImporter.WholeSale * 20) / 100;

            // AzImporter Fee
            summer = summer + 2;

            // shipping
            summer = summer + AzImporterPriceWeight;

            // Amazon Fee 20%
            summer = summer + (summer * 18) / 100;

            return summer;
        }

        long? IWholesaleHelper.DigitGetter(string v)
        {
            throw new NotImplementedException();
        }

        public Fragrancex fragrancex { get; set; }

        public AzImporter azImporter { get; set; }

        public PerfumeWorldWide perfumeWorldWide { get; set; }
        
    }
}