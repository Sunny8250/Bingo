using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.DB
{
    public interface IDataService
    {
        Task<DataTable> Execute(string storedProcedureName, DbParameter queryParam, CommandType commandType);
    }
}
