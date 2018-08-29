using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelModifier
{
    interface IWholesaleHelper
    {
        bool isAzImporter(string sku);

        bool isFragrancex(long? innerItem);

        long? DigitGetter(string v);

        double getSellingPrice();
    }
}
