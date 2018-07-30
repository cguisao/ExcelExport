using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseModifier
{
    interface IDatabaseModifier
    {
        DataTable CreateTable();

        void TableExecutor();
    }
}
