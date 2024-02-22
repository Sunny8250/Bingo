using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.DB
{
    public class DataService : IDataService
    {
        private readonly DbContext _dbContext; 
        public DataService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DataTable> Execute(string storedProcedureName, DbParameter queryParam, CommandType commandType)
        {
            //Create a new instance of DataTable
            var dataTable = new DataTable();
            using (var command  = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedureName;
                command.CommandType = commandType;
                command.Parameters.Add(queryParam);
                _dbContext.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    //here result is dbDataReader which is result set returned from SP and we load it into the datatable
                    dataTable.Load(result);
                }
            }
            return dataTable;
        }
    }
}
