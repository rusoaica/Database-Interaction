# Database Interaction

## Prerequisites
### NuGet dependencies
- Install for the DatabaseInteraction project: `Microsoft.Extensions.DependencyInjection`, `System.Configuration.Abstractions`, `System.Data.SQLite.Core` or any other database provider (`MySql.Data`, etc).
- Install for the DatabaseInteraction.Library project: `Dapper`, `System.Configuration.Abstractions`.

## DatabaseInteraction.Library  
### `UserDB`
Simple sample model mapped to the database columns of the Users table

### `GenericResponse`
Deserialization model for generic responses (ex: inserts, updates or deletes - they don't return any data, so we return status codes. In case of queries that result in an id, such as inserts, the `Id` property stores it).

### `ServerResponseModel<T>`
Container for serialization models supplied from the database. Instead of returning a list of `UserDB` models, we return a `ServerResponseModel<UserDB>`. The `Data` property of this class holds the list of 
`UserDB`, while additionally, the `Count` property returns the number of affected rows, or the number of returned results, and the `Error` property stores a string in case the query produced an error.

### `QueryType`
Enumeration for the type of SQL query to perform, in case of generic queries.

### `DatabaseTables`
Enumeration for the SQL table to use in case of generic queries.

### `ISqlDataAccess`
Interface for interaction with the database. Contains methods for inserting, selecting, updating and deleting data, in a generic manner.

### `SqlDataAccess`
Concrete implementation of [`ISqlDataAccess`](#ISqlDataAccess). It provides two ways of performing CRUD operations on a database: using stored procedures and generic queries. The prefered way should always be the usage of 
stored procedures, since they offer a last defence layer against SQL Injection attacks. The methods designed to use stored procedures are `LoadDataAsync<T, U>`, where `T` represents the model used (`UserDB` in this case), and `U`
is a dynamic object in which we pass the parameters expected by the stored procedure, as an anonymous object.
Usage:  
```csharp
// Get the user which has an id of 1. The stored procedure expects a numeric parameter named Id.
LoadDataAsync<UserDB, dynamic>("DefaultConnection", "spGetUserById", new { Id = 1 });
```

The method which allows a generic query is `GenericQuery<T>`. It can be used to perform any type of SQL query, like the basic CRUD operations, joins, transactions, etc. `T` stands for the type of model returned in case of selects, etc.
Usage:  
```csharp
// get a username using a SelectWhere type of query, on the Users database table, supplying "EmailAddress" as the "where" column and _email as the "where" value_
await GenericQueryAsync<UserDB>("DefaultConnection", QueryType.SelectWhere, DatabaseTables.Users, "Id, FirstName, LastName, EmailAddress", "EmailAddress", _email);

// save a user in the database
await GenericQueryAsync<UserDB>("DefaultConnection", QueryType.Insert, DatabaseTables.Users, "FirstName, LastName, EmailAddress", "", "", "'John', 'Doe', 'test@test.com'");
```

The database connection and the connection string are injected in the constructor using a Dependency Injection container in the [`Program`](#Program) class.

### `IUserData`
Interface for the bridge-through between database and the API endpoint for Users in the [`UsersController`](#UsersController). 

### `UserData`
Class that implements [`IUserData`](#IUserData). Contains methods that take an input from the [`UsersController`](#UsersController), call the [`SqlDataAccess`](#SqlDataAccess) methods that interact with the database, 
then return the result back to the controller, which returns them to the user.

## DatabaseInteraction  
### `UsersController`
Mock-up API controller for the Users API end-point. Includes methods that simulate the implementation of a REST service, such as `GetUserByIdAsync`, `InsertUserAsync`, etc. This is the API endpoint called by an external service/user.

### `Program`
The application's startup class. Here we implement a Dependency Injection container in which we register the services required by the application. It is also the place where the `UserController`'s methods are called, 
simulating a request to an API endpoint. It contains methods such as `GetUserByIdAsync`, `DeleteUserAsync`, etc.
