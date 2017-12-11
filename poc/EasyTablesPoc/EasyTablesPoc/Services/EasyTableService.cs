using EasyTablesPoc.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace EasyTablesPoc.Services
{
    public abstract class EasyTableService<T> where T : EasyTableBase
    {
        private MobileServiceClient _client;
        private IMobileServiceSyncTable<T> _table;

        public async Task InitializeAsync()
        {
            if (_client != null)
                return;

            var store = new MobileServiceSQLiteStore(GlobalSettings.SqliteDbName);
            store.DefineTable<T>();

            _client = App.MobileService;
            _table = _client.GetSyncTable<T>();

            await _client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            if (HasInternet())
            {
                try
                {
                    await _client.SyncContext.PushAsync();
                    await _table.PullAsync($"all{typeof(T).Name}", _table.CreateQuery());
                }
                catch (Exception)
                {
                    //TODO
                }
            }
        }

        public virtual async Task<IEnumerable<T>> ReadAsync()
        {
            await InitializeAsync();
            return await _table.ReadAsync();
        }

        public virtual async Task AddOrUpdateAsync(T item)
        {
            await InitializeAsync();

            if (string.IsNullOrEmpty(item.Id))
            {
                await _table.InsertAsync(item);
            }
            else
            {
                await _table.UpdateAsync(item);
            }

            await SynchronizeItemAsync(item.Id);
        }

        public virtual async Task DeleteAsync(T item)
        {
            await InitializeAsync();

            await _table.DeleteAsync(item);

            await SynchronizeItemAsync(item.Id);

        }

        private async Task SynchronizeItemAsync(string itemId)
        {
            if (!HasInternet())
                return;

            try
            {
                await _client.SyncContext.PushAsync();
                await _table.PullAsync($"sync{typeof(T).Name}" + itemId, _table.Where(r => r.Id == itemId));
            }
            catch (MobileServicePushFailedException ex)
            {
                if (ex.PushResult != null)
                {
                    foreach (var result in ex.PushResult.Errors)
                    {
                        await ResolveErrorAsync(result);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task ResolveErrorAsync(MobileServiceTableOperationError result)
        {
            if (result.Result == null || result.Item == null)
                return;

            var serverItem = result.Result.ToObject<T>();
            var localItem = result.Item.ToObject<T>();

            if (ItemsAreEquals(serverItem, localItem))
            {
                // The elements are equals, ignore the conflict
                await result.CancelAndDiscardItemAsync();
            }
            else
            {
                // Client win
                localItem.AzureVersion = serverItem.AzureVersion;
                await result.UpdateOperationAsync(JObject.FromObject(localItem));
            }
        }

        protected virtual bool ItemsAreEquals(T serverItem, T localItem)
        {
            return serverItem.Id == localItem.Id;
        }

        private bool HasInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            var hasInternet = connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;

            return hasInternet;
        }
    }
}
