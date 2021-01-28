/// Written by: Yulia Danilova
/// Creation Date: 10th of November 2020
/// Purpose: Gets or saves data to and from the database
#region ========================================================================= USING =====================================================================================
using Dapper;
using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using DatabaseInteraction.Library.Internal.Enums;
using DatabaseInteraction.Library.Internal.Models.Common;
using DatabaseInteraction.Library.Internal.DataAccess.Interfaces;
#endregion

namespace DatabaseInteraction.Library.Internal.DataAccess.Common
{
    public class SqlDataAccess : ISqlDataAccess
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IDbConnection databaseConnection;
        private readonly IConfigurationManager configuration;
        private readonly Dictionary<DatabaseTables, string> databaseTableNamesMaping = new Dictionary<DatabaseTables, string>();
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="_configuration">The injected configuration to use</param>
        /// <param name="_databaseConnection">The injected database connection to use</param>
        public SqlDataAccess(IConfigurationManager _configuration, IDbConnection _databaseConnection)
        {
            configuration = _configuration;
            databaseConnection = _databaseConnection;
            MapDatabaseTableNames();
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Maps the names of the database columns to the names of the columns used in application
        /// </summary>
        private void MapDatabaseTableNames()
        {
            databaseTableNamesMaping.Add(DatabaseTables.Users, "Users");
        }

        /// <summary>
        /// Gets a connection string from web.config whose name matches <paramref name="_name"/>
        /// </summary>
        /// <param name="_name">The name of the connection string</param>
        /// <returns>A connection string from web.config that matches <paramref name="_name"/></returns>
        public string GetConnectionString(string _name)
        {
            return configuration.ConnectionStrings[_name].ConnectionString;
        }

        /// <summary>
        /// Loads data from the database using a direct sql query. This method should be avoided, and the one using a stored procedure should be used instead! 
        /// This method only exists for databases that do not support stored procedures, such as SQLite
        /// </summary>
        /// <typeparam name="T">The model type to use for the returned rows</typeparam>
        /// <param name="_connectionStringName">The name of the connection string to be used</param>
        /// <param name="_queryType">The SQL operation to perform</param>
        /// <param name="_table">The SQL table upon which the query is performed</param>
        /// <param name="_columns">The columns of the table to perform query to</param>
        /// <param name="_condition">For "SELECT WHERE" cases, specifies condition</param>
        /// <param name="_value">For "SELECT WHERE" cases, specifies condition value</param>
        /// <param name="_values">For INSERT query only, specifies array of values to be inserted</param>
        /// <param name="_usesPseudonym">If true, automatically adds an SQL table pseudonym</param>
        /// <returns>The result of executing the sql query</returns>
        public async Task<ServerResponseModel<T>> GenericQueryAsync<T>(string _connectionStringName, QueryType _queryType, object _table = null, string _columns = "", string _condition = "", string _value = "", string _values = "", bool _usesPseudonym = false)
        {
            ServerResponseModel<T> _serialized_data = new ServerResponseModel<T>();
            // get the connection string
            string _connection_string = GetConnectionString(_connectionStringName);
            // check whether the database table name is sent as a predefined DatabaseTables value, or as custom string (which can include joins, etc, so it can contain more than a single table name)
            string _database_table = _table is DatabaseTables _t ? databaseTableNamesMaping[_t] : _table as string;
            // when using joins, adds a table name pseudonym automatically (ex: Users AS u)
            if (_usesPseudonym)
                _database_table += " AS " + _database_table[0].ToString().ToLower();
            // reconstruct the SQL queries from the parameters
            string _query = "";
            switch (_queryType)
            {
                case QueryType.Transaction:
                    _query = _database_table;
                    break;
                case QueryType.Select:
                    _query = "SELECT " + _columns + " FROM " + _database_table + ";";
                    break;
                case QueryType.Delete:
                    _query = "DELETE FROM " + _database_table + " WHERE " + _condition + " = " + _value + ";";
                    break;
                case QueryType.Insert:
                    _query = "INSERT INTO " + _database_table + "(" + _columns + ") VALUES (" + _values + ");";
                    break;
                case QueryType.Update:
                    _query = "UPDATE " + _database_table + " SET " + _values + " WHERE " + _condition + " = " + _value + ";";
                    break;
                case QueryType.SelectWhere:
                    _query = "SELECT " + _columns + " FROM " + _database_table + " WHERE " + _condition + " = " + _value + ";";
                    break;
                case QueryType.SelectWhereLike:
                    _query = "SELECT " + _columns + " FROM " + _database_table + " WHERE " + _condition + " LIKE " + _value + ";";
                    break;
                case QueryType.SelectWhereBetween:
                    _query = "SELECT " + _columns + " FROM " + _database_table + " WHERE " + _condition + " BETWEEN " + _value + ";";
                    break;
            }
            // preview the query on the console
            Console.WriteLine(_query);
            try
            {
                // execute the stored procedure and return the rows list
                databaseConnection.ConnectionString = _connection_string;
                T[] _temp = (await databaseConnection.QueryAsync<T>(_query)).ToArray();
                // assign the received data
                _serialized_data.Data = _temp.Length > 0 ? _temp : null;
                _serialized_data.Count = _temp.Length;
            }
            catch (Exception _ex)
            {
                // in case of exceptions, populate the Error property
                _serialized_data.Error = _ex.Message;
            }
            return _serialized_data;
        }

        /// <summary>
        /// Loads data from the database using a stored procedure
        /// </summary>
        /// <typeparam name="T">The model type to use for the returned rows</typeparam>
        /// <typeparam name="U">The type used for the parameters</typeparam>
        /// <param name="_connectionStringName">The name of the connection string to be used</param>
        /// <param name="_storedProcedure">The name of the stored procedure to be called</param>
        /// <param name="_parameters">The parameters to be passed to the stored procedure</param>
        /// <returns>The result of executing the stored procedure <paramref name="_storedProcedure"/> with the <paramref name="_parameters"/> parameters</returns>
        public async Task<ServerResponseModel<T>> LoadDataAsync<T, U>(string _connectionStringName, string _storedProcedure, U _parameters)
        {
            ServerResponseModel<T> _serialized_data = new ServerResponseModel<T>();
            // get the connection string
            string _connection_string = GetConnectionString(_connectionStringName);
            try
            {
                // execute the stored procedure and return the rows list
                databaseConnection.ConnectionString = _connection_string;
                // execute the stored procedure
                T[] _temp = (await databaseConnection.QueryAsync<T>(_storedProcedure, _parameters, commandType: CommandType.StoredProcedure)).ToArray();
                // assign the received data
                _serialized_data.Data = _temp.Length > 0 ? _temp : null;
                _serialized_data.Count = _temp.Length;
            }
            catch (Exception _ex)
            {
                // in case of exceptions, populate the Error property
                _serialized_data.Error = _ex.Message;
            }
            return _serialized_data;
        }

        /// <summary>
        /// Saves data to the database
        /// </summary>
        /// <typeparam name="T">The model type to save to the database</typeparam>
        /// <param name="_storedProcedure">The name of the stored procedure to be executed</param>
        /// <param name="_parameters">The parameters to be passed to the stored procedure</param>
        /// <param name="_connectionStringName">The name of the connection string to be used</param>
        /// <returns>The result of executing the stored procedure <paramref name="_storedProcedure"/> with the <paramref name="_parameters"/> parameters</returns>
        public async Task<ServerResponseModel<GenericResponse>> SaveDataAsync<T>(string _storedProcedure, T _parameters, string _connectionStringName)
        {
            ServerResponseModel<GenericResponse> _serialized_data = new ServerResponseModel<GenericResponse>();
            // get the connection string
            string _connection_string = GetConnectionString(_connectionStringName);
            try
            {
                // execute the stored procedure 
                databaseConnection.ConnectionString = _connection_string;
                int _output_id = await databaseConnection.ExecuteScalarAsync<int>(_storedProcedure, _parameters, commandType: CommandType.StoredProcedure);
                // populate the Data property. In this case, since we performed an insert, we can only return the id of the newly inserted row
                _serialized_data.Data = new GenericResponse[1] { new GenericResponse() };
                _serialized_data.Data[0].StatusData = "success";
                _serialized_data.Data[0].Id = _output_id;
            }
            catch (Exception _ex)
            {
                // in case of exceptions, populate the Error property
                _serialized_data.Error = _ex.Message;
            }
            return _serialized_data;
        }

        /// <summary>
        /// Deletes data from the database
        /// </summary>
        /// <param name="_storedProcedure">The name of the sotred procedure to be executed</param>
        /// <param name="_id">The id of the record to delete</param>
        /// <param name="_connectionStringName">The name of the connection string to be used</param>
        /// <returns>The result of executing the stored procedure <paramref name="_storedProcedure"/> for the record identified by <paramref name="_parameters"/></returns>
        public async Task<ServerResponseModel<GenericResponse>> DeleteDataAsync(string _storedProcedure, string _connectionStringName, int _id = -1)
        {
            ServerResponseModel<GenericResponse> _serialized_data = new ServerResponseModel<GenericResponse>();
            // get the connection string
            string _connection_string = GetConnectionString(_connectionStringName);
            databaseConnection.ConnectionString = _connection_string;
            try
            {
                // execute the stored procedure 
                DynamicParameters _param = new DynamicParameters();
                if (_id != -1)
                    _param.Add("@Id", _id, DbType.Int32);
                int _affected_rows = await databaseConnection.ExecuteAsync(_storedProcedure, _param, commandType: CommandType.StoredProcedure);
                // populate the Data property. In this case, since we performed a delete, we can only return the number of affected rows
                _serialized_data.Data = new GenericResponse[1] { new GenericResponse() };
                _serialized_data.Data[0].StatusData = "deleted";
                _serialized_data.Count = _affected_rows;
            }
            catch (Exception _ex)
            {
                // in case of exceptions, populate the Error property
                _serialized_data.Error = _ex.Message;
            }
            return _serialized_data;
        }
        #endregion
    }
}