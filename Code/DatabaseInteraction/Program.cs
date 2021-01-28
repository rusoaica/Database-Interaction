/// Written by: Yulia Danilova
/// Creation Date: 22nd of January, 2021
/// Purpose: Startup class of the application
#region ========================================================================= USING =====================================================================================
using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using DatabaseInteraction.Users;
using System.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DatabaseInteraction.Library.Internal.Models.Users;
using DatabaseInteraction.Library.Internal.Models.Common;
using DatabaseInteraction.Library.Internal.DataAccess.Users;
using DatabaseInteraction.Library.Internal.DataAccess.Common;
using DatabaseInteraction.Library.Internal.DataAccess.Interfaces;
#endregion

namespace DatabaseInteraction
{
    internal class Program
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private static IServiceCollection _services;
        private static IServiceProvider serviceProvider;
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Application entry point
        /// </summary>
        private static void Main()
        {
            Init().GetAwaiter().GetResult();
            // wait for user input
            Console.ReadLine();
        }

        /// <summary>
        /// Register services into the IServiceCollection, used in the Dependency Injection system
        /// </summary>
        /// <param name="_services">The IServiceCollection to add the services to</param>
        private static void RegisterServices(IServiceCollection _services)
        {
            // register the service for interacting with the program's configuration
            _services.AddSingleton<IConfigurationManager, ConfigurationManager>();

            // register the service for interacting with the User controller
            _services.AddTransient<IUserData, UserData>();

            // for using MySQL database, install MySQL.Data NuGet, then:
            //_services.AddTransient<IDbConnection, MySqlConnection>();

            // for using SqlServer database, use System.Data.SqlClient, then:
            //_services.AddTransient<IDbConnection, SqlConnection>();

            // for using SQLite database, install System.Data.SQLite.Core, then:
            _services.AddTransient<IDbConnection, SQLiteConnection>();

            // register the service for interacting with the database
            _services.AddTransient<ISqlDataAccess, SqlDataAccess>();
        }

        /// <summary>
        /// Method for registering Dependency Injection services and interacting with the mock-up API controller
        /// </summary>
        private static async Task Init()
        {
            // create an instance of a service collection
            _services = new ServiceCollection();
            // add the required services to the service collection
            RegisterServices(_services);
            // create a service provider with the registered services
            serviceProvider = _services.BuildServiceProvider();

            // create an instance of the mock-up api controller
            UserController _user_controller = new UserController(serviceProvider.GetService<IUserData>());
            // get a user using a name
            ServerResponseModel<UserDB> _first_user = await GetUserByEmailAsync(_user_controller);
            // get a user uding an Id
            ServerResponseModel<UserDB> _second_user = await GetUserByIdAsync(_user_controller);
            // since this application uses SQLite, which doesn't support stored procedures, the calls below won't work with this kind of database
            return;
            // create a test user
            UserDB _user = new UserDB()
            {
                EmailAddress = "test@test.com",
                FirstName = "test",
                LastName = "test"
            };
            // insert the test user in the database
            ServerResponseModel<GenericResponse> _user_insert = await InsertUserAsync(_user_controller, _user);
            // delete the test user
            ServerResponseModel<GenericResponse> _user_delete = await DeleteUserAsync(_user_controller, _user_insert.Data[0].Id);
        }

        /// <summary>
        /// Calls the mock-up API controller endpoint for getting a user by email
        /// </summary>
        /// <param name="_user_controller">The mock-up API user controller</param>
        /// <returns>A UserDB model wrapped in a ServerResponseModel</returns>
        private static async Task<ServerResponseModel<UserDB>> GetUserByEmailAsync(UserController _user_controller)
        {
            return await _user_controller.GetByEmail("'test@test.com'");
        }

        /// <summary>
        /// Calls the mock-up API controller endpoint for getting a user by Id
        /// </summary>
        /// <param name="_user_controller">The mock-up API user controller</param>
        /// <returns>A UserDB model wrapped in a ServerResponseModel</returns>
        private static async Task<ServerResponseModel<UserDB>> GetUserByIdAsync(UserController _user_controller)
        {
            return await _user_controller.GetById("'1'");
            // or:
            return await _user_controller.GetByIdWithStoredProcedure(1); // invalid on databases that don't support stored procedures
        }

        /// <summary>
        /// Calls the mock-up API controller endpoint for inserting a user 
        /// </summary>
        /// <param name="_user_controller">The mock-up API user controller</param>
        /// <param name="_user">The user to insert</param>
        /// <returns>A GenericResponse model wrapped in a ServerResponseModel, containing the status of the requested operation</returns>
        private static async Task<ServerResponseModel<GenericResponse>> InsertUserAsync(UserController _user_controller, UserDB _user)
        {
            return await _user_controller.InsertUser(_user); // invalid on databases that don't support stored procedures
        }

        /// <summary>
        /// Calls the mock-up API controller endpoint for deleting a user
        /// </summary>
        /// <param name="_user_controller">The mock-up API user controller</param>
        /// <param name="_id">The id of the user to delete</param>
        /// <returns>A GenericResponse model wrapped in a ServerResponseModel, containing the status of the requested operation</returns>
        private static async Task<ServerResponseModel<GenericResponse>> DeleteUserAsync(UserController _user_controller, int _id)
        {
            return await _user_controller.DeleteUser(_id); // invalid on databases that don't support stored procedures
        }
        #endregion
    }
}
