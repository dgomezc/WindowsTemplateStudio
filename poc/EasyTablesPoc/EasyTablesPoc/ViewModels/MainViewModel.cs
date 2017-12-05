using System;

using EasyTablesPoc.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using EasyTablesPoc.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EasyTablesPoc.ViewModels
{
    public class MainViewModel : Observable
    {
        private IMobileServiceTable<Foods> _foodsTable;
        
        public ObservableCollection<Foods> Items = new ObservableCollection<Foods>();
        
        public MainViewModel()
        {
            _foodsTable = App.MobileService.GetTable<Foods>();
        }
        
        public async Task<IEnumerable<Foods>> ReadFoodsAsync()
        {
            return await _foodsTable.ReadAsync();
        }

        private ICommand _populateListCommand;

        public ICommand PopulateListCommand => _populateListCommand ?? (_populateListCommand = new RelayCommand(async () => await PopulateList()));

        private async Task PopulateList()
        {
            var items = await ReadFoodsAsync();

            foreach(var i in items)
            {
                Items.Add(i);
            }
        }
    }
}
