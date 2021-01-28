/// Written by: Yulia Danilova
/// Creation Date: 22nd of January 2021
/// Purpose: Mock controller for users
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using DatabaseInteraction.Library.Internal.Models.Users;
using DatabaseInteraction.Library.Internal.Models.Common;
using DatabaseInteraction.Library.Internal.DataAccess.Interfaces;
#endregion

namespace DatabaseInteraction.Users
{
    public class UserController
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly IUserData userData;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="_userData">The injected user data to use</param>
        public UserController(IUserData _userData)
        {
            userData = _userData;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Gets a user by id
        /// </summary>
        /// <param name="_id">The id of the user to get</param>
        /// <returns>A user model wrapped in a ServerResponseModel container</returns>
        public async Task<ServerResponseModel<UserDB>> GetById(string _id)
        {
            return await userData.GetUserByIdAsync(_id.ToString());
        }

        /// <summary>
        /// Gets a user by id, using a stored procedure
        /// </summary>
        /// <param name="_id">The id of the user to get</param>
        /// <returns>A user model wrapped in a ServerResponseModel container</returns>
        public async Task<ServerResponseModel<UserDB>> GetByIdWithStoredProcedure(int _id)
        {
            return await userData.GetUserByIdWithStoredProcedureAsync(_id);
        }

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <returns>A user model wrapped in a ServerResponseModel container</returns>
        public async Task<ServerResponseModel<UserDB>> GetByEmail(string _email)
        {
            return await userData.GetUserByEmailAsync(_email);
        }

        /// <summary>
        /// Inserts a user
        /// </summary>
        /// <param name="_user">The user to insert</param>
        /// <returns>The status of the requested operation, wrapped in a ServerResponseModel container</returns>
        public async Task<ServerResponseModel<GenericResponse>> InsertUser(UserDB _user)
        {
            return await userData.InsertUser(_user);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="_id">The id of the user to delete</param>
        /// <returns>The status of the requested operation, wrapped in a ServerResponseModel container</returns>
        public async Task<ServerResponseModel<GenericResponse>> DeleteUser(int _id)
        {
            return await userData.DeleteUser(_id);
        }
        #endregion
    }
}
