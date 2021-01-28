/// Written by: Yulia Danilova
/// Creation Date: 12th of November 2020
/// Purpose: Deserialization model for the Users database table
#region ========================================================================= USING =====================================================================================
using System;
#endregion

namespace DatabaseInteraction.Library.Internal.Models.Users
{
    public class UserDB
    {
        #region ================================================================ PROPERTIES =================================================================================
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Customized ToString() method
        /// </summary>
        /// <returns>Custom string value showing relevant data for current class</returns>
        public override string ToString()
        {
            return Id + " :: " + EmailAddress;
        }
        #endregion
    }
}
