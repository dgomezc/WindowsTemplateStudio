using EasyTablesPoc.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;

namespace EasyTablesPoc.Services
{
    public class MobileService
    {
        private const string AzureServiceEndPoint = "INSERT AZURE URL";
        private const string SqliteDbName = "localstore.db";

        private static MobileService _instance;
        public static MobileService Instance => _instance ?? (_instance = new MobileService());

        public MobileServiceClient Client { get; } = new MobileServiceClient(AzureServiceEndPoint);

        public MobileServiceSQLiteStore Store { get; private set; }

        public async Task InitializeAsync()
        {
            if (!Client.SyncContext.IsInitialized)
            {
                Store = new MobileServiceSQLiteStore(SqliteDbName);
                RegisterTables();
                await Client.SyncContext.InitializeAsync(Store, new MobileServiceSyncHandler());
            }                
        }

        private void RegisterTables()
        {
            Store.DefineTable<Food>();
            Store.DefineTable<TodoItem>();

            //TODO - Register models to tables.
        }
    }
}
