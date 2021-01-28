/// Written by: Yulia Danilova
/// Creation Date: 13th of January, 2020
/// Purpose: Container for serialization models supplied by the server API
namespace DatabaseInteraction.Library.Internal.Models.Common
{
    public class ServerResponseModel<T>
    {
        #region ================================================================ PROPERTIES =================================================================================
        public T[] Data { get; set; }
        public int Count { get; set; }
        public string Error { get; set; }
        #endregion
    }
}
