/// Written by: Yulia Danilova
/// Creation Date: 02nd of December 2020
/// Purpose: Interface for getting or saving data to and from the database
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using DatabaseInteraction.Library.Internal.Enums;
using DatabaseInteraction.Library.Internal.Models.Common;
#endregion

namespace DatabaseInteraction.Library.Internal.DataAccess.Interfaces
{
    public interface ISqlDataAccess
    {
        #region ================================================================= METHODS ===================================================================================
        string GetConnectionString(string _name);
        Task<ServerResponseModel<T>> LoadDataAsync<T, U>(string _connectionStringName, string _storedProcedure, U _parameters);
        Task<ServerResponseModel<GenericResponse>> SaveDataAsync<T>(string _storedProcedure, T _parameters, string _connectionStringName);
        Task<ServerResponseModel<GenericResponse>> DeleteDataAsync(string _storedProcedure, string _connectionStringName, int _id = -1);
        Task<ServerResponseModel<T>> GenericQueryAsync<T>(string _connectionStringName, QueryType _queryType, object _table = null, string _columns = "", string _condition = "", string _value = "", string _values = "", bool _usesPseudonym = false);
        #endregion
    }
}
