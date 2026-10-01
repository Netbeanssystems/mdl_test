using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class DbHelper
    {
        public IConfiguration configuration;
        public DbHelper(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task<int> ExecuteStoredProcedureAndCheckRowsAffectedAsync(string procedureName, params SqlParameter[] parameters)
        {
            try
            {
                // Ensure configuration is available and properly initialized
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddRange(parameters);

                // Execute the command and return the number of rows affected
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<DataTable> ExecuteStoredProcedureAndGetDataAsync(string procedureName, params SqlParameter[] parameters)
        {
            // Ensure configuration is available and properly initialized
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            await using var reader = await command.ExecuteReaderAsync();

            var dataTable = new DataTable();
            dataTable.Load(reader);

            //foreach (var param in parameters)
            //{
            //    if (param.ParameterName == "@TableName")
            //    {
            //        if (param.Value != null && param.Value.ToString() == "NetUsers")
            //        {
            //            return await ConvertDataTableToListAsync<T>(dataTable);
            //        }
            //    }
            //}

            if (dataTable.Rows.Count == 0) return null;
            //if (dataTable.Rows.Count == 1) return await ConvertDataTableToSingleInstanceAsync<T>(dataTable);
            return dataTable;//await ConvertDataTableToListAsync<T>(dataTable);
        }

        public async Task<List<T>> ConvertDataTableToListAsync<T>(DataTable dataTable) where T : new()
        {
            return await Task.Run(() =>
            {
                var list = new List<T>();
                var properties = typeof(T).GetProperties();

                foreach (DataRow row in dataTable.Rows)
                {
                    var obj = new T();
                    foreach (var property in properties)
                    {
                        if (dataTable.Columns.Contains(property.Name))
                        {
                            var value = row[property.Name];
                            property.SetValue(obj, value == DBNull.Value ? null : value);
                        }
                    }
                    list.Add(obj);
                }

                return list;
            });
        }

        public async Task<T> ConvertDataTableToSingleInstanceAsync<T>(DataTable dataTable) where T : new()
        {
            if (dataTable.Rows.Count != 1)
                throw new InvalidOperationException("DataTable does not contain exactly one row.");

            return await Task.Run(() =>
            {
                var obj = new T();
                var properties = typeof(T).GetProperties();
                var row = dataTable.Rows[0];

                foreach (var property in properties)
                {
                    if (dataTable.Columns.Contains(property.Name))
                    {
                        var value = row[property.Name];
                        property.SetValue(obj, value == DBNull.Value ? null : value);
                    }
                }

                return obj;
            });
        }
    }
}
