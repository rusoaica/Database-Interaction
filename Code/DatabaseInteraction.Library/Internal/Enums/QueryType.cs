/// Written by: Yulia Danilova
/// Creation Date: 8th of June, 2020
/// Purpose: Enumeration for SQL query types, determine the type of SQL query to perform

namespace DatabaseInteraction.Library.Internal.Enums
{
    public enum QueryType
    {
        Select,
        Insert,
        Update,
        Delete,
        Transaction,
        SelectWhere,
        SelectWhereLike,
        SelectWhereBetween
    }
}
