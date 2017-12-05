using EasyTablesPoc.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyTablesPoc.Services
{
    public abstract class MobileBaseService<T> where T : EasyTableBase
    {
        private MobileServiceClient _client;
        private IMobileServiceTable<T> _table;        

        protected MobileBaseService()
        {
            if (_client != null)
                return;

            _client = new MobileServiceClient(GlobalSettings.AzureServiceEndPoint);
            _table = _client.GetTable<T>();
        }

        public virtual async Task<IEnumerable<T>> ReadAsync()
        {
            return await _table.ReadAsync();
        }

        public virtual async Task AddOrUpdateAsync(T item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await _table.InsertAsync(item);
            }
            else
            {
                await _table.UpdateAsync(item);
            }
        }

        public virtual async Task DeleteAsync(T item)
        {
            await _table.DeleteAsync(item);
        }
    }
}
