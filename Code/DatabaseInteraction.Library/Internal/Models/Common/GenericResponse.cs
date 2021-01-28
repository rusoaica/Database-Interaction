/// Written by: Yulia Danilova
/// Creation Date: 13th of January, 2020
/// Purpose: Deserialization model for generic response returned from server API (example: insert, update or delete status codes)
namespace DatabaseInteraction.Library.Internal.Models.Common
{
    public class GenericResponse
    {
        #region ================================================================ PROPERTIES =================================================================================
        public int Id { get; set; } = -1;
        public string StatusData { get; set; }
        #endregion
    }
}
