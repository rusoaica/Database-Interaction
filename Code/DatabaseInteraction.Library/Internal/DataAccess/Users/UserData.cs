/// Written by: Yulia Danilova
/// Creation Date: 12th of November 2020
/// Purpose: Bridge-through between database and API for Users
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using DatabaseInteraction.Library.Internal.Enums;
using DatabaseInteraction.Library.Internal.Models.Users;
using DatabaseInteraction.Library.Internal.Models.Common;
using DatabaseInteraction.Library.Internal.DataAccess.Interfaces;
#endregion

namespace DatabaseInteraction.Library.Internal.DataAccess.Users
{
    public class UserData : IUserData
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly ISqlDataAccess sqlDataAccess;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="_sqlDataAccess">The injected SQL data access to use</param>
        public UserData(ISqlDataAccess _sqlDataAccess)
        {
            sqlDataAccess = _sqlDataAccess;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets a user identified by <paramref name="_id"/> using a stored procedure
        /// </summary>
        /// <param name="_id">The Id of the user to get</param>
        /// <returns>A user identified by <paramref name="_id"/></returns>
        public async Task<ServerResponseModel<UserDB>> GetUserByIdWithStoredProcedureAsync(int _id)
        {
            return await sqlDataAccess.LoadDataAsync<UserDB, dynamic>("DefaultConnection", "spGetUserById", new { Id = _id });
        }

        /// <summary>
        /// Gets a user identified by <paramref name="_id"/>
        /// </summary>
        /// <param name="_id">The Id of the user to get</param>
        /// <returns>A user identified by <paramref name="_id"/></returns>
        public async Task<ServerResponseModel<UserDB>> GetUserByIdAsync(string _id)
        {
            return await sqlDataAccess.GenericQueryAsync<UserDB>("DefaultConnection", QueryType.SelectWhere, DatabaseTables.Users, "Id, FirstName, LastName, EmailAddress", "Id", _id);
        }

        /// <summary>
        /// Gets a user identified by <paramref name="_email"/>
        /// </summary>
        /// <param name="_email">The email of the user to get</param>
        /// <returns>A user identified by <paramref name="_email"/></returns>
        public async Task<ServerResponseModel<UserDB>> GetUserByEmailAsync(string _email)
        {
            return await sqlDataAccess.GenericQueryAsync<UserDB>("DefaultConnection", QueryType.SelectWhere, DatabaseTables.Users, "Id, FirstName, LastName, EmailAddress", "EmailAddress", _email);
        }

        /// <summary>
        /// Inserts a new user
        /// </summary>
        /// <param name="_user">The user to insert</param>
        /// <returns>The result of inserting the user</returns>
        public async Task<ServerResponseModel<GenericResponse>> InsertUser(UserDB _user)
        {
            return await sqlDataAccess.SaveDataAsync("spSaveUser", _user, "DefaultConnection");
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="_id">The Id of the user to delete</param>
        /// <returns>The result of deleting the user</returns>
        public async Task<ServerResponseModel<GenericResponse>> DeleteUser(int _id)
        {
            return await sqlDataAccess.DeleteDataAsync("spDeleteUser", "DefaultConnection", _id);
        }
        #endregion
    }
}
