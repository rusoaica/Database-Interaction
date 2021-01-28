/// Written by: Yulia Danilova
/// Creation Date: 02nd of December 2020
/// Purpose: Interface for the bridge-through between database and API for Users
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
using DatabaseInteraction.Library.Internal.Models.Users;
using DatabaseInteraction.Library.Internal.Models.Common;
#endregion

namespace DatabaseInteraction.Library.Internal.DataAccess.Interfaces
{
    public interface IUserData
    {
        #region ================================================================= METHODS ===================================================================================
        Task<ServerResponseModel<UserDB>> GetUserByIdWithStoredProcedureAsync(int _id);
        Task<ServerResponseModel<UserDB>> GetUserByIdAsync(string _id);
        Task<ServerResponseModel<UserDB>> GetUserByEmailAsync(string _name);
        Task<ServerResponseModel<GenericResponse>> InsertUser(UserDB _user);
        Task<ServerResponseModel<GenericResponse>> DeleteUser(int _id);
        #endregion
    }
}
