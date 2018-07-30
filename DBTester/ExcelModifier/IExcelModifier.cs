using DBTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelModifier
{
    interface IExcelExtension
    {
        string sWebRootFolder { get; set; }

        Dictionary<int, double> prices { get; set; }

        Profile profile { get; set; }

        void ExcelGenerator();
    }
}
